using Microsoft.AspNetCore.Mvc;
using guide.Domain.Repositories;
using guide.Domain.Models.Company;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoreAPI.Utilities;

namespace guide.Domain.Api.Controllers;

[ApiController]
[Route("guide/company/{symbol}")]
public class CompanyController : ControllerBase
{
  private readonly ICompanyRepository _companyRepository;

  public CompanyController(ICompanyRepository companyRepository)
  {
    _companyRepository = companyRepository;
  }

  /// <summary>
  /// Get stock information.
  /// </summary>
  /// <param name="symbol">Is the ticker of the asset (i.e ABEV3, SAPR11, PETR4)</param>
  /// <returns>General information about some asset</returns>
  [HttpGet("info")]
  public ObjectResult getCompanyInfo(string symbol)
  {
    try
    {
      if (!symbol.isValidBRSymbol())
      {
        return BadRequest("Informe um ativo válido (ex. PETR3, SAPR11, ITUB4)");
      }

      CompanyInfo companyInfo = _companyRepository.getCompanyInfo(symbol);

      if(companyInfo == null){
        return NotFound("Ativo não encontrado, execute o endpoin updateHistory antes de buscar o ativo, histórico ou ultmo preço.");
      }

      return Ok(companyInfo);
    }catch (Exception ex){      
        return StatusCode(500, "Erro interno.");
    }
  }

  /// <summary>
  /// Get prices history of an asset.
  /// </summary>
  /// <param name="symbol">Is the ticker of the asset (i.e ABEV3, SAPR11, PETR4)</param>
  /// <returns>
  /// Lista de preços para o ativo, contendo a data, o valor de abertura, 
  /// a variação para o último dia e para o início do período de 30 dias.
  /// </returns>
  [HttpGet("priceHistory")]
  public ObjectResult getCompanyPriceHistory(string symbol)
  {
    IEnumerable<CompanyPriceHistory> result = _companyRepository.getCompanyPriceHistory(symbol);

    if (result.Count() == 0)
    {
      return NotFound("Este ativo não possui histórico, verifique se o código está correto.");
    }

    return Ok(result);
  }

  /// <summary>
  /// Get the last quote for an asset.
  /// </summary>
  /// <param name="symbol">Is the ticker of the asset (i.e ABEV3, SAPR11, PETR4)</param>
  /// <returns>O último preço do ativo.</returns>
  [HttpGet("lastPrice")]
  public ObjectResult getCompanyLastPrice(string symbol)
  {
    try
    {
      if (!symbol.isValidBRSymbol())
      {
        return BadRequest("Informe um ativo válido (ex. PETR3, SAPR11, ITUB4)");
      }

      return Ok(_companyRepository.getCompanyLastPrice(symbol));
    }
    catch (Exception ex)
    {
      if (ex.Message == "Sequence contains no elements")
        return NotFound("Ativo não encontrado, execute o endpoin updateHistory antes de buscar o ativo, histórico ou ultmo preço.");
      else
      {
        return StatusCode(500, "Erro interno.");
      }
    }
    
  }

