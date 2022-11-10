using System.Text.RegularExpressions;

namespace CoreAPI.Utilities
{
  public static class SymbolExtensions
  {
    public static string ClearSymbol(this string  symbol)
    {
      string _symbol = symbol.Substring(0, symbol.IndexOf('.'));
      _symbol = Regex.Replace(_symbol, @"[\d-]", string.Empty);
      return _symbol; 
    }

    public static bool isValidBRSymbol(this string symbol)
    {
      if(Regex.IsMatch(symbol, @"\d+"))
        return true;
      else 
        return false;
    }

    public static string SymbolInjectSA(this string symbol)
    {
      string _symbol = symbol;
      if (_symbol.IndexOf(".SA") == -1)
        return _symbol += ".SA";
      else{
        return _symbol;
      }      
    }

    public static string GetCompanyType(this string symbol)
    {
      string _symbol = symbol.Substring(0, symbol.IndexOf('.'));
      string _type = Regex.Match(_symbol, @"\d+").Value;

      switch(_type){
        case "3":
          return "ON";
        case "4":
          return "PN";
        default:  return "UNIT";
      }
    }
  }
}