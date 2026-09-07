using BlogApi.BLL.DTOs.Comment;
using FluentValidation;

namespace BlogApi.BLL.Validators.Comment
{
    public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentDtoValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required.")
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");

            RuleFor(x => x.PostId)
                .GreaterThan(0).WithMessage("Valid post ID is required.");
        }
    }
}