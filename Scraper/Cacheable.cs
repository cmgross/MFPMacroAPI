using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace Scraper
{
    public static class Cacheable
    {
        public static ObjectCache Cache
        {
            get { return MemoryCache.Default; }
        }

        #region CacheExpirations

        public static CacheItemPolicy Never = new CacheItemPolicy
        {
            AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration,
            Priority = CacheItemPriority.NotRemovable,
            SlidingExpiration = new TimeSpan(24, 0, 0)
        };

        public static CacheItemPolicy ShortTerm = new CacheItemPolicy
        {
            AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration,
            Priority = CacheItemPriority.NotRemovable,
            SlidingExpiration = new TimeSpan(1, 0, 0)
        };

        public static CacheItemPolicy Shortest = new CacheItemPolicy
        {
            AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration,
            Priority = CacheItemPriority.NotRemovable,
            SlidingExpiration = new TimeSpan(0, 1, 0)
        };

        #endregion

        public static CookieCollection Login()
        {
            if (Cache == null) return FitocracyScraper.Login();
            const string cacheKey = "FitocracyLoginCookie";
            if (Cache.Get(cacheKey) != null) return (CookieCollection)Cache[cacheKey];
            var loginCookie = FitocracyScraper.Login();
            Cache.Set(cacheKey, loginCookie, ShortTerm);
            return loginCookie;
        }

        public static string GetUserId(string userName, CookieCollection cookies)
        {
            if (Cache == null) return FitocracyScraper.GetUserId(userName, cookies);
            var cacheKey = "{userName}_UserId".Replace("userName", userName);
            if (Cache.Get(cacheKey) != null) return (string)Cache[cacheKey];
            var userId = FitocracyScraper.GetUserId(userName, cookies);
            Cache.Set(cacheKey, userId, Never);
            return userId;
        }
    }
}
