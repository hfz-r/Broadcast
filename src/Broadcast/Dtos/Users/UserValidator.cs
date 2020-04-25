using FluentValidation;

namespace Broadcast.Dtos.Users
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Designation).NotEmpty().WithMessage("Designation is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleForEach(x => x.Roles).NotEmpty().WithMessage("Roles is required.");
        }
    }
}