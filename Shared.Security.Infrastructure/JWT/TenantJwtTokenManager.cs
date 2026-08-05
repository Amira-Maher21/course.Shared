using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NDS.Shared.Security.Infrastructure.Multitenant.CommandRepositories.TenantIdentity;
using Shared.Application.JWT;
using Shared.Domain.SystemConstants;
using Shared.Kernel.BaseReturnTypes;
using Shared.Security.Application.Contracts.CommandRepositories;
using Shared.Security.Application.Contracts.CommandRepositories.Multitenant;
using Shared.Security.Application.Contracts.DTO.JWT;
using Shared.Security.Application.Contracts.DTO.Multitenant.Identity;
using Shared.Security.Application.Contracts.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Security.Infrastructure.JWT
{
    public class TenantJwtTokenManager : ITenantJwtTokenManager
    {
        private readonly TenantUserManager _userManager;
        private readonly JwtOptions _jwtOptions;

        public TenantJwtTokenManager(ISecurityUnitOfWork commandManager, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = commandManager.TenantUsers;
            _jwtOptions = jwtOptions.Value;
        }
        public async Task<ReturnBase<CreateTokenResultDto>> CreateTokenAsync(TenantLoginDto login)
        {
            var validateUserResult = await _userManager.ValidateUserAsync(login);
            if (validateUserResult.Succeeded)
            {
                var generateTokenResult = await generateToken(validateUserResult.Result);
                if (generateTokenResult.Succeeded)
                {
                    return ReturnBase<CreateTokenResultDto>.Success(generateTokenResult.Result);
                }
                return ReturnBase<CreateTokenResultDto>.Fail(generateTokenResult.Errors);

            }

            return ReturnBase<CreateTokenResultDto>.Fail(validateUserResult.Errors);
        }




        private async Task<ReturnBase<CreateTokenResultDto>> generateToken(TenantUserDto user)
        {
            var signingCredentials = getSigningCredentials();
            var claims = await getClaims(user);
            var tokenOptions = generateTokenOptions(signingCredentials, claims);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            var refreshToken = GenerateRefreshToken();

            var tokenResult = new CreateTokenResultDto()
            {
                UserId = user.UserId,
                Token = accessToken,
                RefreshToken = refreshToken,
                TokenExpires = tokenOptions.ValidTo,
                RefreshTokenExpires = DateTime.Now.AddDays(_jwtOptions.RefreshTokenExpiresIn)
            };
            return ReturnBase<CreateTokenResultDto>.Success(tokenResult);

        }

        private SigningCredentials getSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(JwtSecretKey.Key);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> getClaims(TenantUserDto user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.TenantUserName),
        new Claim(InspectionUserClaims.TenantUserName, user.TenantUserName),
        new Claim(InspectionUserClaims.TenantName, user.TenantName)
    };

            if (user.ERPUserCodeId.HasValue)
            {
                claims.Add(new Claim(
                    InspectionUserClaims.ERPUserCodeId,
                    user.ERPUserCodeId.Value.ToString()));
            }

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles.Result)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken generateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {

            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtOptions.ValidIssuer,
                audience: _jwtOptions.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtOptions.TokenExpiresIn)),
                signingCredentials: signingCredentials);
            return tokenOptions;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            var randomGenerator = RandomNumberGenerator.Create();
            randomGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);


        }
    }
}
