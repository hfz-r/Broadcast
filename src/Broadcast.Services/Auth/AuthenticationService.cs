using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Broadcast.Core.Configuration;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Core.Infrastructure.Mapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Broadcast.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly LdapSettingOptions _ldapOptions;
        private readonly IUnitOfWork _worker;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(
            IOptions<LdapSettingOptions> ldapOptions,
            IUnitOfWork worker,
            IHttpContextAccessor httpContextAccessor)
        {
            _ldapOptions = ldapOptions.Value;
            _worker = worker;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Utilities

        private async Task<User> CastToUserAsync(UserPrincipal userPrincipal, Func<Task<User>> castFunc)
        {
            var repo = _worker.GetRepositoryAsync<User>();

            var user = await repo.SingleAsync(u => u.AccountName == userPrincipal.SamAccountName);
            if (user == null)
            {
                await repo.AddAsync(userPrincipal.ToEntity<User>());
                await _worker.SaveChangesAsync();
            }

            return await castFunc();
        }

        private async Task RunSignInRoutineAsync(User user, bool isPersistent = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Name, user.AccountName,
                    ClaimValueTypes.String,
                    AuthenticationDefaults.ClaimsIssuer)
            };

            var userIdentity = new ClaimsIdentity(claims, AuthenticationDefaults.AuthenticationScheme);

            await _httpContextAccessor.HttpContext.SignInAsync(AuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(userIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = isPersistent, // todo
                    IssuedUtc = DateTime.UtcNow
                });
        }

        #endregion

        public async Task<User> SignInAsync(string adUsername, string adPassword)
        {
            var repo = _worker.GetRepositoryAsync<User>();

            try
            {
                using (var context = new PrincipalContext(
                    ContextType.Domain,
                    _ldapOptions.DomainName,
                    _ldapOptions.DomainContainer))
                {
                    if (!context.ValidateCredentials(adUsername, adPassword)) throw new Exception("Invalid Active Directory credentials.");

                    var userPrincipal = UserPrincipal.FindByIdentity(context, adUsername);
                    var user = await CastToUserAsync(
                        userPrincipal,
                        async () => await repo.SingleAsync(u => u.AccountName == adUsername));

                    await RunSignInRoutineAsync(user);

                    return user;
                }
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.Unauthorized, new {Error = ex.Message});
            }
        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(AuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<User> AuthenticatedUserAsync()
        {
            var repo = _worker.GetRepositoryAsync<User>();

            try
            {
                var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(AuthenticationDefaults.AuthenticationScheme);
                if (!authenticateResult.Succeeded) return null;

                var usernameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name && claim.Issuer.Equals(AuthenticationDefaults.ClaimsIssuer));
                return usernameClaim != null
                    ? await repo.SingleAsync(u => u.AccountName == usernameClaim.Value)
                    : throw new ArgumentNullException(nameof(usernameClaim.Value));
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.Unauthorized, new {Error = ex.Message});
            }
        }
    }
}