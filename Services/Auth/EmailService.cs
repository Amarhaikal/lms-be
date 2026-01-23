using MailKit.Net.Smtp;
using MimeKit;

namespace LMS.Services.Auth
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            try
            {
                _logger.LogInformation("Attempting to send email to {Email} with subject: {Subject}", toEmail, subject);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["Email:FromName"],
                    _configuration["Email:FromAddress"]
                ));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["Email:SmtpHost"],
                    int.Parse(_configuration["Email:SmtpPort"]!),
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("✅ Email successfully sent to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send email to {Email}. Error: {ErrorMessage}", toEmail, ex.Message);
            }
        }

        public async Task SendAccountSuspendedEmailAsync(string toEmail, string username)
        {
            var subject = "Account Suspended - Security Alert";
            var body = $@"
                <h2>Account Suspended</h2>
                <p>Dear {username},</p>
                <p>Your account has been suspended due to multiple failed login attempts (5 attempts).</p>
                <p><strong>Please contact your administrator to reactivate your account.</strong></p>
                <p><strong>Security Tips:</strong></p>
                <ul>
                    <li>Never share your password with anyone</li>
                    <li>Use a strong, unique password</li>
                    <li>Contact administrator if you need password assistance</li>
                </ul>
                <p>If this wasn't you, please contact your administrator immediately.</p>
                <hr>
                <p style='color: #666; font-size: 12px;'>This is an automated message from Quantm Bank. Please do not reply to this email.</p>
            ";
            await SendAsync(toEmail, subject, body);
        }

        public async Task SendAccountSuspendedAdminAlertAsync(
            string adminEmail,
            string adminName,
            string suspendedUsername,
            string suspendedUserEmail,
            string ipAddress)
        {
            var subject = "⚠️ User Account Suspended - Admin Alert";
            var body = $@"
                <h2>Account Suspension Alert</h2>
                <p>Dear {adminName},</p>
                <p>A user account has been automatically suspended due to multiple failed login attempts.</p>

                <h3>Suspended User Details:</h3>
                <ul>
                    <li><strong>Username:</strong> {suspendedUsername}</li>
                    <li><strong>Email:</strong> {suspendedUserEmail}</li>
                    <li><strong>Reason:</strong> 5 consecutive failed login attempts</li>
                    <li><strong>IP Address:</strong> {ipAddress}</li>
                    <li><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss UTC}</li>
                </ul>

                <p><strong>Action Required:</strong></p>
                <ul>
                    <li>Verify the user's identity before reactivating the account</li>
                    <li>Investigate if this is a potential security breach</li>
                    <li>Consider resetting the user's password</li>
                </ul>

                <p>To reactivate the account, update the user's status to 'Active' in the admin panel.</p>
                <hr>
                <p style='color: #666; font-size: 12px;'>This is an automated security alert from Quantm Bank.</p>
            ";
            await SendAsync(adminEmail, subject, body);
        }

        public async Task SendRegisterSuccessEmailAsync(string toEmail, string username)
        {
            var subject = "Account Created - Quantm Bank";
            var body = $@"
                <h2>Welcome to Quantm Bank</h2>
                <p>Dear {username},</p>
                <p>An administrator has created an account for you at Quantm Bank.</p>
                <p>You can now log in to the system using the credentials provided to you.</p>
                <p>For security reasons, we recommend changing your password after your first login.</p>
                <hr>
                <p style='color: #666; font-size: 12px;'>This is an automated message from Quantm Bank. Please do not reply to this email.</p>
            ";
            await SendAsync(toEmail, subject, body);
        }

        public async Task SendNewLoginAlertAsync(
            string toEmail,
            string username,
            string ipAddress,
            string deviceType)
        {
            var subject = "New Login Detected";
            var body = $@"
                <h2>New Login Alert</h2>
                <p>Dear {username},</p>
                <p>We detected a login to your account from a new location:</p>
                <ul>
                    <li><strong>IP Address:</strong> {ipAddress}</li>
                    <li><strong>Device Type:</strong> {deviceType}</li>
                    <li><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss UTC}</li>
                </ul>
                <p>If this was you, no action is needed.</p>
                <p><strong>If this wasn't you, please contact your administrator immediately.</strong></p>
                <hr>
                <p style='color: #666; font-size: 12px;'>This is an automated security alert from Quantm Bank.</p>
            ";
            await SendAsync(toEmail, subject, body);
        }

        public async Task SendPasswordChangedByAdminEmailAsync(
            string toEmail,
            string username,
            string adminName)
        {
            var subject = "Password Changed by Administrator";
            var body = $@"
                <h2>Password Changed</h2>
                <p>Dear {username},</p>
                <p>Your password was successfully changed by administrator: <strong>{adminName}</strong></p>
                <p><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss UTC}</p>
                <p>If you didn't request this change, please contact your administrator immediately.</p>
                <hr>
                <p style='color: #666; font-size: 12px;'>This is an automated message from Quantm Bank.</p>
            ";
            await SendAsync(toEmail, subject, body);
        }

        public async Task SendRoleChangedEmailAsync(
            string toEmail,
            string username,
            string oldRole,
            string newRole,
            string adminName)
        {
            var subject = "Role Changed - Account Update";
            var body = $@"
                <h2>Role Changed</h2>
                <p>Dear {username},</p>
                <p>Your account role has been updated by administrator: <strong>{adminName}</strong></p>
                <ul>
                    <li><strong>Previous Role:</strong> {oldRole}</li>
                    <li><strong>New Role:</strong> {newRole}</li>
                    <li><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss UTC}</li>
                </ul>
                <p>If you have questions about this change, please contact your administrator.</p>
                <hr>
                <p style='color: #666; font-size: 12px;'>This is an automated message from Quantm Bank.</p>
            ";
            await SendAsync(toEmail, subject, body);
        }

    }
}