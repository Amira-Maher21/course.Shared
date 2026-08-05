using  Shared.Application.SharedModels;
using  Shared.Kernel.BaseReturnTypes;

namespace  Shared.Application.RepositoryBase.RepositoryHelpers.EntityPEP
{
    public interface IPricingCommand
    {
        Task<ReturnBase<bool>> Price(EntityKeyValueDictionary keys);
        Task<ReturnBase<bool>> UnPrice(EntityKeyValueDictionary keys);
    }
    public interface IEntityPricingHelper : IPricingCommand
    {
    }
}
