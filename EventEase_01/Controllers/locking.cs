
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
namespace EventEase_01.Controllers
{
   

    public class RedisDistributedLock
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisDistributedLock(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }

        public async Task<bool> AcquireLockAsync(string key, TimeSpan expiry)
        {
            return await _database.LockTakeAsync(key, Environment.MachineName, expiry);
        }

        public async Task ReleaseLockAsync(string key)
        {
            await _database.LockReleaseAsync(key, Environment.MachineName);
        }
    }

}
