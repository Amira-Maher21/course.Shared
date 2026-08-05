namespace Shared.Security.API.Main.DiExtensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerServicesExtensions(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;

        }

        public static IApplicationBuilder AddSwaggerApplicationExtensions(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            return app;

        }

    }
}
