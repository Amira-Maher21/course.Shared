using Microsoft.Extensions.DependencyInjection;
using  Shared.Infrastructure.Multitenant;
using Shared.Application.Multitenant;
using Shared.Infrastructure.DataContext;
using  Shared.Kernel.Exceptions;
using Shared.Infrastructure.DataQuery;
using Shared.Infrastructure.RepositoryBase.RepositoryHelpers;
using Shared.Infrastructure.Exceptions;
using Shared.Application.RepositoryBase.RepositoryHelpers;
using Shared.Application.DataQuery;

namespace Shared.Infrastructure.Extensions
{
   
        public static class SharedServiceInfrastructureRegistration
        {
            public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
            {
                services.AddScoped<ITenantResolver, TenantResolver>();
                services.AddSingleton<DapperDbContext>();
                services.AddSingleton<IDataQueryManager, DataQueryManager>();
                services.AddScoped<ISqlQueryBuilder, SqlQueryBuilder>();
                services.AddSingleton<IExceptionManager, ExceptionManager>();

                services.AddScoped<IEntityKeyHelper, EntityKeyHelper>();

                //services.AddScoped(typeof(IEntityInsertHelper<>), typeof(EntityInsertHelper<>));
                //services.AddScoped(typeof(IEntityUpdateHelper<>), typeof(EntityUpdateHelper<>));
                //services.AddScoped(typeof(IEntityDeleteHelper<>), typeof(EntityDeleteHelper<>));
                return services;
            }
        }
    
}
