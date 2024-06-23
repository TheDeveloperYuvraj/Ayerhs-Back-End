namespace Ayerhs.Core.Entities.Utility
{
    /// <summary>
    /// Provides utility methods for generating constant data.
    /// </summary>
    public static class ConstantData
    {
        /// <summary>
        /// Generates a unique transaction ID based on current UTC time.
        /// </summary>
        /// <returns>A string representing the transaction ID in the format "yyyyMMddHHmmssfff".</returns>
        public static string GenerateTransactionId()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        }

        /// <summary>
        /// Default email body to send OTP on email.
        /// </summary>
        /// <param name="otp">Dynamically generated OTP.</param>
        /// <returns>Return a HTML string for email body template</returns>
        public static string GetDefaultOtpHtmlBody(string otp)
        {
            return $@"
            <html>
            <head>
                <style>
                    .email-container {{
                        font-family: Arial, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        padding: 20px;
                    }}
                    .email-header {{
                        background-color: #007bff;
                        color: white;
                        padding: 10px;
                        text-align: center;
                        font-size: 24px;
                    }}
                    .email-body {{
                        margin: 20px 0;
                    }}
                    .otp-code {{
                        font-size: 20px;
                        font-weight: bold;
                        color: #007bff;
                    }}
                    .email-footer {{
                        margin-top: 20px;
                        font-size: 12px;
                        color: #777;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <div class='email-header'>
                        Your OTP Code
                    </div>
                    <div class='email-body'>
                        <p>Dear User,</p>
                        <p>Your OTP code is:</p>
                        <p class='otp-code'>{otp}</p>
                        <p>Please use this code to proceed with your request. This code is valid for the next 15 minutes.</p>
                    </div>
                    <div class='email-footer'>
                        <p>Thank you,</p>
                        <p>Ayerhs Team</p>
                    </div>
                </div>
            </body>
            </html>";
        }

        /// <summary>
        /// Minimalist and Clean email body.
        /// </summary>
        /// <param name="otp">Dynamically generated OTP.</param>
        /// <returns>Return a HTML string for email body template</returns>
        public static string GetMinimalistOtpHtmlBody(string otp)
        {
            return $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                margin: 0;
                                padding: 0;
                                background-color: #f4f4f4;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                background-color: #ffffff;
                                padding: 20px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                            }}
                            .header {{
                                background-color: #007bff;
                                color: white;
                                padding: 10px 0;
                                text-align: center;
                                font-size: 24px;
                            }}
                            .body {{
                                padding: 20px;
                                text-align: center;
                            }}
                            .otp {{
                                font-size: 36px;
                                font-weight: bold;
                                color: #007bff;
                                margin: 20px 0;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 12px;
                                color: #777;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>Your OTP Code</div>
                            <div class='body'>
                                <p>Dear User,</p>
                                <p>Your OTP code is:</p>
                                <p class='otp'>{otp}</p>
                                <p>Please use this code to proceed with your request. This code is valid for the next 15 minutes.</p>
                            </div>
                            <div class='footer'>
                                <p>Thank you,</p>
                                <p>Ayerhs Team</p>
                            </div>
                        </div>
                    </body>
                    </html>";
        }

        /// <summary>
        /// Professional and Modern email body.
        /// </summary>
        /// <param name="otp">Dynamically generated OTP.</param>
        /// <returns>Return a HTML string for email body template</returns>
        public static string GetProfessionalOtpHtmlBody(string otp)
        {
            return $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
                                background-color: #f8f9fa;
                                margin: 0;
                                padding: 0;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                background-color: #ffffff;
                                padding: 20px;
                                border: 1px solid #e9ecef;
                                border-radius: 5px;
                                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                            }}
                            .header {{
                                background-color: #28a745;
                                color: white;
                                padding: 15px;
                                text-align: center;
                                font-size: 20px;
                                border-radius: 5px 5px 0 0;
                            }}
                            .body {{
                                padding: 20px;
                                text-align: center;
                            }}
                            .otp {{
                                font-size: 32px;
                                font-weight: bold;
                                color: #28a745;
                                margin: 20px 0;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 14px;
                                color: #6c757d;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>Secure OTP Code</div>
                            <div class='body'>
                                <p>Hello,</p>
                                <p>Your OTP code is:</p>
                                <p class='otp'>{otp}</p>
                                <p>This code is valid for the next 15 minutes. Please use it promptly to complete your request.</p>
                            </div>
                            <div class='footer'>
                                <p>Best Regards,</p>
                                <p>The Ayerhs Team</p>
                            </div>
                        </div>
                    </body>
                    </html>";
        }

        /// <summary>
        /// Vibrant and Engaging email body.
        /// </summary>
        /// <param name="otp">Dynamically generated OTP.</param>
        /// <returns>Return a HTML string for email body template</returns>
        public static string GetVibrantOtpHtmlBody(string otp)
        {
            return $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: 'Verdana', Geneva, Tahoma, sans-serif;
                                background-color: #fff;
                                margin: 0;
                                padding: 0;
                                display: flex;
                                justify-content: center;
                                align-items: center;
                                height: 100vh;
                                background-color: #f0f0f0;
                            }}
                            .container {{
                                max-width: 600px;
                                background-color: #ffffff;
                                padding: 20px;
                                border-radius: 10px;
                                box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
                            }}
                            .header {{
                                background-color: #ff5733;
                                color: white;
                                padding: 10px;
                                text-align: center;
                                font-size: 22px;
                                border-radius: 10px 10px 0 0;
                            }}
                            .body {{
                                padding: 20px;
                                text-align: center;
                            }}
                            .otp {{
                                font-size: 28px;
                                font-weight: bold;
                                color: #ff5733;
                                margin: 15px 0;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 14px;
                                color: #999;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>OTP Verification</div>
                            <div class='body'>
                                <p>Hello,</p>
                                <p>Your One-Time Password (OTP) is:</p>
                                <p class='otp'>{otp}</p>
                                <p>This code is valid for 15 minutes. Please use it to complete your action.</p>
                            </div>
                            <div class='footer'>
                                <p>Thank you for choosing Ayerhs.</p>
                            </div>
                        </div>
                    </body>
                    </html>";
        }
    }
}
