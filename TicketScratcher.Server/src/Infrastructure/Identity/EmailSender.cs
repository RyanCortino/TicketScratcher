using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using TicketScratcher.Server.Application.Common.Interfaces;
using TicketScratcher.Server.Infrastructure.Messages;
using TicketScratcher.Server.Infrastructure.Options;

namespace TicketScratcher.Server.Infrastructure.Identity;

public class EmailSender(
    IOptions<EmailOptions> emailSettings,
    ISmtpClient smtpClient,
    IMessageBuilder messageBuilder
) : IEmailSender
{
    private readonly ISmtpClient _smtpClient = smtpClient;
    private readonly IMessageBuilder _messageBuilder = messageBuilder;
    private readonly EmailOptions _settings = emailSettings.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await _smtpClient.ConnectAsync(
            host: _settings.SmtpServer,
            port: _settings.SmtpPort,
            useSsl: _settings.UseSsl,
            cancellationToken: default
        );

        _messageBuilder
            .FromSender("sender@ticket-scratcher.net")
            .ToRecipients(new List<string>() { email })
            .WithSubject(subject)
            .WithHtmlBody(htmlMessage);

        await _smtpClient.SendAsync((_messageBuilder as MimeMessageBuilder)?.GetResult);

        await _smtpClient.DisconnectAsync(true);
    }
}
