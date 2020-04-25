using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Dtos.Users;
using Broadcast.Infrastructure;
using FluentValidation;
using MediatR;

namespace Broadcast.Features.Users
{
    public class Edit
    {
        public class Command : IRequest<UserEnvelope>
        {
            public string Username { get; set; }

            public UserDto User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.User).NotNull().SetValidator(new UserValidator());
            }
        }

        public class Handler : IRequestHandler<Command, UserEnvelope>
        {
            private readonly IUnitOfWork _worker;
            private readonly ICurrentUserAccessor _currentUser;

            public Handler(IUnitOfWork worker, ICurrentUserAccessor currentUser)
            {
                _worker = worker;
                _currentUser = currentUser;
            }

            public async Task<UserEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var userRepo = _worker.GetRepositoryAsync<User>();
                var roleRepo = _worker.GetRepositoryAsync<Role>();

                var user = await userRepo.SingleAsync(u => u.AccountName == request.Username);
                if (user == null) throw new RestException(HttpStatusCode.NotFound, new {User = $"User {Constants.NotFound}"});

                //user.Address = request.User.Address ?? user.Address todo
                user.Email = request.User.Email ?? user.Email;
                user.Name = request.User.FullName ?? user.Name;
                user.Title = request.User.Designation ?? user.Title;
                user.PhoneNumber = request.User.Phone ?? user.PhoneNumber;
                //handle roles
                foreach (var role in await roleRepo.GetQueryableAsync())
                {
                    if (request.User.Roles.Contains(role.Name))
                    {
                        if (user.UserRoles.Count(ur => ur.RoleId == role.Id) == 0)
                            user.AddUserRole(new UserRole {Role = role});
                    }
                    else
                    {
                        if (user.UserRoles.Count(ur => ur.RoleId == role.Id) > 0)
                            user.RemoveUserRole(user.UserRoles.FirstOrDefault(ur => ur.RoleId == role.Id));
                    }
                }

                await _worker.SaveChangesAsync();

                return new UserEnvelope(user.ToDto<UserDto>());
            }
        }
    }
}