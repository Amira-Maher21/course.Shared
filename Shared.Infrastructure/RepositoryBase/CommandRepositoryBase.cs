using Microsoft.EntityFrameworkCore;
using  Shared.Application.API;
using  Shared.Application.Multitenant;
using  Shared.Application.RepositoryBase;
using  Shared.Application.RepositoryBase.RepositoryHelpers;
using  Shared.Application.SharedModels;
using  Shared.Domain.Contracts;
using  Shared.Infrastructure.RepositoryBase.RepositoryHelpers;
using  Shared.Infrastructure.RepositoryBase.RepositoryHelpers.EntityCommands;
using  Shared.Kernel.BaseReturnTypes;
using  Shared.Kernel.Exceptions;
using  Shared.Kernel.Extensions;

namespace  Shared.Infrastructure.RepositoryBase
{
    public abstract class CommandRepositoryBase<TEntity> : ICommandRepository<TEntity> where TEntity : class, IRootEntity
    {
        protected readonly DbContext _context;
        private readonly IEntityKeyHelper _keyHelper;
        protected readonly DbSet<TEntity> _dbSet;

        protected readonly IExceptionManager _exceptionManager;
        protected readonly ITenantResolver _tenantResolver;
        protected readonly CommonUserData _userData;

        protected readonly IEntityInsertHelper<TEntity> _entityInsertHelper;
        protected readonly IEntityUpdateHelper<TEntity> _entityUpdateHelper;
        protected readonly IEntityDeleteHelper<TEntity> _entityDeleteHelper;


        protected EntityStructure? _entityStructure;
        public CommandRepositoryBase(DbContext context,
                                     ITenantResolver tenantResolver,
                                     IExceptionManager exceptionManager)
        {
            _context = context;
            _keyHelper = new EntityKeyHelper();
            _exceptionManager = exceptionManager;
            _tenantResolver = tenantResolver;
            _dbSet = context.Set<TEntity>();
            _userData = _tenantResolver.GetCommonUserData();


            _entityInsertHelper = new EntityInsertHelper<TEntity>(_tenantResolver, _exceptionManager);
            _entityUpdateHelper = new EntityUpdateHelper<TEntity>(_context, _keyHelper, _tenantResolver, _exceptionManager);
            _entityDeleteHelper = new EntityDeleteHelper<TEntity>(_context, _exceptionManager);

        }

        public virtual async Task<ReturnBase<TEntity>> GetEntityAsync<TId>(TId id)
        {
            if (_entityStructure is null) throw new Exception("Entity Structure must be defined!!.");
            if (_entityStructure.Key is null || _entityStructure.Key.Length != 1)
                throw new Exception("Entity Key must have one field");
            EntityKeyValueDictionary keys = new EntityKeyValueDictionary()

           { new KeyValuePair<string, object>(_entityStructure.Key.First(), id!)};

            return await GetEntityAsync(keys);
        }

        public virtual async Task<ReturnBase<TEntity>> GetEntityAsync(EntityKeyValueDictionary keys)
        {
            try
            {
                var query = _dbSet.AsQueryable();
                if (_entityStructure is null) throw new Exception("Entity Structure must be defined!!.");
                if (_entityStructure.RelatedEntities.Any())
                {
                    foreach (var relatedEntity in _entityStructure.RelatedEntities)
                    {
                        if (!string.IsNullOrEmpty(relatedEntity.NavigationProperty))
                        {
                            query = query.Include(relatedEntity.NavigationProperty);
                        }
                    }
                }

                query = query.GetByKeys(keys);
                var findResult = await query.ToListAsync();
                if (findResult is not null && findResult.Any())
                    return ReturnBase<TEntity>.Success(findResult.First());
                return ReturnBase<TEntity>.Success(null);
            }
            catch (Exception ex)
            {
                return ReturnBase<TEntity>.Fail(ex.InnerException, _exceptionManager);
            }

        }

        public virtual async Task<ReturnBase<TEntity>> InsertAsync(TEntity entity)
        {
            try
            {
                if (_entityStructure is null) throw new Exception("Entity Structure must be defined!!.");

                var insertResult = _entityInsertHelper.InsertEntityAsync(entity, _entityStructure);
                if (insertResult.Succeeded)
                {
                    await _dbSet.AddAsync(entity);
                    return ReturnBase<TEntity>.Success(entity);

                }
                return insertResult;

            }
            catch (Exception ex)
            {
                return ReturnBase<TEntity>.Fail(ex.InnerException, _exceptionManager);

            }
        }
        public virtual async Task<ReturnBase<TEntity>> UpdateAsync(TEntity updatedEntity)
        {
            try
            {
                if (_entityStructure is null ||
                    _entityStructure.Key is null ||
                    !_entityStructure.Key.Any())
                    throw new Exception("Entity Structure must be defined!!.");




                var keys = _keyHelper.GetKeyPairs(updatedEntity, _entityStructure.Key);

                var getEntityResult = await this.GetEntityAsync(keys);
                if (getEntityResult.Succeeded && getEntityResult.Result is not null)
                {
                    TEntity originalEntity = getEntityResult.Result;

                    return _entityUpdateHelper.Update(updatedEntity, originalEntity, _entityStructure);
                }

                return getEntityResult;
            }
            catch (Exception ex)
            {
                return ReturnBase<TEntity>.Fail(ex.InnerException, _exceptionManager);
            }
        }


