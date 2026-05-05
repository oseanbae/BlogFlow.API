using BlogFlow.API.DTOs.Post;
using FluentValidation;


namespace BlogFlow.API.Validators
{
    public class PostCreateValidator : AbstractValidator<PostCreateDTO>
    {
        public PostCreateValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(255)
                .Must(t => t.Trim() == t)
                .WithMessage("Title must not have leading or trailing spaces.");

            RuleFor(p => p.Body)
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(10000);

            RuleFor(p => p.CategoryId)
                .NotEmpty()
                .WithMessage("CategoryId cannot be an empty GUID");

            RuleFor(p => p.TagIds)
                .Must(tags => tags == null || tags.All(id => id != Guid.Empty))
                .WithMessage("Tag Ids cannot contain empty GUID");

            RuleFor(p => p.TagIds)
                .Must(tags => tags == null || tags.Distinct().Count() == tags.Count)
                .WithMessage("Duplicate TagIds are not allowed.");
        }
    }
}
