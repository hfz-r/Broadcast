namespace Broadcast.Services.Caching.CachingDefaults
{
    public static class SecurityCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : role id
        /// {1} : permission name
        /// </remarks>
        public static string PermissionsAllowedCacheKey => "brdcst.permission.allowed-{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : role id
        /// </remarks>
        public static string PermissionsByRoleIdCacheKey => "brdcst.permission.byroleid-{0}";

    }
}