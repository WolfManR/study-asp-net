namespace TemplatesReporter.MailsSender.Data;

public class SendStates
{
    private static readonly SendState[] States = new[]
    {
        new SendState{ Id = 0, Name = "Send", Description = "Email successfully sended to all address's"},
        new SendState{ Id = 1, Name = "OnSend", Description = "Email on send"},
        new SendState{ Id = 2, Name = "NotSend", Description = "Email on send"},
        new SendState{ Id = 3, Name = "FailSend", Description = "Fail on send email"},
    };

    public static SendState Send => States[0];
    public static SendState OnSend => States[1];
    public static SendState NotSend => States[2];
    public static SendState Fail => States[3];
}