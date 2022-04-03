using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ibanity;

public class JsonDotPathConverter : JsonConverter
  {
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
      JObject jobject = JObject.Load(reader);
      object instance = Activator.CreateInstance(objectType);
      foreach (PropertyInfo propertyInfo in ((IEnumerable<PropertyInfo>) objectType.GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.CanRead && p.CanWrite)))
      {
        JsonPropertyAttribute propertyAttribute = propertyInfo.GetCustomAttributes(true).OfType<JsonPropertyAttribute>().FirstOrDefault<JsonPropertyAttribute>();
        string str = propertyAttribute != null ? propertyAttribute.PropertyName : propertyInfo.Name;
        JToken jtoken = ((JToken) jobject).SelectToken(str);
        if (jtoken != null && jtoken.Type != JTokenType.Null)
        {
          object obj = jtoken.ToObject(propertyInfo.PropertyType, serializer);
          propertyInfo.SetValue(instance, obj, (object[]) null);
        }
      }
      return instance;
    }

    public override bool CanConvert(Type objectType) => true;

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      IEnumerable<PropertyInfo> propertyInfos = value.GetType().GetRuntimeProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.CanRead && p.CanWrite));
      JObject jobject1 = new JObject();
      foreach (PropertyInfo propertyInfo in propertyInfos)
      {
        JsonPropertyAttribute propertyAttribute = propertyInfo.GetCustomAttributes(true).OfType<JsonPropertyAttribute>().FirstOrDefault<JsonPropertyAttribute>();
        DefaultValueAttribute defaultValueAttribute = propertyInfo.GetCustomAttributes(true).OfType<DefaultValueAttribute>().FirstOrDefault<DefaultValueAttribute>();
        object obj1 = propertyInfo.GetValue(value);
        if (obj1 != null || propertyAttribute.NullValueHandling != NullValueHandling.Ignore)
        {
          bool flag = false;
          string[] strArray = (propertyAttribute != null ? propertyAttribute.PropertyName : propertyInfo.Name).Split(new char[1]
          {
            '.'
          });
          JObject jobject2 = jobject1;
          for (int index = 0; index < strArray.Length; ++index)
          {
            if (index == strArray.Length - 1)
            {
              if (propertyInfo.PropertyType.IsArray)
              {
                jobject2[strArray[index]] = JToken.FromObject(obj1);
              }
              else
              {
                object obj2 = obj1;
                if (obj2 == null && propertyAttribute.DefaultValueHandling == DefaultValueHandling.Populate && defaultValueAttribute != null)
                  obj2 = defaultValueAttribute.Value;
                jobject2[strArray[index]] = (JToken) new JValue(obj2);
              }
            }
            else
            {
              if (jobject2[strArray[index]] == null)
                jobject2[strArray[index]] = (JToken) new JObject();
              jobject2 = (JObject) jobject2[strArray[index]];
            }
          }
          if (flag)
            ;
        }
      }
      serializer.Serialize(writer, (object) jobject1);
    }
  }