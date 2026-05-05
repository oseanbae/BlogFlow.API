using BlogFlow.API.DTOs.Comment;
using FluentValidation;

namespace BlogFlow.API.Validators
{
    public class CommentUpdateValidator : AbstractValidator<CommentUpdateDTO>
    {
        public CommentUpdateValidator()
        {
            RuleFor(c => c.Body)
                .NotEmpty()
                    .WithMessage("Comment body cannot be empty.")
                .MaximumLength(10000)
                    .WithMessage("You reached the character limit of 10,000.");
        }
    }
}
