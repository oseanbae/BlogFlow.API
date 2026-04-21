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
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(100);
        }
    }
}
