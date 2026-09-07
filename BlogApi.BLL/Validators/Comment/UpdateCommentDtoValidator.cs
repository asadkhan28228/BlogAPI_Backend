using FluentValidation;

// Note: UpdateCommentDto is declared with no namespace in your project (global namespace),
// so no "using" is needed for it here.
namespace BlogApi.BLL.Validators.Comment
{
    public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
    {
        public UpdateCommentDtoValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required.")
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
        }
    }
}