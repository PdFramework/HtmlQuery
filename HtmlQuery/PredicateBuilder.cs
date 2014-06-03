namespace PeinearyDevelopment.Web.HtmlQuery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static class PredicateBuilder
    {
        public enum StringComparison
        {
            Contains,
            EndsWith,
            Equals,
            NotEquals,
            StartsWith
        }

        public enum StringTransform
        {
            None,
            ToLower,
            ToLowerInvariant,
            ToUpper,
            ToUpperInvariant
        }

        public enum EnumerableMethodName
        {
            All,
            Any,
            Where
        }

        public static Expression CreateStringPropertyCompareExpression(this Expression keyValueParm, string keyCompareValue, string propertyValue, StringTransform keyTransform, StringComparison keyComparison)
        {
            var keyProp = Expression.Property(keyValueParm, propertyValue);
            var keyConst = Expression.Constant(keyCompareValue, typeof(string));
            return keyProp.CreateStringCompare(keyTransform, keyComparison, keyConst);
        }

        public static Expression CreateStringPropertyCompareExpression<TValue>(this Expression keyValueParm, TValue keyCompareValue, Expression property, StringTransform keyTransform, StringComparison keyComparison)
        {
            var keyConst = Expression.Constant(keyCompareValue, typeof(TValue));
            return keyComparison == StringComparison.NotEquals ? Expression.Not(property.CreateStringCompare(keyTransform, keyComparison, keyConst)) : property.CreateStringCompare(keyTransform, keyComparison, keyConst);
        }

        public static Expression CreateKeyValueBoolExpression<TValue>(this Expression keyValueParm, ParameterExpression valueParam, Expression valueExpression, EnumerableMethodName enumerableMethodName)
        {
            var valueType = typeof(TValue);
            var valueProp = Expression.Property(keyValueParm, "Value");
            var valueLamda = Expression.Lambda<Func<TValue, bool>>(valueExpression, new[] { valueParam });

            var nullConst = Expression.Constant(null);
            var notEqualNullExpr = Expression.NotEqual(valueProp, nullConst);
            return Expression.AndAlso(notEqualNullExpr, Expression.Call(typeof(Enumerable), enumerableMethodName.ToString(), new[] { valueType }, valueProp, valueLamda));
        }

        public static Expression CreateKeyValueAnyExpression(this Expression valueParam, EnumerableMethodName enumerableMethodName, string propertyName, Expression boolExpression, ParameterExpression keyValueParm)
        {
            var enumerableProperty = Expression.Property(valueParam, propertyName);
            var valueLamda = Expression.Lambda<Func<KeyValuePair<string, IEnumerable<string>>, bool>>(boolExpression, new[] { keyValueParm });

            var nullConst = Expression.Constant(null);
            var notEqualNullExpr = Expression.NotEqual(enumerableProperty, nullConst);
            return Expression.AndAlso(notEqualNullExpr, Expression.Call(typeof(Enumerable), enumerableMethodName.ToString(), new[] { typeof(KeyValuePair<string, IEnumerable<string>>) }, enumerableProperty, valueLamda));
        }

        public static Expression CreateAnyExpression(this Expression elementParm)
        {
            var parm = Expression.Property(elementParm, "Attributes");
            return Expression.Call(typeof(Enumerable), EnumerableMethodName.Any.ToString(), new[] { typeof(KeyValuePair<string, IEnumerable<string>>) }, parm);
        }

        //public static Expression CreateKeyAnyExpression(this Expression valueParam, EnumerableMethodName enumerableMethodName, string propertyName, Expression boolExpression, ParameterExpression keyValueParm)
        //{
        //    var enumerableProperty = Expression.Property(valueParam, "Attributes");
        //    var valueLamda = Expression.Lambda<Func<Dictionary<string, IEnumerable<string>>, bool>>(boolExpression, new[] { keyValueParm });

        //    var nullConst = Expression.Constant(null);
        //    var notEqualNullExpr = Expression.NotEqual(enumerableProperty, nullConst);
        //    return Expression.AndAlso(notEqualNullExpr, Expression.Call(typeof(Enumerable), enumerableMethodName.ToString(), new[] { typeof(KeyValuePair<string, IEnumerable<string>>) }, enumerableProperty, valueLamda));
        //}

        public static Expression CreateElementsWhereExpression<TInput>(this ParameterExpression valueParam, EnumerableMethodName enumerableMethodName, string propertyName, Expression boolExpression)
        {
            var enumerableProperty = Expression.Property(valueParam, propertyName);
            var valueLamda = Expression.Lambda<Func<TInput, bool>>(boolExpression, new[] { valueParam });
            return Expression.Call(typeof(Enumerable), enumerableMethodName.ToString(), new[] { typeof(TInput) }, enumerableProperty, valueLamda);
        }

        public static Expression CreateStringCompare(this Expression leftExpression, StringTransform stringTransform, StringComparison stringComparison, Expression rightExpression)
        {
            var stringType = typeof(string);
            var stringTransformMethod = stringType.GetMethod(stringTransform.ToString(), Type.EmptyTypes);

            var leftParam = leftExpression == null
                                ? stringTransform == StringTransform.None
                                        ? (Expression)Expression.Parameter(stringType)
                                        : Expression.Call(
                                            Expression.Parameter(stringType),
                                            stringTransformMethod)
                                : stringTransform == StringTransform.None
                                        ? leftExpression
                                        : Expression.Call(
                                            leftExpression,
                                            stringTransformMethod);

            var rightParam = stringTransform == StringTransform.None ? rightExpression : Expression.Call(rightExpression, stringTransformMethod);

            var stringCompareMethod = stringType.GetMethod(stringComparison.ToString(), new[] { stringType });
            return Expression.Call(leftParam, stringCompareMethod, rightParam);
        }
    }
}