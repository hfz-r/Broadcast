﻿using System.Linq;
using System.Security.Claims;
using Broadcast.Core.Domain.Users;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Core.Infrastructure
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

        public string GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext.User?.Claims
                ?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        #region Properties

        public User CurrentUser
        {
            get
            {
                if (_cachedUser != null)
                    return _cachedUser;

                var repo = _worker.GetRepositoryAsync<User>();

                var user = AsyncHelper.RunSync(() => repo.SingleAsync(u => u.Username == GetCurrentUsername()));

                if (user == null) return null;

                return _cachedUser = user;
            }
            set => _cachedUser = value;
        }

        #endregion
    }
}