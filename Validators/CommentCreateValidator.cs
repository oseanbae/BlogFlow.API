using FluentValidation;
using BlogFlow.API.DTOs.Comment;

namespace BlogFlow.API.Validators
{
    public class CommentCreateValidator : AbstractValidator<CommentCreateDTO>
    {
        public CommentCreateValidator() 
        {
            RuleFor(c => c.Body)
                .NotEmpty()
                    .WithMessage("Comment body cannot be empty.")
                .MaximumLength(10000)
                    .WithMessage("You reached the character limit of 10,000.");
        }
    }
}
