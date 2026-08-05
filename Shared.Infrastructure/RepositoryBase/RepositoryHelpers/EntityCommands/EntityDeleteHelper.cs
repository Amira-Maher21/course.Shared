using Microsoft.EntityFrameworkCore;
using Shared.Application.RepositoryBase.RepositoryHelpers;
using Shared.Kernel.BaseReturnTypes;
using Shared.Kernel.Exceptions;

namespace NDS.Shared.Infrastructure.RepositoryBase.RepositoryHelpers.EntityCommands
{
    internal class EntityDeleteHelper<TEntity> : IEntityDeleteHelper<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        private readonly IExceptionManager _exceptionManager;

        public EntityDeleteHelper(DbContext context, IExceptionManager exceptionManager)
        {
            _context = context;
            _exceptionManager = exceptionManager;


        }

        public ReturnBase<TEntity> DeleteEntity(TEntity entity, EntityStructure entityStructure)
        {
            try
            {
                _context.Set<TEntity>().Remove(entity);
                return ReturnBase<TEntity>.Success(entity);
            }
            catch (Exception ex)
            {
                return ReturnBase<TEntity>.Fail(ex, _exceptionManager);

            }
        }
    }
}
