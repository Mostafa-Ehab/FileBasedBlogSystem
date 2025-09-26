namespace BlogSystem.Infrastructure.EmailService;

public static class VerifyEmailTemplate
{
    public static string GetContent(string username, string verificationCode)
    {
        return $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Email Verification</title>
            <style>
                body {{
                    margin: 0;
                    padding: 0;
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background-color: #f4f4f4;
                    line-height: 1.6;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                }}
                .header {{
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    padding: 30px;
                    text-align: center;
                }}
                .header h1 {{
                    margin: 0;
                    font-size: 28px;
                    font-weight: 300;
                }}
                .content {{
                    padding: 40px 30px;
                    text-align: center;
                }}
                .welcome-text {{
                    font-size: 24px;
                    color: #333;
                    margin-bottom: 10px;
                }}
                .subtitle {{
                    color: #666;
                    font-size: 16px;
                    margin-bottom: 30px;
                }}
                .verification-box {{
                    background-color: #f8f9ff;
                    border: 2px dashed #667eea;
                    border-radius: 10px;
                    padding: 30px;
                    margin: 30px 0;
                }}
                .verification-label {{
                    font-size: 14px;
                    color: #666;
                    text-transform: uppercase;
                    letter-spacing: 1px;
                    margin-bottom: 10px;
                }}
                .verification-code {{
                    font-size: 32px;
                    font-weight: bold;
                    color: #667eea;
                    letter-spacing: 4px;
                    font-family: 'Courier New', monospace;
                    margin: 10px 0;
                }}
                .instructions {{
                    color: #666;
                    font-size: 14px;
                    margin-top: 20px;
                }}
                .footer {{
                    background-color: #f8f9fa;
                    padding: 20px 30px;
                    text-align: center;
                    border-top: 1px solid #e9ecef;
                }}
                .footer p {{
                    margin: 0;
                    color: #666;
                    font-size: 12px;
                }}
                .warning {{
                    background-color: #fff3cd;
                    border: 1px solid #ffeaa7;
                    border-radius: 5px;
                    padding: 15px;
                    margin: 20px 0;
                    color: #856404;
                    font-size: 14px;
                }}
                @media (max-width: 600px) {{
                    .container {{
                        margin: 10px;
                    }}
                    .content {{
                        padding: 20px 15px;
                    }}
                    .verification-code {{
                        font-size: 24px;
                        letter-spacing: 2px;
                    }}
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>📝 Blog System</h1>
                </div>

                <div class='content'>
                    <div class='welcome-text'>Hello, {username}! 👋</div>
                    <div class='subtitle'>Welcome to our blogging community!</div>

                    <p>Thank you for registering with us. To complete your account setup and start sharing your amazing content, please verify your email address.</p>

                    <div class='verification-box'>
                        <div class='verification-label'>Your Verification Code</div>
                        <div class='verification-code'>{verificationCode}</div>
                        <div class='instructions'>
                            Enter this code in the verification form to activate your account
                        </div>
                    </div>

                    <div class='warning'>
                        ⚠️ This code will expire in 15 minutes for security purposes.
                    </div>

                    <p>Once verified, you'll be able to:</p>
                    <ul style='text-align: left; display: inline-block; color: #666;'>
                        <li>Create and publish blog posts</li>
                        <li>Interact with other bloggers</li>
                        <li>Customize your profile</li>
                        <li>Access premium features</li>
                    </ul>
                </div>

                <div class='footer'>
                    <p>If you didn't create an account with us, please ignore this email.</p>
                    <p>© 2024 Blog System. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";
    }
}
