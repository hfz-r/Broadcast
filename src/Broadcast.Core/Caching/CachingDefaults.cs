namespace Broadcast.Core.Caching
{
    public static class CachingDefaults
    {
        public static int CacheTime => 60;

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Entity type name
        /// {1} : Entity id
        /// </remarks>
        public static string EntityCacheKey => "brdcst.{0}.id-{1}";
    }
}