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
                .WithMessage("Please enter your username or email.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Please enter your password.");

            // Username or Email field
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty()
                .WithMessage("Username or Email is required.")
                .MinimumLength(3)
                .MaximumLength(100)
                .WithMessage("Username or Email must be between 3 and 100 characters.");

            // Password
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(6)
                .MaximumLength(100)
                .WithMessage("Password must be between 6 and 100 characters.");
        }
    }
}