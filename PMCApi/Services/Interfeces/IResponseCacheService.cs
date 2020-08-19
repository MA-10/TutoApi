using System;
using System.Threading.Tasks;

namespace Test.Services
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string cacheKey , object response , TimeSpan timeToLive);
        Task<string> GetCachedResponseAsync(string CacheKey);
    }
}