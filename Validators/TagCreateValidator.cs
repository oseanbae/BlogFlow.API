using FluentValidation;
using BlogFlow.API.DTOs.Tag;
namespace BlogFlow.API.Validators
{
    public class TagCreateValidator : AbstractValidator<TagCreateDTO>
    {
        public TagCreateValidator() 
        {
            RuleFor(t => t.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50)
                .Must(t => t.Trim() == t)
                .WithMessage("Tag must not have leading or trailing spaces.");
        }
    }
}
