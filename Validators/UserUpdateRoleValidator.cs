using BlogFlow.API.DTOs.User;
using FluentValidation;

namespace BlogFlow.API.Validators
{
    public class UserUpdateRoleValidator : AbstractValidator<UserUpdateRoleDTO>
    {
        public UserUpdateRoleValidator()
        {
            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Invalid role value.");
        }
    }
}
