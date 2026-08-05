namespace  Shared.Security.Application.Contracts.DTO.JWT
{
    public class LoginTokenResultDto
    {
        public string? Token { get; init; }
        public string? RefreshToken { get; init; }

        public DateTime TokenExpires { get; init; }
        public DateTime RefreshTokenExpires { get; init; }

    }
}
