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
        /// Error code for OTP generation.
        /// </summary>
        public const string OtpGeneration = "ERR-1000-008";

        /// <summary>
        /// Error code for OTP Verification.
        /// </summary>
        public const string OtpVerification = "ERR-1000-008";

        /// <summary>
        /// Error code for Account Activation
        /// </summary>
        public const string AccountActivation = "ERR-1000-009";

        /// <summary>
        /// Error code for Forgot Password
        /// </summary>
        public const string ForgotClientPassword = "ERR-1000-010";


        /// <summary>
        /// Error code indicating an unknown error occurred.
        /// </summary>
        public const string UnknownError = "ERR-1000-999";

        #endregion

        #region Error Codes for User Management from `ERR-3000-001` to `ERR-3000-999`

        /// <summary>
        /// Error code indicating a validation error occurred while User Management
        /// </summary>
        public const string UserManagementValidationError = "ERR-3000-001";

        /// <summary>
        /// Error code indicating error occurred while adding partition.
        /// </summary>
        public const string AddPartitionError = "ERR-3000-002";

        /// <summary>
        /// Error code indicating error occurred while getting partitions list.
        /// </summary>
        public const string GetPartitionsError = "ERR-3000-003";

        /// <summary>
        /// Error code indicating error occurred while updating partition
        /// </summary>
        public const string UpdatePartitionError = "ERR-3000-004";

        /// <summary>
        /// Error Code indicating error occurred while deleting partition due to negative ID provided.
        /// </summary>
        public const string DeletePartitionNegativeIdError = "ERR-3000-005";

        /// <summary>
        /// Error code indicating error occurred while deleting partition.
        /// </summary>
        public const string DeletePartitionError = "ERR-3000-006";

        /// <summary>
        /// Error code indicating error occurred while adding group.
        /// </summary>
        public const string AddGroupError = "ERR-3000-007";

        /// <summary>
        /// Error code indicating error occurred while getting group.
        /// </summary>
        public const string GetGroupsError = "ERR-3000-008";

        /// <summary>
        /// Error code indicating error occurred while updating group.
        /// </summary>
        public const string UpdateGroupError = "ERR-3000-009";


        /// <summary>
        /// Error code for Unknow Error occurred while User Management
        /// </summary>
        public const string UserManagementUnknownError = "ERR-3000-999";

        #endregion
    }
}
