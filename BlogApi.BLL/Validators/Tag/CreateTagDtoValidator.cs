using BlogApi.BLL.DTOs.Tag;
using FluentValidation;

namespace BlogApi.BLL.Validators.Tag
{
    public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
    {
        public CreateTagDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tag name is required.")
                .Length(2, 50).WithMessage("Tag name must be between 2 and 50 characters.");
        }
    }
}