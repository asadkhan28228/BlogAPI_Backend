using BlogApi.BLL.DTOs.Post;
using FluentValidation;

namespace BlogApi.BLL.Validators.Post
{
    public class UpdatePostDtoValidator : AbstractValidator<UpdatePostDto>
    {
        public UpdatePostDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Post title is required.")
                .Length(3, 200).WithMessage("Title must be between 3 and 200 characters.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Post content is required.")
                .MinimumLength(10).WithMessage("Content must be at least 10 characters.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Valid category ID is required.");
        }
    }
}