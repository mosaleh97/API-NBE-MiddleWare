using System.Net.Mail;
using System.Net;

namespace API_NBE_MiddleWare.MailSolutions
{
    public class SMTP
    {

        public Tuple<bool,string> Send()
        {

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

            string smtpServer = configuration["SmtpSettings:Server"];
            int smtpPort = int.Parse(configuration["SmtpSettings:Port"]);
            string smtpUsername = configuration["SmtpSettings:Username"];
            string smtpPassword = configuration["SmtpSettings:Password"];
            bool enableSsl = bool.Parse(configuration["SmtpSettings:EnableSsl"]);

            string senderEmail = configuration["SmtpSettings:SenderEmail"];
            string recipientEmail = configuration["SmtpSettings:RecipientEmail"];
            string subject = configuration["SmtpSettings:Subject"];
            string body = configuration["SmtpSettings:Body"];

            string attachmentPath = configuration["SmtpSettings:AttachmentPath"];

            // Create and configure SMTP client
            using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = enableSsl;

                // Compose the email message
                MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail, subject, body);

                // Add attachment if provided
                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    Attachment attachment = new Attachment(attachmentPath);
                    mailMessage.Attachments.Add(attachment);
                }

                // Send the email
                try
                {
                    client.Send(mailMessage);
                    return new Tuple<bool, string>(true, "Email sent successfully.");
                }
                catch (Exception ex)
                {
                    return new Tuple<bool, string>(false, $"Failed to send email: {ex.Message}");
                }
            }
        }
        
    }
}
