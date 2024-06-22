namespace Ayerhs.Core.Entities.Utility
{
    /// <summary>
    /// This class provides a collection of custom error codes used throughout the application.
    /// </summary>
    public static class CustomErrorCodes
    {
        #region Error Codes for Account Management from `ERR-1000-001` to `ERR-1000-999`

        /// <summary>
        /// Error code indicating a validation error occurred.
        /// </summary>
        public const string ValidationError = "ERR-1000-001";

        /// <summary>
        /// Error code indicating a database error occurred.
        /// </summary>
        public const string DatabaseError = "ERR-1000-002";

        /// <summary>
        /// Error code indicating an unauthorized access attempt.
        /// </summary>
        public const string UnauthorizedError = "ERR-1000-003";

        /// <summary>
        /// Error code indicating a requested resource was not found.
        /// </summary>
        public const string NotFoundError = "ERR-1000-004";

        /// <summary>
        /// Error code for Invalid Credentials
        /// </summary>
        public const string InvalidCredentials = "ERR-1000-005";

        /// <summary>
        /// Error code for Client Account is locked.
        /// </summary>
        public const string AccountLocked = "ERR-1000-006";

        /// <summary>
        /// Error code for Null Registered Clients
        /// </summary>
        public const string NullRegisteredClients = "ERR-1000-007";


        /// <summary>
        /// Error code indicating an unknown error occurred.
        /// </summary>
        public const string UnknownError = "ERR-1000-999"; 

        #endregion
    }
}
