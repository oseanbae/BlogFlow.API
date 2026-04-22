using BlogFlow.API.DTOs.Post;
using FluentValidation;

namespace BlogFlow.API.Validators
{
    public class PostUpdateValidator : AbstractValidator<PostUpdateDTO>
    {
        public PostUpdateValidator()
        {
            RuleFor(p => p.Title)
                .MinimumLength(3)
                .MaximumLength(255)
                .When(p => p.Title != null);

            RuleFor(p => p.Body)
                .MinimumLength(3)
                .MaximumLength(10000)
                .When(p => p.Body != null);

            RuleFor(p => p.CategoryId)
                .Must(id => id != Guid.Empty)
                .When(x => x.CategoryId.HasValue)
                .WithMessage("CategoryId cannot be empty");

            RuleFor(p => p.TagIds)
                .Must(tags => tags!.All(id => id != Guid.Empty))
                .When(x => x.TagIds != null)
                .WithMessage("TagIds cannot contain empty GUIDs.");

            RuleFor(x => x.TagIds)
                .Must(tags => tags == null || tags!.Distinct().Count() == tags.Count)
                .When(x => x.TagIds != null)
                .WithMessage("Duplicate TagIds are not allowed");
        }
    }
}
