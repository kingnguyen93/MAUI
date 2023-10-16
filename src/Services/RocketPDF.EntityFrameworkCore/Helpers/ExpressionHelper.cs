using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using System.Reflection;
using RocketPDF.Shared.Extensions;

namespace RocketPDF.EntityFrameworkCore.Helpers;

public static class ExpressionHelper
{
    internal static readonly MethodInfo PropertyMethod
        = typeof(EF).GetTypeInfo().GetDeclaredMethod(nameof(Property))!;

    internal static readonly MethodInfo GetValueMethod
        = typeof(ValueBuffer).GetRuntimeProperties().Single(p => p.GetIndexParameters().Length > 0).GetMethod!;

    internal static MethodInfo MakePropertyMethod(Type type)
        => PropertyMethod.MakeGenericMethod(type);

    private static readonly MethodInfo ObjectEqualsMethodInfo
        = typeof(object).GetRuntimeMethod(nameof(Equals), new[] { typeof(object), typeof(object) })!;

    internal static Expression CreateEqualsExpression(Expression left, Expression right, bool negated = false)
    {
        var result = Expression.Call(ObjectEqualsMethodInfo, AddConvertToObject(left), AddConvertToObject(right));
        return negated
            ? Expression.Not(result)
            : result;
        static Expression AddConvertToObject(Expression expression)
            => expression.Type.IsValueType
                ? Expression.Convert(expression, typeof(object))
                : expression;
    }

    public static Expression<Func<TEntity, bool>> CreateIdsExpression<TEntity>(IReadOnlyList<IReadOnlyProperty> keyProperties, object[] ids)
    {
        var param = Expression.Parameter(typeof(TEntity), "e");
        var constant = Expression.Constant(new ValueBuffer(ids));
        var body = GenerateEqualExpression(param, constant, keyProperties[0], 0);
        for (var i = 1; i < keyProperties.Count; i++)
        {
            body = Expression.AndAlso(body, GenerateEqualExpression(param, constant, keyProperties[i], i));
        }
        return Expression.Lambda<Func<TEntity, bool>>(body, param);
        static Expression GenerateEqualExpression(Expression entityParameterExpression, Expression keyValuesConstantExpression, IReadOnlyProperty property, int i)
            => property.ClrType.IsValueType
                && property.ClrType.UnwrapNullableType() is Type nonNullableType
                && !(nonNullableType == typeof(bool) || nonNullableType.IsNumeric() || nonNullableType.IsEnum)
                    ? CreateEqualsExpression(
                        Expression.Call(
                            MakePropertyMethod(typeof(object)),
                            entityParameterExpression,
                            Expression.Constant(property.Name, typeof(string))),
                        Expression.Call(
                            keyValuesConstantExpression,
                            GetValueMethod,
                            Expression.Constant(i)))
                    : Expression.Equal(
                        Expression.Call(
                            MakePropertyMethod(property.ClrType),
                            entityParameterExpression,
                            Expression.Constant(property.Name, typeof(string))),
                        Expression.Convert(
                            Expression.Call(
                                keyValuesConstantExpression,
                                GetValueMethod,
                                Expression.Constant(i)),
                            property.ClrType));
    }

    public static Expression<Func<TEntity, bool>> CreateEqualExpression<TEntity>(string propertyName, object? value)
    {
        var param = Expression.Parameter(typeof(TEntity), "e");
        var member = Expression.Property(param, propertyName);
        var constant = Expression.Constant(value);
        var body = Expression.Equal(member, constant);
        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }

