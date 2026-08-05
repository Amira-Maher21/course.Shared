namespace  Shared.Security.Application.Contracts.DTO.Clients.UserCodeClientDTOs
{
    public class ERPUserOptions
    {
        public bool System_Owner { get; set; } = false;
        public bool System_Administrator { get; set; } = false;
        public List<CreateUserCodeGroupRequest>? User_Code_dGroups { get; set; }
    }
}