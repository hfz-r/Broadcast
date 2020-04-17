using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Security;
using Broadcast.Services.Auth;
using FluentValidation;
using MediatR;

namespace Broadcast.Features.Auth
{
    public class Token
    {
        public class Query : IRequest<TokenEnvelope>
        {
            public Query(string guid)
            {
                Guid = guid;
            }

            public string Guid { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Guid).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, TokenEnvelope>
        {
            private readonly IAuthenticationService _authentication;
            private readonly IJwtTokenGenerator _tokenGenerator;

            public QueryHandler(IAuthenticationService authentication, IJwtTokenGenerator tokenGenerator)
            {
                _authentication = authentication;
                _tokenGenerator = tokenGenerator;
            }

            public async Task<TokenEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _authentication.AuthenticatedUserAsync();
                if (user == null || !(Guid.TryParse(request.Guid, out var validGuid) && user.Guid == validGuid))
                    throw new RestException(HttpStatusCode.Unauthorized, new {Error = "Failed to authenticate user."});

                return await _tokenGenerator.CreateToken(user.AccountName);
            }
        }
    }
}