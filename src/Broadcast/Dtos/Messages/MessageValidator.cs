using FluentValidation;

namespace Broadcast.Dtos.Messages
{
    public class MessageValidator : AbstractValidator<MessageDto>
    {
        public MessageValidator()
        {
            RuleFor(x => x.Project).NotNull().NotEmpty().WithMessage("Project is required.");
            RuleFor(x => x.AboutDto.Title).NotNull().NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.AboutDto.Description).NotNull().NotEmpty().WithMessage("Description is required.");
        }
    }
}