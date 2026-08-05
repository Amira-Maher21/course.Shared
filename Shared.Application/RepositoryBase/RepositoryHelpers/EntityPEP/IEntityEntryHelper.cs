using  Shared.Application.SharedModels;
using Shared.Kernel.BaseReturnTypes;

namespace Shared.Application.RepositoryBase.RepositoryHelpers.EntityPEP
{
    public interface IEntryCommand
    {

        Task<ReturnBase<bool>> Entry(EntityKeyValueDictionary keys);
        Task<ReturnBase<bool>> UnEntry(EntityKeyValueDictionary keys);
    }
    public interface IEntityEntryHelper : IEntryCommand
    {

    }


}
