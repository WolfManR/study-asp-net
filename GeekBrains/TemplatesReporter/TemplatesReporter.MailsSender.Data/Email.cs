using TemplatesReporter.Mail.Core;

namespace TemplatesReporter.MailsSender.Data;

public sealed class Email
{
    public Guid Id { get; set; }
    public string UserId { get; init; }
    public DateTime SendDate { get; init; }
    public EmailMessage Message { get; init; }
    public SendState SendState { get; set; } = SendStates.NotSend;
}