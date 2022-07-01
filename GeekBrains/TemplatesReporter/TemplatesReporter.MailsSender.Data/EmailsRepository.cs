namespace TemplatesReporter.MailsSender.Data;

public class EmailsRepository
{
    private static readonly List<Email> _emails = new List<Email>();

    public IReadOnlyCollection<Email> EmailsToSend()
    {
        var currentDate = DateTime.Now;
        return _emails.Where(email => email.SendState == SendStates.NotSend && email.SendDate > currentDate).ToList();
    }

    public void AddEmailToSend(Email email)
    {
        email.Id = Guid.NewGuid();
        _emails.Add(email);
    }

    public void UpdateSendStatus(Guid emailId, bool isSuccess)
    {
        if (_emails.FirstOrDefault(e => e.Id == emailId) is not { } email) return;

        email.SendState = isSuccess ? SendStates.Send : SendStates.Fail;
    }

    public IReadOnlyCollection<Email> UserMails(string userId)
    {
        return _emails.Where(email => string.Equals(email.UserId, userId, StringComparison.InvariantCulture)).ToList();
    }
}