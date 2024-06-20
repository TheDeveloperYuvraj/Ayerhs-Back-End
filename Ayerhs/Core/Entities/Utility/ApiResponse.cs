namespace Ayerhs.Core.Entities.Utility
{
    /// <summary>
    /// Represents an API response containing status information and data.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the API.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Gets or sets the status of the API response. (e.g., Success, Failure)
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code of the API response.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the response of the API response.
        /// </summary>
        public int Response { get; set; }

        /// <summary>
        /// Gets or sets a success message associated with the API response.
        /// </summary>
        public string? SuccessMessage { get; set; }

        /// <summary>
        /// Gets or sets an error message associated with the API response.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets an error code associated with the API response.
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets a transaction number associated with the API response.
        /// </summary>
        public string? Txn { get; set; }

        /// <summary>
        /// Gets or sets the data returned by the API.
        /// </summary>
        public T? ReturnValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class.
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class with the specified properties.
        /// </summary>
        /// <param name="status">The status of the API response.</param>
        /// <param name="statusCode">The HTTP status code of the API response.</param>
        /// <param name="response">The response code of the API response.</param>
        /// <param name="successMessage">A success message associated with the API response (optional).</param>
        /// <param name="errorMessage">An error message associated with the API response (optional).</param>
        /// <param name="errorCode">An error code associated with the API response (optional).</param>
        /// <param name="txn">An txn associated with the API response (optional).</param>
        /// <param name="returnValue">The data returned by the API (optional, default is the default value of T).</param>
        public ApiResponse(string status, int statusCode, int response, string? successMessage = null, string? errorMessage = null, string? errorCode = null, string? txn = null, T? returnValue = default)
        {
            Status = status;
            StatusCode = statusCode;
            Response = response;
            SuccessMessage = successMessage;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            Txn = txn;
            ReturnValue = returnValue;
        }
    }
}
