using System.ComponentModel;
using System.Reflection;

namespace Library.API.Extensions;

public static class EnumExtensions
{
    public static string ToDescription(this Enum value)
    {
        FieldInfo? info = value.GetType().GetField(value.ToString());
        if (info == null)
        {
            return value.ToString();
        }
        var attribute = Attribute.GetCustomAttribute(info, typeof(DescriptionAttribute))
            as DescriptionAttribute;

        return attribute?.Description ?? value.ToString();
    }
}