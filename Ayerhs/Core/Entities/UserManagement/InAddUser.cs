namespace Ayerhs.Core.Entities.UserManagement
{
    /// <summary>
    /// This class represents the data model for adding a new user.
    /// </summary>
    public class InAddUser
    {
        /// <summary>
        /// User's display name.
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// User's email address.
        /// </summary>
        public string? UserEmail { get; set; }
        /// <summary>
        /// User's password (plain text - will be hashed on server-side).
        /// </summary>
        public string? UserPassword { get; set; }
        /// <summary>
        /// User's mobile phone number.
        /// </summary>
        public string? UserMobileNumber { get; set; }
        /// <summary>
        /// Foreign key for the user's role identifier.
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// User partition identifier (specific meaning depends on your application logic).
        /// </summary>
        public string? UserPartition { get; set; }
        /// <summary>
        /// User group identifier (specific meaning depends on your application logic).
        /// </summary>
        public string? UserGroup { get; set; }
    }
}
