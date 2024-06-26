using Ayerhs.Core.Entities.AccountManagement;
using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.UserManagement
{
    /// <summary>
    /// This class represents a user entity in the system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user. (Primary Key)
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// User identifier (used for login purposes).
        /// </summary>
        [Required]
        public string? UserId { get; set; }

        /// <summary>
        /// User's display name.
        /// </summary>
        [Required]
        [MinLength(3)]
        public string? UserName { get; set; }

        /// <summary>
        /// User's email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string? UserEmail { get; set; }

        /// <summary>
        /// User's password (hashed).
        /// </summary>
        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^\da-zA-Z])(?=.{8,})$", ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one number and one special character, and be at least 8 characters long")]
        public string? UserPassword { get; set; }

        /// <summary>
        /// User's mobile phone number.
        /// </summary>
        [Required]
        public string? UserMobileNumber { get; set; }

        /// <summary>
        /// User partition identifier (specific meaning depends on your application logic).
        /// </summary>
        [Required]
        public string? Partition { get; set; }

        /// <summary>
        /// User group identifier (specific meaning depends on your application logic).
        /// </summary>
        [Required]
        public string? Group { get; set; }

        /// <summary>
        /// Foreign key for the user's role.
        /// </summary>
        [Required]
        public int UserRoleId { get; set; }

        /// <summary>
        /// Flag indicating whether the user account is active.
        /// </summary>
        [Required]
        public bool? UserIsActive { get; set; }

        /// <summary>
        /// Flag indicating whether the user account is locked.
        /// </summary>
        [Required]
        public bool? UserIsLocked { get; set; }

        /// <summary>
        /// Enumeration value representing the user's current status (Active, Inactive, Suspended).
        /// </summary>
        [Required]
        public UserStatus? UserStatus { get; set; }

        /// <summary>
        /// Number of login attempts made by the user.
        /// </summary>
        [Required]
        public int UserLoginAttemptCount { get; set; }

        /// <summary>
        /// Enumeration value representing the user's deletion state (NotDeleted, SoftDeleted, HardDeleted).
        /// </summary>
        [Required]
        public UserDeleteState? UserDeletedState { get; set; }

        /// <summary>
        /// Date and time the user record was created.
        /// </summary>
        [Required]
        public DateTime? UserCreatedOn { get; set; }

        /// <summary>
        /// Date and time the user record was last updated.
        /// </summary>
        [Required]
        public DateTime? UserUpdatedOn { get; set; }

        /// <summary>
        /// Date and time of the user's last login.
        /// </summary>
        public DateTime? UserLastLoginOn { get; set; }

        /// <summary>
        /// Date and time until which the user account is locked.
        /// </summary>
        public DateTime? UserLockedUntil { get; set; }

        /// <summary>
        /// Date and time the user record was soft deleted.
        /// </summary>
        public DateTime? UserDeletedOn { get; set; }

        /// <summary>
        /// Date and time the user record was auto-deleted (nullable).
        /// </summary>
        public DateTime? UserAutoDeletedOn { get; set; }

        /// <summary>
        /// Random string value used for password hashing.
        /// </summary>
        public string? Salt { get; set; }

        /// <summary>
        /// Navigation property for the user's client roles (collection).
        /// </summary>
        public ICollection<ClientRoles>? ClientRoles { get; set; }
    }

    /// <summary>
    /// Enumeration representing the different user account status options.
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// User account is active.
        /// </summary>
        Active = 1,

        /// <summary>
        /// User account is inactive.
        /// </summary>
        Inactive = 2,

        /// <summary>
        /// User account is suspended.
        /// </summary>
        Suspended = 3
    }

    /// <summary>
    /// Enumeration representing the different user deletion state options.
    /// </summary>
    public enum UserDeleteState
    {
        /// <summary>
        /// User record is not deleted.
        /// </summary>
        NotDeleted = 0,

        /// <summary>
        /// User record is in deleted state.
        /// </summary>
        SoftDeleted = 1,

        /// <summary>
        /// User record is deleted.
        /// </summary>
        HardDeleted = 2
    }
}