  /// <summary>
  /// Update the asset and his history quotes.
  /// Should be called for each asset new on the database that we want to test.
  /// The endpoint updates the asset on the database and rebuild the entire quotes history 
  /// of the last 30 days. It connects to Yahoo Finance API to collect the data.
  /// </summary>
  /// <param name="symbol">Is the ticker of the asset (i.e ABEV3, SAPR11, PETR4)</param>
  /// <response code="200">Asset data and history prices updated</response>
  [HttpPost("updateHistory")]
  public async Task<ObjectResult> updateCompanyPriceHistoryAsync(string symbol)
  {
    var client = new RestClient($"https://query1.finance.yahoo.com/v8/finance/chart/");
    var request = new RestRequest(symbol.SymbolInjectSA() + "?metrics=high?&interval=1d&range=31d", Method.Get);
    var response = await client.ExecuteAsync(request);

    if (response.IsSuccessful)
    {
      if(response.Content == null)
        return NotFound(string.Format("No data found for the symbol {0}", symbol + ".SA"));

      var content = JsonConvert.DeserializeObject<JToken>(response.Content);

      //Define the base element
      var result = content["chart"]["result"][0];

      
      string _symbol = result["meta"]["symbol"].Value<string>();
      string _friendlyName = _symbol;
      string _type = _symbol.GetCompanyType();
      string _currency = result["meta"]["currency"].Value<string>();
      _friendlyName = _symbol.ClearSymbol();

      CompanyInfo companyInfo = _companyRepository.getCompanyInfo(_symbol);

      if(companyInfo == null){
        companyInfo = new CompanyInfo(
          0,
          _symbol,
          _friendlyName,
          _type,  
          _currency,
          string.Format("https://bastter-storage.b-cdn.net/acao/{0}.gif", _friendlyName)
        );
        companyInfo.id = _companyRepository.insertCompanyInfo(companyInfo);
        
      } else {
        companyInfo.symbol = _symbol;
        companyInfo.friendlyName = _friendlyName;
        companyInfo.type = _type; 
        companyInfo.currency = _currency;
        companyInfo.companyLogo = string.Format("https://bastter-storage.b-cdn.net/acao/{0}.gif", _friendlyName);
        _companyRepository.updateCompanyInfo(companyInfo);
      }

      companyInfo.currency = _currency;

      List<CompanyPriceHistory> companyPriceHistoryList = new List<CompanyPriceHistory>();

      //for each day, we iterate to create a CompanyPriceHistory element
      //we also calculate the d-1 and d-30 variation to persist
      double _firstOpenPrice = result["indicators"]["quote"][0]["open"][0].Value<double>();
      for (var i= 0; i < result["timestamp"].Count(); i++){

        // extension UnixTimestampToDateTime was created to handle
        // the Unix Timestamp format from Yahoo Finance
        int _date = result["timestamp"][i].Value<int>();
        
        double _openPrice = result["indicators"]["quote"][0]["open"][i].Value<double>();

        //found cases where open price is 0.0, I will not put on DB that trash value
        if (_openPrice == 0)
          continue;

        double _d1openPrice = 0;
        double _d1VariationPercentage = 0;
        double _firstPriceVariationPercentage = 0;

        // we only calculate the d-1 variation if there is enough data
        if (i != 0 && result["timestamp"].Count() > 1) {
          _d1openPrice = result["indicators"]["quote"][0]["open"][i - 1].Value<double>();
          double _d1variantionFactor = _openPrice / _d1openPrice;
          double _firstPriceVariantionFactor = _openPrice / _firstOpenPrice;
          _d1VariationPercentage = Math.Round(Math.Abs((_d1variantionFactor) -1) * 100, 2);
          _firstPriceVariationPercentage = Math.Round(Math.Abs((_firstPriceVariantionFactor) - 1) * 100, 2);

          //positive or negative variation for d1
          if (_d1variantionFactor < 1){
            //negative
            _d1VariationPercentage *= -1;
          }

          //positive or negative variation for firstPrice
          if (_firstPriceVariantionFactor < 1)
          {
            //negative
            _firstPriceVariationPercentage *= -1;
          }
        } else {
          _d1VariationPercentage = 0;
          _firstPriceVariationPercentage = 0;
        }

        companyPriceHistoryList.Add(
          new CompanyPriceHistory(
            0,
            companyInfo.id,
            "",
            _date,
            _openPrice,   
            _d1VariationPercentage,
            _firstPriceVariationPercentage
          )
        );
      }

      _companyRepository.insertCompanyPriceHistory(companyPriceHistoryList);

      return Ok("Histórico do ativo atualizado.");
    } else {
      return NotFound("Ativo não localizado na API do Yahoo Finance.");
    }
  }
}
