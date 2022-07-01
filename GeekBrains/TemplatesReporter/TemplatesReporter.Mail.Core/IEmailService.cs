namespace TemplatesReporter.Mail.Core;

public interface IEmailService
{
    void Send(EmailMessage emailMessage, EmailConfiguration emailConfiguration);
}