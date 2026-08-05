using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NDS.Shared.Domain.SystemConstants;
using Shared.Application.JWT;
using System.Text;

namespace Shared.APIi.Extensions
{
  


    public static class SharedServicesApiRegistrations
    {
        public static AuthenticationBuilder ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {

            var jwtOptions = new JwtOptions();
            configuration.Bind(JwtOptions.JwtOptionsSection, jwtOptions);

            var secretKey = JwtSecretKey.Key;
            return services
                 .AddAuthentication(options =>
                 {
                     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


                 })
                 .AddJwtBearer(options =>
                 {
                     options.RequireHttpsMetadata = false;
                     options.SaveToken = true;
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = false,
                         ValidateAudience = false,
                         ValidateLifetime = false,
                         ValidateIssuerSigningKey = false,
                         ValidIssuer = jwtOptions.ValidIssuer,
                         ValidAudience = jwtOptions.ValidAudience,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                     };
                 });
        }

    }
}

