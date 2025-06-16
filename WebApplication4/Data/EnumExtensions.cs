using System.Reflection;
using System.Runtime.Serialization;

namespace WebApplication4.Data
{
    public static class EnumExtensions
    {
        public static string GetEnumMemberValue(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<EnumMemberAttribute>()?
                .Value ?? enumValue.ToString();
        }

        public static T ToEnumFromEnumMemberValue<T>(this string value) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                if (attribute != null)
                {
                    if (attribute.Value == value)
                        return (T)field.GetValue(null)!;
                }
                else
                {
                    if (field.Name == value)
                        return (T)field.GetValue(null)!;
                }
            }
            throw new ArgumentException($"Unknown value: {value}");
        }
    }
}
