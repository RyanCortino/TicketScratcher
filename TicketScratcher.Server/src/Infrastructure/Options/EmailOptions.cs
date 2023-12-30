using FluentValidation;

namespace TicketScratcher.Server.Infrastructure.Options;

public class EmailOptions
{
    public const string SectionName = "EmailConfiguration";

    public bool UseSsl { get; set; } = true;
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 0;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
}

public class EmailOptionsValidator : AbstractValidator<EmailOptions>
{
    public EmailOptionsValidator() { }
}
