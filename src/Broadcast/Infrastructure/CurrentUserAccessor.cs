using System;
using System.Linq;
using System.Security.Claims;
using Broadcast.Core;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Infrastructure
{
    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly IUnitOfWork _worker;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private User _cachedUser;

        public CurrentUserAccessor(IUnitOfWork worker, IHttpContextAccessor httpContextAccessor)
        {
            _worker = worker;
            _httpContextAccessor = httpContextAccessor;
        }

        protected void StoreCookie(Guid guid)
        {
            if (_httpContextAccessor.HttpContext?.Response == null) return;

            var cookieName = $"{UserDefaults.Prefix}{UserDefaults.GuidCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            var cookieExpiresDate = DateTime.Now.AddHours(1); //todo - make it configurable

            if (guid == Guid.Empty) cookieExpiresDate = DateTime.Now.AddMonths(-1);

            var options = new CookieOptions
            {
                HttpOnly = false, //todo - should not expose this
                Expires = cookieExpiresDate
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, guid.ToString(), options);
        }

        public string GetCurrentUsername() => _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        #region Properties

        public User CurrentUser
        {
            get
            {
                if (_cachedUser != null) return _cachedUser;

                var repo = _worker.GetRepositoryAsync<User>();

                var user = AsyncHelper.RunSync(() => repo.SingleAsync(u => u.AccountName == GetCurrentUsername()));
                if (user == null) return null;

                StoreCookie(user.Guid);
                return _cachedUser = user;
            }
            set
            {
                StoreCookie(value.Guid);
                _cachedUser = value;
            }
        }

        #endregion
    }
}