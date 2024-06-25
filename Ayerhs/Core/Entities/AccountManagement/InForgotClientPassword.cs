namespace Ayerhs.Core.Entities.AccountManagement
{
    /// <summary>
    /// This class represents the data used when a client forgets their password and requests a reset.
    /// </summary>
    public class InForgotClientPassword
    {
        /// <summary>
        /// The email address of the client who forgot their password.
        /// </summary>
        public string? ClientEmail { get; set; }

        /// <summary>
        /// The OTP received on gmail for forgot password
        /// </summary>
        public string? Otp { get; set; }

        /// <summary>
        /// The new password for the client (used after reset). 
        /// This property might be null if the request is only to send a reset email.
        /// </summary>
        public string? ClientPassword { get; set; }
    }
}
