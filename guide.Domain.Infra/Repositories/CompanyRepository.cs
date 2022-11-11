using Dapper;
using guide.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using guide.Domain.Models.Company;
using Microsoft.Extensions.Configuration;

namespace guide.Domain.Infra.Repositories
{
  public class CompanyRepository : ICompanyRepository
  {
    private readonly IDbConnection _conn;   
    private readonly IConfiguration _config;
   
    public CompanyRepository(IConfiguration config)
    {
      _config = config;
      _conn = new SqlConnection(_config["ConnectionStrings:GuideChallange"]);      
    }


    public CompanyInfo getCompanyInfo(string symbol)
    {
      try
      {
        string query = @"SELECT 
                        id,
                        symbol,
                        friendlyName,
                        type,
                        currency,
                        companyLogo
                      FROM Company
                      WHERE	symbol like @symbol";

        var param = new { symbol = string.Format("%{0}%", symbol) };
        var result = _conn.QuerySingle<CompanyInfo>(query, param);

        return result;
      }catch (System.InvalidOperationException e)
      {
        return null;
      }
    }

    public IEnumerable<CompanyPriceHistory> getCompanyPriceHistory(string symbol)
    {
      string query = @"SELECT
                        h.id,
                        companyId,
                        c.companyLogo,
                        date,
                        openPrice,
                        d1VariationPercentage,
                        firstPriceVariationPercentage                       
                      FROM 	CompanyPriceHistory h
                      INNER JOIN Company c ON h.companyId = c.id
                      WHERE	c.symbol like @symbol
                      ORDER BY date DESC;";

      IEnumerable<CompanyPriceHistory> result = _conn.Query<CompanyPriceHistory>(query, new { symbol = string.Format("%{0}%", symbol) });

      
      return result;
    }

    public CompanyPriceHistory getCompanyLastPrice(string symbol)
    {
      try
      {
        string query = @"SELECT
                        h.id,
                        companyId,
                        date,
                        openPrice,
                        d1VariationPercentage,
                        firstPriceVariationPercentage
                      FROM 	CompanyPriceHistory h
                      INNER JOIN Company c ON h.companyId = c.id
                      WHERE	
                        c.symbol like @symbol AND
                        h.date = (
                          SELECT
                            MAX(date)
                          FROM 	CompanyPriceHistory h1
                          INNER JOIN Company c1 ON h1.companyId = c1.id
                          WHERE	c1.symbol like @symbol
                        )
                      ;";

        var result = _conn.QuerySingle<CompanyPriceHistory>(query, new { symbol = string.Format("%{0}%", symbol) });

        return result;
      }
      catch (System.InvalidOperationException e)
      {
        throw e;
      }
    }


    public bool insertCompanyPriceHistory(IEnumerable<CompanyPriceHistory> companyPriceHistory)
    {
      // Clear the price history
      _conn.Execute(
        "DELETE FROM CompanyPriceHistory WHERE companyId = @companyId", new { companyId = companyPriceHistory.First().companyId});

      // insert new price history
      _conn.Execute(
          "INSERT INTO CompanyPriceHistory VALUES (" +
          " @companyId, " +
          " @date," +
          " @openPrice," +
          " @d1VariationPercentage," +
          " @firstPriceVariationPercentage" +
          ")", companyPriceHistory
      );      

      return true;
    }

    public int insertCompanyInfo(CompanyInfo companyInfo)
    {
      var id = _conn.QuerySingle<int>(@"
        INSERT INTO Company (symbol, friendlyName, type, currency, companyLogo) 
        VALUES (@symbol, @friendlyName, @type, @currency, @companyLogo);
        SELECT CAST(SCOPE_IDENTITY() as int)", 
        new { 
          symbol = companyInfo.symbol,
          friendlyName = companyInfo.friendlyName,
          type = companyInfo.type,
          currency = companyInfo.currency,
          companyLogo = companyInfo.companyLogo
        }
      );

      return id;
    }

    public bool updateCompanyInfo(CompanyInfo companyInfo)
    {
      var id = _conn.Execute(@"
        UPDATE Company SET
          symbol = @symbol, 
          friendlyName = @friendlyName, 
          type = @type,
          currency = @currency, 
          companyLogo = @companyLogo
        WHERE id = @id
        ",
        new
        {
          id = companyInfo.id,
          symbol = companyInfo.symbol,
          friendlyName = companyInfo.friendlyName,
          type = companyInfo.type,
          currency = companyInfo.currency,
          companyLogo = companyInfo.companyLogo
        }
      );

      return true;
    }
  }
}