        public virtual async Task<ReturnBase<TEntity>> DeleteAsync<TId>(TId id)
        {
            if (_entityStructure is null) throw new Exception("Entity Structure must be defined!!.");
            if (_entityStructure.Key is null || _entityStructure.Key.Length != 1)
                throw new Exception("Entity Key must have one field");
            EntityKeyValueDictionary keys = new EntityKeyValueDictionary()

           { new KeyValuePair<string, object>(_entityStructure.Key.First(), id!)};
            return await this.DeleteAsync(keys);
        }

        public virtual async Task<ReturnBase<TEntity>> DeleteAsync(EntityKeyValueDictionary keys)
        {
            try
            {
                if (_entityStructure is null ||
                    _entityStructure.Key is null ||
                    !_entityStructure.Key.Any())
                    throw new Exception("Entity Structure must be defined!!.");


                var getEntityResult = await GetEntityAsync(keys);
                if (getEntityResult.Succeeded && getEntityResult.Result is not null)
                {
                    TEntity entity = getEntityResult.Result;

                    return _entityDeleteHelper.DeleteEntity(entity, _entityStructure!);
                }

                return getEntityResult;
            }
            catch (Exception ex)
            {
                return ReturnBase<TEntity>.Fail(ex.InnerException, _exceptionManager);

            }
        }

        public virtual async Task<ReturnBase<IEnumerable<ReturnBase<TEntity>>>> BulkDeleteAsync(IEnumerable<EntityKeyValueDictionary> keysList, Func<Task<ReturnBase<int>>> saveFunc)
        {
            try
            {
                if (_entityStructure is null ||
                    _entityStructure.Key is null ||
                    !_entityStructure.Key.Any())
                    throw new Exception("Entity Structure must be defined!!.");

                var deletedResults = new List<ReturnBase<TEntity>>();

                foreach (var key in keysList)
                {
                    var deletedResult = await DeleteAsync(key);

                    if (!deletedResult.Succeeded)
                    {
                        deletedResults.Add(ReturnBase<TEntity>.Fail(deletedResult.Errors));
                        continue;
                    }

                    var saveResult = await saveFunc();

                    if (!saveResult.Succeeded)
                    {
                        deletedResults.Add(ReturnBase<TEntity>.Fail(saveResult.Errors));
                        _context.Entry(deletedResult.Result).State = EntityState.Detached;
                        continue;
                    }

                    deletedResults.Add(ReturnBase<TEntity>.Success(deletedResult.Result));
                }

                return ReturnBase<IEnumerable<ReturnBase<TEntity>>>.Success(deletedResults);
            }
            catch (Exception ex)
            {
                return ReturnBase<IEnumerable<ReturnBase<TEntity>>>.Fail(ex.InnerException, _exceptionManager);
            }
        }

        //public virtual async Task<ReturnBase<string>> AutoIncrementAsync(string tableName, string code, long? groupValue = null, string group = "")
        //{
        //    try
        //    {
        //        using var connection = new SqlConnection("Data Source=91.229.118.79;Initial Catalog=db_ECIT;Persist Security Info=True;User ID=sa;Password=50%admin;Trust Server Certificate=True");
        //        await connection.OpenAsync();

        //        string query;
        //        Dictionary<string, object> parameters;

        //        if (string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(code))
        //            return ReturnBase<string>.Fail();

        //        tableName = tableName.Replace("'", "''");
        //        code = code.Replace("'", "''");
        //        group = group.Replace("'", "''");

        //        if (!string.IsNullOrEmpty(group) && groupValue is not null)
        //        {
        //            parameters = new()
        //            {
        //                { "@GroupValue", groupValue }
        //            };

        //            query = $@"SELECT ISNULL(MAX({code}), 0) FROM {tableName} WHERE {group} = @GroupValue";
        //        }
        //        else
        //        {
        //            parameters = [];
        //            query = $@"SELECT ISNULL(MAX({code}), 0) FROM {tableName}";
        //        }

        //        var maxCode = await connection.ExecuteScalarAsync<string>(query, parameters);

        //        if (string.IsNullOrWhiteSpace(maxCode))
        //            return ReturnBase<string>.Fail();


        //        await connection.CloseAsync();
        //        await connection.DisposeAsync();

        //        string res = "";
        //        var lenMaxNode = maxCode.Substring(groupValue.ToString().Length).Length;
        //        var NextNode = long.Parse(maxCode.Substring(groupValue.ToString().Length)) + 1;
        //        var lenDiff = lenMaxNode - NextNode.ToString().Length;
        //        for (int i = 1; i <= lenDiff; i++)
        //        {
        //            res += "0";
        //        }
        //        res += NextNode.ToString();

        //        return ReturnBase<string>.Success(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ReturnBase<string>.Fail(ex.InnerException, _exceptionManager);
        //    }
        //}
    }
}
