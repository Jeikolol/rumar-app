using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.Application.Common.Validation;

namespace Shared.Application.Identity.Users
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = default!;
    }

    public class ForgotPasswordRequestValidator : CustomValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator(IStringLocalizer<ForgotPasswordRequestValidator> T) =>
            RuleFor(p => p.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .EmailAddress()
                .WithMessage(T["Invalid Email Address."]);
    }
}
