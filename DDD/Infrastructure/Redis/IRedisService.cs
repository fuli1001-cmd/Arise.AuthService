using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arise.DDD.Infrastructure.Redis
{
    public interface IRedisService
    {
        Task StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expireTimeSpan);

        Task<string> StringGetAsync(RedisKey key);

        Task HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value);

        Task HashDeleteAsync(RedisKey key, RedisValue hashField);

        Task<bool> KeyDeleteAsync(RedisKey key);

        Task PublishAsync(RedisChannel channel, RedisValue value);
    }
}
