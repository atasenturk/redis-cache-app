using System.Reflection;
using StackExchange.Redis;

namespace RedisCachingApp.Extensions
{
    public static class RedisExtension
    {
        public static HashEntry[] ToHashEntries(this object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            var value = properties
                .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
                .Select(property => new HashEntry(property.Name, property.GetValue(obj)
                    .ToString())).ToArray();
            return value;
        }

        public static T ConvertFromRedis<T>(this HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return (T)obj;
        }
    }
}
