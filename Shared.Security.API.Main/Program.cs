
using Shared.Security.API.Main.DiExtensions;

namespace Shared.Security.API.Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);






            builder.Services.AddControllersExtensions();
            builder.Services.AddHttpContextAccessor();

            //builder.Services.Configure<JwtOptions>(
            //    builder.Configuration.GetSection(JwtOptions.JwtOptionsSection));

            //builder.Services.Configure<ERPSettings>(
            //    builder.Configuration.GetSection("ERPSettings"));

            //builder.Services.AddHttpClient<IERPUserCodeService, ERPUserCodeService>();

            //builder.Services.AddInspectionSecurityServices(builder.Configuration);
            //builder.Services.AddSharedInfrastructure();
            //builder.Services.AddSecurityServices();

            //builder.Services.ConfigureJWT(builder.Configuration);
            //builder.Services.AddAuthorization();

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAll", policy =>
            //    {
            //        policy.AllowAnyHeader()
            //              .AllowAnyMethod()
            //              .AllowAnyOrigin();
            //    });
            //});

            //builder.Services.AddSwaggerServicesExtensions();








            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.AddSwaggerApplicationExtensions();
            }

            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();


            //app.UseHttpsRedirection();

 

 
         }
    }
}
