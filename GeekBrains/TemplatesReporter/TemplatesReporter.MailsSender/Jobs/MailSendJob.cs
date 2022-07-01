using Microsoft.Extensions.Options;
using Quartz;
using TemplatesReporter.Mail.Core;
using TemplatesReporter.MailsSender.Data;

namespace TemplatesReporter.MailsSender.Jobs;

public class MailSendJob : IJob
{
    private readonly EmailsRepository _emailsRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<MailSendJob> _logger;
    private readonly EmailConfiguration _emailSendDomainConfiguration;

    public MailSendJob(
        EmailsRepository emailsRepository,
        IEmailService emailService,
        IOptions<EmailConfiguration> emailSendDomainConfiguration,
        ILogger<MailSendJob> logger)
    {
        _emailsRepository = emailsRepository;
        _emailService = emailService;
        _logger = logger;
        _emailSendDomainConfiguration = emailSendDomainConfiguration.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var emails = _emailsRepository.EmailsToSend();
        if (emails.Count <= 0) return;

        List<Task> sendTasks = emails.Select(SendEmail).ToList();

        await Task.WhenAll(sendTasks);
    }

    private Task SendEmail(Email email)
    {
        try
        {
            _emailService.Send(email.Message, _emailSendDomainConfiguration);
            _emailsRepository.UpdateSendStatus(email.Id, true);
            _logger.LogInformation("Email {subject} sended", email.Message.Subject);
        }
        catch
        {
            _emailsRepository.UpdateSendStatus(email.Id, false);
            _logger.LogInformation("Email {subject} failed to send", email.Message.Subject);
        }

        return Task.CompletedTask;
    }
}