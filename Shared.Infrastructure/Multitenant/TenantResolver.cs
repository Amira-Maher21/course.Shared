using Microsoft.AspNetCore.Http;
using Shared.Domain.SystemConstants;
using Shared.Application.API;
using Shared.Application.Multitenant;
using Shared.Domain.Models.Multitenant;
using  Shared.Domain.SystemConstants;

namespace  Shared.Infrastructure.Multitenant
{
    public class TenantResolver : ITenantResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private CommonUserData _userData;
        public TenantResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Tenant GetTenant()
        {
            throw new NotImplementedException();
        }

        public string GetTenantName()
        {

            var claim = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(e => e.Type == InspectionUserClaims.TenantName);
            if (claim is null)
                throw new UnauthorizedAccessException("[Inspection Error]. Authentication failed.");

            return claim.Value;
        }

        public CommonUserData GetCommonUserData()
        {
            if (_userData != null) return _userData;
            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            var tenantClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(e => e.Type == InspectionUserClaims.TenantName);

            var eRPUserCodeIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(e => e.Type == InspectionUserClaims.ERPUserCodeId);

            if (username is null || tenantClaim is null)
                throw new UnauthorizedAccessException("[Inspection Error]. Authentication failed.");

            var headers = _httpContextAccessor.HttpContext.Request.Headers;

            var fiscalYear = headers.FirstOrDefault(h => h.Key == RequestHeaders.FiscalYear);
            var company = headers.FirstOrDefault(h => h.Key == RequestHeaders.Company);
            var language = headers.FirstOrDefault(h => h.Key == RequestHeaders.Language);

            long? erpUserCodeId = null;
            if (eRPUserCodeIdClaim != null && long.TryParse(eRPUserCodeIdClaim.Value, out var parsedId))
            {
                erpUserCodeId = parsedId;
            }

            return _userData = new CommonUserData
            {
                ERPUserCodeId = erpUserCodeId,
                TenantName = tenantClaim.Value,
                UserName = username,
                FiscalYear = fiscalYear.Value,
                Company = company.Value,
                UserLanguage = language.Value,
            };
        }

    }
}
