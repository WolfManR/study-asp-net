using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using TemplatesReporter.Mail.Core;

namespace TemplatesReporter.Mail.MailKit;

public class EmailService : IEmailService
{
    public void Send(EmailMessage emailMessage, EmailConfiguration emailConfiguration)
    {
        var message = new MimeMessage();
        message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
        message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

        message.Subject = emailMessage.Subject;
        //We will say we are sending HTML. But there are options for plaintext etc. 
        message.Body = new TextPart(TextFormat.Html)
        {
            Text = emailMessage.Content
        };

        //Be careful that the SmtpClient class is the one from Mailkit not the framework!
        using var emailClient = new SmtpClient();
        //The last parameter here is to use SSL (Which you should!)
        emailClient.Connect(emailConfiguration.SmtpServer, emailConfiguration.SmtpPort, true);

        //Remove any OAuth functionality as we won't be using it. 
        emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

        emailClient.Authenticate(emailConfiguration.SmtpUsername, emailConfiguration.SmtpPassword);

        emailClient.Send(message);

        emailClient.Disconnect(true);
    }
}