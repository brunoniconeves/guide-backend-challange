using System;

namespace guide.Domain.Models.Company
{
  public class CompanyPriceHistory
  {
    public int id { get; set; }
    public int companyId { get; set; }
    public DateTime date { get; set; }
    public double openPrice { get; set; }
    public double d1VariationPercentage { get; set; }
    public double firstPriceVariationPercentage { get; set; }

    public CompanyPriceHistory(
      int id, 
      int companyId,
      DateTime date,
      double openPrice,
      double d1VariationPercentage,
      double firstPriceVariationPercentage
    ) {
      this.id = id;
      this.companyId = companyId;
      this.date = date;
      this.openPrice = openPrice;
      this.d1VariationPercentage = d1VariationPercentage;
      this.firstPriceVariationPercentage = firstPriceVariationPercentage; 
    }
  }
}
