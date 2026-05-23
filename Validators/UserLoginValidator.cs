using FluentValidation;
using BlogFlow.API.DTOs.Auth;

namespace BlogFlow.API.Validators
{
    public class UserLoginValidator : AbstractValidator<LoginRequestDTO>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty()
                    .WithMessage("Username or Email is required.")
                .MinimumLength(3)
                    .WithMessage("Username or Email must be at least 3 characters.")
                .MaximumLength(100)
                    .WithMessage("Username or Email must not exceed 100 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                    .WithMessage("Password is required.")
                .MinimumLength(6)
                    .WithMessage("Password must be at least 6 characters.")
                .MaximumLength(100)
                    .WithMessage("Password must not exceed 100 characters.");
        }
    }
}