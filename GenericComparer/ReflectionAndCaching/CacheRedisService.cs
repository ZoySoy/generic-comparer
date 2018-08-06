using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReflectionAndCaching
{
    class CacheRedisService<T>
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        IDatabase db = ConnectionMultiplexer.Connect("localhost").GetDatabase();

        public CacheRedisService()
        {

        }

        public CacheRedisService(string connectionString)
        {
            if (!String.IsNullOrWhiteSpace(connectionString))
            {
                redis = ConnectionMultiplexer.Connect(connectionString);
            }

            db = redis.GetDatabase();
        }

        public void Put(T obj, RedisKey key)
        {
            string serializedObj = JsonConvert.SerializeObject(obj);
            db.StringSet(key, serializedObj);
        }

        public T Get(RedisKey key)
        {
            return JsonConvert.DeserializeObject<T>(db.StringGet(key));
        }
    }
}
