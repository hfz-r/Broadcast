using System;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Dtos.Users;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Services.Users;
using FluentValidation;
using MediatR;

namespace Broadcast.Features.Users
{
    public class Create
    {
        public class Command : IRequest<UserEnvelope>
        {
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
            private readonly IUserService _userService;

            public Handler(IUserService userService)
            {
                _userService = userService;
            }

            public async Task<UserEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userService.InsertUserAsync(request.User);
                    return new UserEnvelope(user.ToDto<UserDto>());
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }
}