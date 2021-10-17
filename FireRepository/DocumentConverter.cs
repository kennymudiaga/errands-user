using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace FireRepository
{
    public static class DocumentConverter
    {
        public static T Parse<T>(this DocumentSnapshot document)
        {
            var dictionary = document.ToDictionary();
            dictionary.TryAdd("Id", document.Id);
            foreach (var pair in dictionary.Where(x=>x.Value is Timestamp).Select(x=> new { x.Key, Value = (Timestamp)x.Value }))
            {
                dictionary[pair.Key] = pair.Value.ToDateTime();
            }
            var json = JsonSerializer.Serialize(dictionary);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static Dictionary<string, object> ToDictionary(this object entity)
        {
            var data = new Dictionary<string, object>();
            var properties = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach(var item in properties)
            {
                data.Add(item.Name, item.GetValue(entity));
            }
            return data;
        }

    }
}
