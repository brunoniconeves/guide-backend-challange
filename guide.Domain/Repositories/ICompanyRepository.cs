using guide.Domain.Models.Company;
using System.Collections.Generic;

namespace guide.Domain.Repositories
{
  public interface ICompanyRepository
  {
    CompanyInfo getCompanyInfo(string symbol);
    IEnumerable<CompanyPriceHistory> getCompanyPriceHistory(string symbol);
    CompanyPriceHistory getCompanyLastPrice(string symbol);
    bool updateCompanyInfo(CompanyInfo companyInfo);
    bool insertCompanyPriceHistory(IEnumerable<CompanyPriceHistory> companyPriceHistory);
    int insertCompanyInfo(CompanyInfo companyPriceHistory);
  }
}
