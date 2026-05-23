using BlogFlow.API.DTOs.User;
using FluentValidation;

namespace BlogFlow.API.Validators
{
    public class UserChangePasswordValidator : AbstractValidator<UserChangePasswordDTO>
    {
        public UserChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MinimumLength(4);

        }
    }
}
