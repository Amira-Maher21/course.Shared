namespace  Shared.Application.API
{
    public class RequestBase<TRequestData>
    {
        public string? UserId { get; }

        public string? FiscalYear { get; }

        public string? Language { get; }
        public TRequestData? RequestData { get; }
    }
}
