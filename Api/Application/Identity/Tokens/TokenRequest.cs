using FluentValidation;
using Microsoft.Extensions.Localization;
using RumarApi.Application.Common.Validation;

namespace RumarApi.Application.Identity.Tokens
{
    public record TokenRequest(string Email, string Password);

    public class TokenRequestValidator : CustomValidator<TokenRequest>
    {
        public TokenRequestValidator(IStringLocalizer<TokenRequestValidator> T)
        {
            RuleFor(p => p.Email).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .EmailAddress()
                    .WithMessage(T["Invalid Email Address."]);

            RuleFor(p => p.Password).Cascade(CascadeMode.Stop)
                .NotEmpty();
        }
    }
}
