namespace Broadcast.Services.Caching.CachingDefaults
{
    public static class UserServiceCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : role name
        /// </remarks>
        public static string RolesByNameCacheKey => "brdcst.role.name-{0}";
    }
}