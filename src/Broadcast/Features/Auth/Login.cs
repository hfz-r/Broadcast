using System;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Services.Auth;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;

namespace Broadcast.Features.Auth
{
    public class Login
    {
        public class Command : IRequest<Guid>
        {
            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
                RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            }
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly IAuthenticationService _authentication;

            public Handler(IAuthenticationService authentication)
            {
                _authentication = authentication;
            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _authentication.SignInAsync(request.Username, request.Password);   

                return user.Guid;
            }
        }
    }
}