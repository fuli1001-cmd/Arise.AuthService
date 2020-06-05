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
        private readonly IOptionsSnapshot<RedisSettings> _redisSettings;
        private readonly ILogger<RedisService> _logger;

        public RedisService(IOptionsSnapshot<RedisSettings> redisSettings, ILogger<RedisService> logger)
        {
            _redisSettings = redisSettings ?? throw new ArgumentNullException(nameof(redisSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetAsync(string key)
        {
            using (var redis = await ConnectAsync())
            {
                var db = redis.GetDatabase();
                return await db.StringGetAsync(key);
            }
        }

        public async Task SetAsync(string key, RedisValue value)
        {
            using (var redis = await ConnectAsync())
            {
                var db = redis.GetDatabase(0);
                await db.StringSetAsync(key, value);
            }
        }

        private async Task<ConnectionMultiplexer> ConnectAsync()
        {
            try
            {
                _logger.LogInformation("**********redis host: {RedisHost}*************", _redisSettings.Value.Host);
                _logger.LogInformation("**********redis port: {RedisPort}*************", _redisSettings.Value.Port);
                _logger.LogInformation("**********redis password: {RedisPassword}*************", _redisSettings.Value.Password);

                var configString = $"{_redisSettings.Value.Host}:{_redisSettings.Value.Port},connectRetry=5,password={_redisSettings.Value.Password}";
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
