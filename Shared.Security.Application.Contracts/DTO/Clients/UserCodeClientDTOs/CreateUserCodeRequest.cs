using Shared.Security.Application.Contracts.DTO.Clients.UserCodeClientDTOs;

namespace  Shared.Security.Application.Contracts.DTO.Clients.UserCodeClientDTOs
{
    public class CreateUserCodeRequest
    {
        public string User_ID { get; set; } = string.Empty;
        public string User_Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool System_Owner { get; set; }
        public bool System_Administrator { get; set; }
        public List<CreateUserCodeGroupRequest> User_Code_dGroups { get; set; } = new();
    }
}