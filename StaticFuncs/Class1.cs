using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace StaticFuncs
{
    public static class StaticFuncs
    {
        public static string GetEnumDescription(Enum source, object separator = null)
        {
            if (source == null) return string.Empty;
            Type enumType = source.GetType();
            List<string> result = new List<string>();
            if (enumType.GetCustomAttributes<FlagsAttribute>().Any())
            {
                if (System.Convert.ToInt32(source) == 0)
                {
                    FieldInfo fieldInfo = enumType.GetField(source.ToString());
                    object[] attribArray = fieldInfo.GetCustomAttributes(false);
                    if (attribArray.Length != 0 && attribArray[0] is DescriptionAttribute description)
                    {
                        return description.Description;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                IEnumerable<Enum> enumValues = Enum.GetValues(enumType).Cast<Enum>();
                foreach (Enum enumValue in enumValues)
                {
                    if (System.Convert.ToInt32(enumValue) != 0 && source.HasFlag(enumValue))
                    {
                        FieldInfo fieldInfo = enumType.GetField(enumValue.ToString());
                        object[] attribArray = fieldInfo.GetCustomAttributes(false);
                        if (attribArray.Length != 0 && attribArray[0] is DescriptionAttribute description)
                        {
                            result.Add(description.Description);
                        }

                    }
                }
            }
            else
            {
                FieldInfo fieldInfo = enumType.GetField(source.ToString());
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                if (attribArray.Length != 0 && attribArray[0] is DescriptionAttribute description)
                {
                    result.Add(description.Description);
                }
            }


            separator ??= ", ";
            return string.Join(separator.ToString(), result);
        }

    }
}
