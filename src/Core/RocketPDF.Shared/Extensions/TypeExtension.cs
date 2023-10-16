using System.Collections.Concurrent;
using System.Reflection;

namespace RocketPDF.Shared.Extensions
{
    public static class TypeExtension
    {
        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static MethodInfo GetMethod(this Type type, string methodName, int pParametersCount = 0, int pGenericArgumentsCount = 0)
        {
            return type
                .GetMethods()
                .Where(m => m.Name == methodName).ToList()
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == pParametersCount
                            && x.Args.Length == pGenericArgumentsCount
                ).Select(x => x.Method)
                .First();
        }

        public static Type UnwrapNullableType(this Type type)
            => Nullable.GetUnderlyingType(type) ?? type;

        public static bool IsNullableValueType(this Type type)
            => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static bool IsNullableType(this Type type)
            => !type.IsValueType || type.IsNullableValueType();

        public static bool IsValidEntityType(this Type type)
            => type is { IsClass: true, IsArray: false };

        public static bool IsNumeric(this Type type)
        {
            type = type.UnwrapNullableType();

            return type.IsInteger()
                || type == typeof(decimal)
                || type == typeof(float)
                || type == typeof(double);
        }

        public static bool IsNullableNumeric(this Type type)
        {
            return type.IsNullableInteger()
                || type == typeof(decimal?)
                || type == typeof(float?)
                || type == typeof(double?);
        }

        public static bool IsInteger(this Type type)
        {
            type = type.UnwrapNullableType();

            return type == typeof(int)
                || type == typeof(long)
                || type == typeof(short)
                || type == typeof(byte)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(ushort)
                || type == typeof(sbyte)
                || type == typeof(char);
        }

        public static bool IsNullableInteger(this Type type)
        {
            return type == typeof(int?)
                || type == typeof(long?)
                || type == typeof(short?)
                || type == typeof(byte?)
                || type == typeof(uint?)
                || type == typeof(ulong?)
                || type == typeof(ushort?)
                || type == typeof(sbyte?)
                || type == typeof(char?);
        }

        public static bool IsSignedInteger(this Type type)
            => type == typeof(int)
                || type == typeof(long)
                || type == typeof(short)
                || type == typeof(sbyte);

        private static readonly ConcurrentDictionary<Type, object> typeDefaults = new();

        public static object? GetDefaultValue(this Type type)
        {
            return type.IsValueType ? typeDefaults.GetOrAdd(type, Activator.CreateInstance!) : null;
        }

        public static object? GetTypeValue(this object value)
        {
            if (value == null) return null;
            var type = value.GetType();
            if (type == typeof(short) || type == typeof(short?))
            {
                return Convert.ToInt16(value);
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                return Convert.ToInt32(value);
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                return Convert.ToInt64(value);
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return Convert.ToDecimal(value);
            }
            if (type == typeof(DateTime))
            {
                return DateTime.Parse(value.ToString()!);
            }
            if (type == typeof(DateOnly))
            {
                return DateOnly.Parse(value.ToString()!);
            }
            if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(value.ToString()!);
            }
            if (type == typeof(TimeOnly))
            {
                return TimeOnly.Parse(value.ToString()!);
            }
            return value;
        }
    }
}