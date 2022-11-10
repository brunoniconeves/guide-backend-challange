namespace guide.Domain.Models.Company
{
  public class CompanyInfo
  {
    public int id { get; set; }
    public string symbol { get; set; }
    public string friendlyName { get; set; }
    public string type { get; set; }
    public string currency { get; set; }
    public string companyLogo { get; set; }

    public CompanyInfo(
      int id,
      string symbol,
      string friendlyName,
      string type,  
      string currency,
      string companyLogo
    ) {
      this.id = id; 
      this.symbol = symbol;
      this.friendlyName = friendlyName;
      this.type = type;
      this.currency = currency;
      this.companyLogo = companyLogo;
    }
  }
}
