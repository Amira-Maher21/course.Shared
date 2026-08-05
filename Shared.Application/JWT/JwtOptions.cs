namespace  Shared.Application.JWT
{
    public class JwtOptions
    {
        public static string JwtOptionsSection { get { return "JwtOptions"; } }
        public int TokenExpiresIn { get; set; }
        public int RefreshTokenExpiresIn { get; set; }
        public string? ValidIssuer { get; set; }
        public string? ValidAudience { get; set; }


        public SystemJwtOptions? SystemOptions { get; set; }
        public MultitenantJwtOptions? MultitenantOptions { get; set; }
    }
    public class SystemJwtOptions
    {
        public string? ValidAudience { get; set; }
        public int TokenExpiresIn { get; set; }
        public int RefreshTokenExpiresIn { get; set; }

    }

    public class MultitenantJwtOptions
    {
        public string? ValidAudience { get; set; }
        public int TokenExpiresIn { get; set; }
        public int RefreshTokenExpiresIn { get; set; }

    }


}
