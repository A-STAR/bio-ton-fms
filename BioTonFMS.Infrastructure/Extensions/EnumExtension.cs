using System.ComponentModel;
using System.Reflection;

namespace BioTonFMS.Infrastructure.Extensions
{
    public static class EnumExtension
    {
        public static string? GetDescription(this Enum value)
        {
            var type = value.GetType();
            string? name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo? field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public static KeyValuePair[] GetKeyValuePairs<T>() where T : Enum
        {
            var pairsList = new List<KeyValuePair>();

            foreach (var key in ((T[])Enum.GetValues(typeof(T))).OrderBy(e => e))
            {
                var pair = new KeyValuePair
                {
                    Key = key.ToString(),
                    Value = key.GetDescription() ?? ""
                };

                pairsList.Add(pair);
            }
            KeyValuePair[]? list = pairsList.OrderBy(p => p.Value).ToArray();
            return list;
        }

        public static KeyValuePair GetKeyValuePair<T>(T value) where T : Enum
        {
            var name = Enum.GetName(typeof(T), value);
            var description = value.GetDescription();

            if (name != null && description != null)
            {
                return new KeyValuePair(name, description);
            }
            throw new ArgumentException($"Неправильное значение {value} для перечисления {nameof(T)}");
        }
    }
}
