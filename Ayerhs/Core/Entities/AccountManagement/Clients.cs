using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.AccountManagement
{
    public class Clients
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string? ClientId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ClientName { get; set; }

        [Required]
        [MaxLength(20)]
        public string? ClientUsername { get; set; }

        [Required]
        [EmailAddress]
        public string? ClientEmail { get; set; }

        [Required]
        [MaxLength(1024)]
        public string? ClientPassword { get; set; }

        [Required]
        [Phone]
        public string? ClientMobileNumber { get; set; }

        public bool? IsAdmin { get; set; }

        public bool? IsActive { get; set; }

        public ClientStatus? Status { get; set; }

        public DeletedState? DeletedState { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public DateTime? DeletedOn { get; set; }

        public DateTime? AutoDeletedOn { get; set; }

        public string? Salt { get; set; }

        public ICollection<ClientRoles>? ClientRoles { get; set; }
    }

    public enum ClientStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3
    }

    public enum DeletedState
    {
        NotDeleted = 0,
        SoftDeleted = 1,
        HardDeleted = 2
    }
}
