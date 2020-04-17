using FluentValidation;

namespace Broadcast.Dtos.Messages
{
    public class MessageValidator : AbstractValidator<MessageDto>
    {
        public MessageValidator()
        {
            RuleFor(x => x.ProjectDto.Project).NotEmpty().WithMessage("Project is required.");
            RuleFor(x => x.AboutDto.Title).NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.AboutDto.Description).NotEmpty().WithMessage("Description is required.");
        }
    }
}