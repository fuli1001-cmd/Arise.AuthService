using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arise.DDD.Infrastructure.Redis
{
    public class RedisService : IRedisService
    {
        private readonly RedisSettings _redisSettings;
        private readonly ILogger<RedisService> _logger;

        public RedisService(IOptionsSnapshot<RedisSettings> redisOptions, ILogger<RedisService> logger)
        {
            _redisSettings = redisOptions?.Value ?? throw new ArgumentNullException(nameof(redisOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> StringGetAsync(RedisKey key)
        {
            using (var redis = await ConnectAsync())
            {
                var db = redis.GetDatabase();
                return await db.StringGetAsync(key);
            }
        }

        public async Task StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expireTimeSpan)
        {
            using (var redis = await ConnectAsync())
            {
                var db = redis.GetDatabase(0);
                if (expireTimeSpan == null)
                    await db.StringSetAsync(key, value);
                else
                    await db.StringSetAsync(key, value, expireTimeSpan);
            }
        }

        public async Task HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value)
        {
            using (var redis = await ConnectAsync())
            {
                var db = redis.GetDatabase(0);
                await db.HashSetAsync(key, hashField, value);
            }
        }

        public async Task HashDeleteAsync(RedisKey key, RedisValue hashField)
        {
            using (var redis = await ConnectAsync())
            {
                var db = redis.GetDatabase(0);
                await db.HashDeleteAsync(key, hashField);
            }
        }

        public async Task<bool> KeyDeleteAsync(RedisKey key)
        {
            using (var redis = await ConnectAsync())
            {
                var db = redis.GetDatabase();
                return await db.KeyDeleteAsync(key);
            }
        }

        public async Task PublishAsync(RedisChannel channel, RedisValue value)
        {
            using (var redis = await ConnectAsync())
            {
                ISubscriber subscriber = redis.GetSubscriber();
                await subscriber.PublishAsync(channel, value);
            }
        }

        private async Task<ConnectionMultiplexer> ConnectAsync()
        {
            try
            {
                _logger.LogInformation("redis host: {RedisHost}", _redisSettings.Host);
                _logger.LogInformation("redis port: {RedisPort}", _redisSettings.Port);
                _logger.LogInformation("redis password: {RedisPassword}", _redisSettings.Password);

                var configString = $"{_redisSettings.Host}:{_redisSettings.Port},connectRetry=5,password={_redisSettings.Password}";
                return await ConnectionMultiplexer.ConnectAsync(configString);
            }
            catch (RedisConnectionException err)
            {
                _logger.LogError(err.ToString());
                throw err;
            }
        }
    }
}
