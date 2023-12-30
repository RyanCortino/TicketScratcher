using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TicketScratcher.Server.Application.Common.Extensions;

public static class OptionsBuilderFluentValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidatFluently<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder
    )
        where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
            s =>
                new FluentValidationOptions<TOptions>(
                    optionsBuilder.Name,
                    s.GetRequiredService<IValidator<TOptions>>()
                )
        );

        return optionsBuilder;
    }

    public class FluentValidationOptions<TOptions> : IValidateOptions<TOptions>
        where TOptions : class
    {
        private readonly IValidator<TOptions> _validator;

        public string? Name { get; }

        public FluentValidationOptions(string? name, IValidator<TOptions> validator)
        {
            Name = name;
            _validator = validator;
        }

        ValidateOptionsResult IValidateOptions<TOptions>.Validate(string? name, TOptions options)
        {
            if (Name is not null && Name != name)
            {
                return ValidateOptionsResult.Skip;
            }

            Guard.Against.Null(options);

            var validationResult = _validator.Validate(options);

            if (validationResult.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            var errors = validationResult.Errors.Select(
                x =>
                    $"Options validation for \"{x.PropertyName}\" with error: \"{x.ErrorMessage}\"."
            );

            return ValidateOptionsResult.Fail(errors);
        }
    }
}
