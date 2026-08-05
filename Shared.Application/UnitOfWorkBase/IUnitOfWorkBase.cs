using  Shared.Kernel.BaseReturnTypes;

namespace  Shared.Application.UnitOfWorkBase
{
    public interface IUnitOfWorkBase
    {
        ReturnBase<int> Save();
        Task<ReturnBase<int>> SaveAsync();
    }
}
