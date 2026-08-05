namespace Shared.Application.RepositoryBase.RepositoryHelpers
{
    public interface IApprovable
    {
        Task<bool> Approve(TableFormKeysValues formInfo);
    }

    public interface IEntityApproveHelper : IApprovable
    {
    }

    public class TableFormKeysValues
    {
        public string? FormId { get; set; }
        public string? TableName { get; set; }

        public List<string>? Keys { get; set; }
        public List<string>? Values { get; set; }

        public string? Language { get; set; }


    }
}
