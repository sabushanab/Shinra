using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Shinra.Services
{
    public class CacheService
    {
        private static readonly MemoryCache _cache = new MemoryCache("cache");

        public static T GetAndSet<T>(string key, Func<T> method, int time = 10) where T: class 
        {
            var result = _cache[key] as T;
            if (result == null) 
            {
                result = method.Invoke() as T;
                if (result != null) 
                {
                    _cache.Add(key, result, DateTimeOffset.Now.AddMinutes(time));
                }
            }
            return result;
        }

        public static async Task<T> GetAndSetAsync<T>(string key, Func<Task<T>> task, int time = 10) where T : class 
        {
            var result = _cache[key] as T;
            if (result == null) 
            {
                result = await task();
                if (result != null) 
                {
                    _cache.Add(key, result, DateTimeOffset.Now.AddMinutes(time));
                }
            }
            return result;
        }

        public static void Set(string key, object data, int time = 10)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(time);
            _cache.Set(key, data, policy);
        }
    }
}