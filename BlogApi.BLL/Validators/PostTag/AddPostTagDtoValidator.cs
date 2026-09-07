using BlogApi.BLL.DTOs.PostTag;
using FluentValidation;

namespace BlogApi.BLL.Validators.PostTag
{
    public class AddPostTagDtoValidator : AbstractValidator<AddPostTagDto>
    {
        public AddPostTagDtoValidator()
        {
            RuleFor(x => x.TagId)
                .GreaterThan(0).WithMessage("Valid tag ID is required.");
        }
    }
}