using Ayerhs.Core.Entities.AccountManagement;

namespace Ayerhs.Core.Interfaces.AccountManagement
{
    /// <summary>
    /// This interface defines methods for account management services.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Asynchronously registers a new Client using the provided data.
        /// </summary>
        /// <param name="inRegisterClientDto">A DTO containing data for client registration.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task RegisterClientAsync(InRegisterClientDto inRegisterClientDto);

        /// <summary>
        /// Asynchronously attempts to login a client using the provided credentials.
        /// </summary>
        /// <param name="inLoginClientDto">A DTO containing login credentials.</param>
        /// <returns>A Task that resolves to a Clients object containing user information on success, or null on failure.</returns>
        Task<Clients?> LoginClientAsync(InLoginClientDto inLoginClientDto);

        /// <summary>
        /// Asynchronously getting registered client list.
        /// </summary>
        /// <returns>A Task represents list of Clients entity.</returns>
        Task<List<Clients>?> GetClientsAsync();

        /// <summary>
        /// Asynchronously generate a random OTP and send on email
        /// </summary>
        /// <param name="inOtpRequestDto">A DTO containing email related data.</param>
        /// <returns>A Task that contains a message.</returns>
        Task<(bool, string)> OtpGenerationAndEmailAsync(InOtpRequestDto inOtpRequestDto);

        /// <summary>
        /// Performs verification of a one-time verification code (OTP).
        /// </summary>
        /// <param name="inOtpVerificationDto">The data transfer object containing the email and OTP.</param>
        /// <returns>A tuple containing a flag indicating success and an optional error message.</returns>
        Task<(bool, string)> OtpVerificationAsync(InOtpVerificationDto inOtpVerificationDto);

        /// <summary>
        /// Attempts to reset a client's password based on the provided information.
        /// </summary>
        /// <param name="inForgotClientPassword">An object containing the client's email and potentially their new password.</param>
        /// <returns>A tuple indicating success (bool) and an optional message (string) related to the operation.</returns>
        Task<(bool, string)> ForgotClientPasswordAsync(InForgotClientPassword inForgotClientPassword);
    }
}