    public static Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>(IEnumerable<KeyValuePair<string, object?>> filters)
    {
        var param = Expression.Parameter(typeof(TEntity), "e");
        Expression? body = null;
        foreach (var pair in filters)
        {
            var member = Expression.Property(param, pair.Key);
            var constant = Expression.Constant(pair.Value);
            var expression = member.Type.IsValueType
                    && member.Type.UnwrapNullableType() is Type nonNullableType
                    && !(nonNullableType == typeof(bool) || nonNullableType.IsNumeric() || nonNullableType.IsEnum)
                        ? Expression.Equal(member, constant)
                        : Expression.Equal(member, Expression.Convert(constant, member.Type));
            body = body == null ? expression : Expression.AndAlso(body, expression);
        }
        return Expression.Lambda<Func<TEntity, bool>>(body!, param);
    }

    public static Expression<Func<TEntity, bool>> CreateNullEqualExpression<TEntity>(string propertyName, object? value)
    {
        var param = Expression.Parameter(typeof(TEntity), "e");
        var member = Expression.Property(param, propertyName);
        var nullCheck = Expression.Equal(member, Expression.Constant(null));
        var constant = Expression.Constant(value);
        var body = Expression.Equal(member, constant);
        var condition = Expression.Condition(nullCheck, Expression.Constant(false), body);
        return Expression.Lambda<Func<TEntity, bool>>(condition, param);
    }

    public static Expression<Func<TEntity, bool>> CreateTypeEqualExpression<TEntity, TProperty>(string propertyName, object? value)
    {
        var param = Expression.Parameter(typeof(TEntity), "e");
        var member = Expression.Property(param, propertyName);
        var typeCheck = Expression.TypeEqual(Expression.Constant(value), typeof(TProperty));
        var constant = Expression.Constant(value, typeof(TProperty));
        var body = Expression.Equal(member, constant);
        var condition = Expression.Condition(typeCheck, body, Expression.Constant(false));
        return Expression.Lambda<Func<TEntity, bool>>(condition, param);
    }

    public static Expression<Func<TEntity, bool>> CreateContainsExpression<TEntity>(string propertyName, object? value)
    {
        var param = Expression.Parameter(typeof(TEntity), "e");
        var member = Expression.Property(param, propertyName);
        var constant = Expression.Constant(value);
        var body = Expression.Call(member, "Contains", Type.EmptyTypes, constant);
        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }

    public static Expression<Func<TEntity, bool>> CreateInExpression<TEntity>(string propertyName, object? value)
    {
        var param = Expression.Parameter(typeof(TEntity), "e");
        var member = Expression.Property(param, propertyName);
        var propertyType = ((PropertyInfo)member.Member).PropertyType;
        var constant = Expression.Constant(value);
        var body = Expression.Call(typeof(Enumerable), "Contains", new[] { propertyType }, constant, member);
        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }

    public static Expression<Func<TEntity, bool>> CreateNestedExpression<TEntity>(string propertyName, object? value)
    {
        var param = Expression.Parameter(typeof(TEntity), "e");
        Expression member = param;
        foreach (var namePart in propertyName.Split('.'))
        {
            member = Expression.Property(member, propertyName);
        }
        var constant = Expression.Constant(value);
        var body = Expression.Equal(member, constant);
        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }

    public static Expression<Func<TEntity, bool>> CreateBetweenExpression<TEntity>(string propertyName, object? lowerValue, object? upperValue)
    {
        var param = Expression.Parameter(typeof(TEntity), "e");
        var member = Expression.Property(param, propertyName);
        var body = Expression.AndAlso(
            Expression.GreaterThanOrEqual(member, Expression.Constant(lowerValue)),
            Expression.LessThanOrEqual(member, Expression.Constant(upperValue))
        );
        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }

    private static readonly Dictionary<int, Delegate> Cache = new();

    public static Func<T, bool> GetPredicate<T>(Expression<Func<T, bool>> expression)
    {
        var key = expression.GetHashCode();
        if (Cache.TryGetValue(key, out var cachedDelegate))
        {
            return (Func<T, bool>)cachedDelegate;
        }
        var compiledDelegate = expression.Compile();
        Cache[key] = compiledDelegate;
        return compiledDelegate;
    }
}