using MimeKit;
using TicketScratcher.Server.Application.Common.Interfaces;

namespace TicketScratcher.Server.Infrastructure.Messages;

public class MimeMessageBuilder : IMessageBuilder
{
    static MimeMessageBuilder() { }

    public MimeMessageBuilder() { }

    private readonly MimeMessage _mimeMessage = new();

    public MimeMessage GetResult => _mimeMessage;

    IMessageBuilder IMessageBuilder.Reset()
    {
        return new MimeMessageBuilder();
    }

    IMessageBuilder IMessageBuilder.FromSender(string address, string? displayName)
    {
        var sender = new MailboxAddress(displayName, address);

        _mimeMessage.From.Add(sender);

        return this;
    }

    IMessageBuilder IMessageBuilder.ToRecipients(IEnumerable<string> addressess)
    {
        var recipients = Enumerable.Empty<MailboxAddress>().ToList();

        foreach (var address in addressess)
        {
            recipients.Add(new(null, address));
        }

        _mimeMessage.Bcc.AddRange(recipients);

        return this;
    }

    IMessageBuilder IMessageBuilder.WithHtmlBody(string context)
    {
        _mimeMessage.Body = new BodyBuilder() { HtmlBody = context }.ToMessageBody();

        return this;
    }

    IMessageBuilder IMessageBuilder.WithSubject(string context)
    {
        _mimeMessage.Subject = context;

        return this;
    }
}
