// Type-safe cache key builder — prevents collisions and makes invalidation queries readable.
namespace Caching;

public static class CacheKeys
{
    public const string Version = "v1";   // bump on schema-incompatible changes

    public static class Orders
    {
        public static string ById(Guid id) => $"{Version}:order:{id:N}";
        public static string ByUser(Guid userId, int page) => $"{Version}:orders:user:{userId:N}:page:{page}";
        public const string AllTag = "orders";
        public static string UserTag(Guid userId) => $"orders:user:{userId:N}";
    }

    public static class Users
    {
        public static string ById(Guid id) => $"{Version}:user:{id:N}";
        public static string ByEmail(string email) => $"{Version}:user:email:{email.ToLowerInvariant()}";
    }

    public static class Catalog
    {
        public static string Product(Guid id) => $"{Version}:product:{id:N}";
        public static string Search(string query, string filtersHash) =>
            $"{Version}:search:{filtersHash}:{System.Web.HttpUtility.UrlEncode(query)}";
    }
}
