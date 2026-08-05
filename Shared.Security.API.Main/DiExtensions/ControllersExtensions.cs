using Shared.Security.API.System;
using Shared.Security.API.Tenant;

namespace Shared.Security.API.Main.DiExtensions
{
   
        public static class ControllersExtensions
        {
            public static IMvcBuilder AddControllersExtensions(this IServiceCollection services)
            {
                return services
                    .AddControllers()
                    .AddApplicationPart(typeof(SystemApiControllersAssemblyReference).Assembly)
                    .AddApplicationPart(typeof(TenantApiControllersAssemblyReference).Assembly);
            }
        }
    
}

