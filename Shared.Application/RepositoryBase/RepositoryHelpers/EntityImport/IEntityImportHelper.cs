using  Shared.Kernel.BaseReturnTypes;

namespace NDS.Shared.Application.RepositoryBase.RepositoryHelpers.EntityImport
{
    public interface IEntityImportHelper
    {
        Task<ReturnBase> Import(string tableId, string formId, Dictionary<string, object> parameters);
    }

}
