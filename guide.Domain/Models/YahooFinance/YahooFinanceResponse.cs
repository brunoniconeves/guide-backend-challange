using System.Collections.Generic;

namespace guide.Domain.Models.YahooFinance
{
  public class YahooFinanceResponse
  {
    public string symbol { get; set; }
    public string currency { get; set; }
    public List<int> timestamp { get; set; }
    public List<double> openPrice { get; set; }

    public YahooFinanceResponse(
      string symbol, 
      string currency,
      List<int> timestamp,
      List<double> openPrice
    ) {
      this.symbol = symbol;
      this.currency = currency;
      this.timestamp = timestamp;
      this.openPrice = openPrice;
    }
  }
}
