using FluentValidation;
using BlogFlow.API.DTOs.Auth;

namespace BlogFlow.API.Validators
{
    public class UserRegisterValidator : AbstractValidator<RegisterRequestDTO>
    {
        public UserRegisterValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                    .WithMessage("Username is required.")
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty()
                    .WithMessage("Email is required.")
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.Password)
                .NotEmpty()
                    .WithMessage("Password is required.")
                .MinimumLength(6)
                .MaximumLength(100);
        }
    }
}
