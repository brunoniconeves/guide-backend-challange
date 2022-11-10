using System;

namespace guide.Domain.Infra.DTOs
{
  public class CompanyPriceHistoryDTO
  {
    public int id { get; set; }
    public int companyId { get; set; }
    public int date { get; set; }
    public double openPrice { get; set; }
    public double d1VariationPercentage { get; set; }
    public double firstPriceVariationPercentage { get; set; }
  }
}
