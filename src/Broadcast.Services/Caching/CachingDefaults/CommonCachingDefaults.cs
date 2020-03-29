namespace Broadcast.Services.Caching.CachingDefaults
{
    public class CommonCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : key group
        /// </remarks>
        public static string GenericAttributeCacheKey => "brdcst.genericattribute.{0}-{1}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string GenericAttributePrefixCacheKey => "brdcst.genericattribute.";
    }
}