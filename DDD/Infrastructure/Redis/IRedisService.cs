using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arise.DDD.Infrastructure.Redis
{
    public interface IRedisService
    {
        Task SetAsync(string key, RedisValue value);
        Task<string> GetAsync(string key);
    }
}
