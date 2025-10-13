
namespace Get.UI.Data;
static class TypeExtension
{
    public static string ToReadableString(this Type type)
    {
        if (type.IsGenericType)
        {
            var name = type.Name;
            int typeIndex = name.IndexOf('`');
            string baseType = name[..typeIndex];
            Type[] typeArguments = type.GetGenericArguments();

            string arguments = string.Join(", ", typeArguments.Select(ToReadableString));
            return $"{baseType}<{arguments}>";
        }
        else
        {
            return SimplifyName(type.Name);
        }
    }
    private static string SimplifyName(string typeName)
    {
        return typeName switch
        {
            "Boolean" => "bool",
            "Byte" => "byte",
            "SByte" => "sbyte",
            "Char" => "char",
            "Decimal" => "decimal",
            "Double" => "double",
            "Single" => "float",
            "Int32" => "int",
            "UInt32" => "uint",
            "Int64" => "long",
            "UInt64" => "ulong",
            "Int16" => "short",
            "UInt16" => "ushort",
            "String" => "string",
            _ => typeName,
        };
    }
}
