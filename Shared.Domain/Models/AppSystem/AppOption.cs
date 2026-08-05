using Shared.Domain.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.Models.AppSystem
{
    public class AppOption : IRootEntity
    {
        [Key]
        public string? OptionKey { get; set; }
        public string? OptionValue { get; set; }
        public string? Description { get; set; }

    }
}
