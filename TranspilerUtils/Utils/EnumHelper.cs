using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerUtils.Utils
{
    public class EnumHelper
    {
        public static IList<KeyValuePair<string, Enum>> GetEnumDescriptions(Type enumType)
        {
            return
                Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(x => new KeyValuePair<string, Enum>(GetEnumDescription(x), x))
                .ToList();
        }

        private static string GetEnumDescription(Enum value)
        {
            var fieldInfoes = value.GetType().GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[])fieldInfoes.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
