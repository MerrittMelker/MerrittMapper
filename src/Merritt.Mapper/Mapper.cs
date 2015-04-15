using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Merritt.Mapper
{
    public interface IMapper
    {
        List<TDestination> MapFromDataReader<TDestination>(IDataReader source);
    }

    public class Mapper : IMapper
    {
        public List<TDestination> MapFromDataReader<TDestination>(IDataReader source)
        {
            if (source == null) throw new ArgumentNullException("source");
            var destinationList = new List<TDestination>();
            while (source.Read())
            {
                var destination = Activator.CreateInstance<TDestination>();
                var properties = destination.GetType().GetProperties();
                for (var i = 0; i < source.FieldCount; i++)
                {
                    var convertedFieldName = ConvertFromLowerUnderscoreToCamelCase(source.GetName(i));
                    var property = properties.FirstOrDefault(x => x.Name == convertedFieldName);
                    if (property != null)
                    {
                        var type = property.PropertyType;
                        var convertedValue = GetValueWithConvertedType(type, source[i]);
                        property.SetValue(destination, convertedValue, null);
                    }
                }
                destinationList.Add(destination);
            }
            return destinationList;
        }

        private static object GetValueWithConvertedType(Type type, object value)
        {
            if (type == typeof(int) && value is string)
            {
                return int.Parse((string) value);
            }
            else if (type == typeof (DateTime) && value is string)
            {
                return DateTime.Parse((string)value);
            }
            else
            {
                return value;
            }
        }

        private static string ConvertFromLowerUnderscoreToCamelCase(string value)
        {
            return string.Join("",
                value.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Substring(0, 1).ToUpper() + s.Substring(1))
                    .ToArray());
        }
    }
}