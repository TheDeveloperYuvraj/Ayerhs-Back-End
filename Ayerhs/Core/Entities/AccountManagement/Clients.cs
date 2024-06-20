using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.AccountManagement
{
    /// <summary>
    /// This class represents a Client entity within the system.
    /// </summary>
    public class Clients
    {
        /// <summary>
        /// Unique identifier for the Client. (Primary Key)
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Client identifier (optional).
        /// </summary>
        public string? ClientId { get; set; }

        /// <summary>
        /// Name of the Client. (Required, Max Length 100)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? ClientName { get; set; }

        /// <summary>
        /// Username for the Client account. (Required, Max Length 20)
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string? ClientUsername { get; set; }

        /// <summary>
        /// Email address of the Client. (Required, Email format)
        /// </summary>
        [Required]
        [EmailAddress]
        public string? ClientEmail { get; set; }

        /// <summary>
        /// Client's password. (Required, Max Length 1024)
        /// </summary>
        [Required]
        [MaxLength(1024)]
        public string? ClientPassword { get; set; }

        /// <summary>
        /// Mobile phone number of the Client. (Required, Phone format)
        /// </summary>
        [Required]
        [Phone]
        public string? ClientMobileNumber { get; set; }

        /// <summary>
        /// Indicates whether the Client is an administrator.
        /// </summary>
        public bool? IsAdmin { get; set; }

        /// <summary>
        /// Indicates whether the Client account is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Status of the Client account (Active, Inactive, Suspended).
        /// </summary>
        public ClientStatus? Status { get; set; }

        /// <summary>
        /// Deleted state of the Client (Not Deleted, Soft Deleted, Hard Deleted).
        /// </summary>
        public DeletedState? DeletedState { get; set; }

        /// <summary>
        /// Date and time the Client record was created.
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Date and time the Client record was last updated.
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Date and time the Client record was soft deleted.
        /// </summary>
        public DateTime? DeletedOn { get; set; }

        /// <summary>
        /// Date and time the Client record was auto-deleted.
        /// </summary>
        public DateTime? AutoDeletedOn { get; set; }

        /// <summary>
        /// Salt value used for password hashing.
        /// </summary>
        public string? Salt { get; set; }

        /// <summary>
        /// Collection of client roles associated with the Client.
        /// </summary>
        public ICollection<ClientRoles>? ClientRoles { get; set; }
    }

    /// <summary>
    /// Enumeration representing the different Client account statuses.
    /// </summary>
    public enum ClientStatus
    {
        /// <summary>
        /// Client account is active.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Client account is inactive.
        /// </summary>
        Inactive = 2,

        /// <summary>
        /// Client account is suspended.
        /// </summary>
        Suspended = 3
    }

    /// <summary>
    /// Enumeration representing the different deleted states of a Client record.
    /// </summary>
    public enum DeletedState
    {
        /// <summary>
        /// Client record is not deleted.
        /// </summary>
        NotDeleted = 0,

        /// <summary>
        /// Client record is in deleted state.
        /// </summary>
        SoftDeleted = 1,

        /// <summary>
        /// Client record is deleted.
        /// </summary>
        HardDeleted = 2
    }
}
