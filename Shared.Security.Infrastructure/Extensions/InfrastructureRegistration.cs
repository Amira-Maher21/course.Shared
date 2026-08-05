
using Shared.Security.Infrastructure.JWT;
using Shared.Security.Infrastructure.Managers;
using Shared.Security.Infrastructure.Multitenant.DataContext;
using Shared.Security.Infrastructure.Multitenant.IdentityModels;
using Shared.Security.Infrastructure.System.DataContext;
using Shared.Security.Infrastructure.System.IdentityModels;

namespace Shared.Security.Infrastructure.Extensions
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection AddInspectionSecurityServices(this IServiceCollection services, IConfigurationManager configurationManager)
        {
            services.AddDbContext<TenantDbContext>();// (options =>
            //{
            //    options.UseSqlServer(configurationManager.GetConnectionString("SecurityDb"));
            //});

            services.AddDbContext<SystemDbContext>();// (options =>
                                                     //{
                                                     //    options.UseSqlServer(configurationManager.GetConnectionString("SecurityDb"));
                                                     //});

            services.AddScoped<ISecurityUnitOfWork, SecurityUoW>();
            services.AddScoped<ITenantJwtTokenManager, TenantJwtTokenManager>();

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;
                options.User.RequireUniqueEmail = false;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<TenantDbContext>();

            services.AddIdentityCore<SystemUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;
                options.User.RequireUniqueEmail = false;
            })
            .AddRoles<SystemRole>()
            .AddEntityFrameworkStores<SystemDbContext>();

            return services;

        }



    }


}
