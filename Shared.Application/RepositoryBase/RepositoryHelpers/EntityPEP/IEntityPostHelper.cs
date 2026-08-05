using  Shared.Application.SharedModels;
using  Shared.Kernel.BaseReturnTypes;

namespace  Shared.Application.RepositoryBase.RepositoryHelpers.EntityPEP
{
    public interface IPostCommand
    {
        Task<ReturnBase<bool>> Post(EntityKeyValueDictionary keys);
        Task<ReturnBase<bool>> UnPost(EntityKeyValueDictionary keys);
    }
    public interface IEntityPostHelper : IPostCommand
    {
    }
}
