using FluentValidation;
using BlogFlow.API.DTOs.Auth;

namespace BlogFlow.API.Validators
{
    public class UserRegisterValidator : AbstractValidator<RegisterRequestDTO>
    {
        public UserRegisterValidator()
        {
            RuleFor(x => x.Username)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Username is required.")
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Email is required.")
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.Password)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Password is required.")
                .MinimumLength(6)
                .MaximumLength(100);
        }
    }
}
