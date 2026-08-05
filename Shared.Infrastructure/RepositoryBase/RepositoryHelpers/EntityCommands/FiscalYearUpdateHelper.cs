using  Shared.Application.API;
using  Shared.Domain.Contracts.EntityCommonData;

namespace Shared.Infrastructure.RepositoryBase.RepositoryHelpers.EntityCommands
{
    internal class FiscalYearUpdateHelper
    {
        public void UpdateFiscalYear(object entity, CommonUserData userData)
        {
            if (entity is IFiscalYear)
            {
                ((IFiscalYear)entity).Fiscal_Year = userData.FiscalYear;
            }
        }
    }
}
