using BlogApi.BLL.Common;
using BlogApi.BLL.DTOs.User;
using FluentValidation;

namespace BlogApi.BLL.Validators.User
{
    public class UpdateUserRoleDtoValidator : AbstractValidator<UpdateUserRoleDto>
    {
        public UpdateUserRoleDtoValidator()
        {
            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(role => role == Roles.Admin || role == Roles.User)
                .WithMessage($"Role must be either '{Roles.Admin}' or '{Roles.User}'.");
        }
    }
}