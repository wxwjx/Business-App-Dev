using System;
using System.Web;
using System.Web.Caching;

namespace Business_App_Dev.Services
{
    public static class TranslationCache
    {
        // Cache for 24 hours by default
        public static string GetOrAdd(string key, Func<string> factory, int hours = 24)
        {
            if (string.IsNullOrWhiteSpace(key))
                return factory();

            object cached = HttpRuntime.Cache.Get(key);
            if (cached is string s && !string.IsNullOrWhiteSpace(s))
                return s;

            string value = factory();

            // Insert with absolute expiration
            HttpRuntime.Cache.Insert(
                key,
                value,
                dependencies: null,
                absoluteExpiration: DateTime.Now.AddHours(hours),
                slidingExpiration: Cache.NoSlidingExpiration
            );

            return value;
        }
    }
}
