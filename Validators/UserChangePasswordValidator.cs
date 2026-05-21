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
                .Cascade(CascadeMode.Stop) //stop rule whenever a validation fails
                .NotEmpty()
                .Must(p => !string.IsNullOrWhiteSpace(p))
                .MinimumLength(4);
          
        }
    }
}
