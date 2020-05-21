using FluentValidation;

namespace Broadcast.Core.Dtos.Projects
{
    public class ProjectValidator : AbstractValidator<ProjectDto>
    {
        public ProjectValidator()
        {
            RuleFor(x => x.Project).NotEmpty().WithMessage("Project is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        }
    }
}
