namespace TicketScratcher.Server.Application.Common.Interfaces;

public interface IMessageBuilder
{
    IMessageBuilder Reset();

    IMessageBuilder FromSender(string address, string? displayName = null);

    IMessageBuilder ToRecipients(IEnumerable<string> addressess);

    IMessageBuilder WithSubject(string context);

    IMessageBuilder WithHtmlBody(string context);
}
