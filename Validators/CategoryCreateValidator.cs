using FluentValidation;
using BlogFlow.API.DTOs.Categories;

namespace BlogFlow.API.Validators
{
    public class CategoryCreateValidator : AbstractValidator<CategoryCreateDTO>
    {
        public CategoryCreateValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(255)
                .Must(t => t.Trim() == t)
                .WithMessage("Category must not have leading or trailing spaces.");
        }
    }
}
