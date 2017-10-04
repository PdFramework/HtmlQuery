namespace PeinearyDevelopment.Framework.HtmlQuery
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class QueryGenerator
    {
        public IEnumerable<Func<DomElement, IEnumerable<DomElement>>> CreateHtmlFuncs(string htmlQueryString)
        {
            var elementParam = Expression.Parameter(typeof(DomElement), "HtmlQueryDomElement");
            var keyValueParam = Expression.Parameter(typeof(KeyValuePair<string, IEnumerable<string>>), "HtmlQueryAttribute");
            var valueParam = Expression.Parameter(typeof(string), "HtmlQueryAttributeValue");

            var expressions = ParseHtmlQuery(elementParam, keyValueParam, valueParam, htmlQueryString);
            foreach (var expression in expressions)
            {
                var expression2 = expression.ToString().Contains("HtmlQueryAttribute")
                                 ? elementParam.CreateKeyValueAnyExpression(
                                     PredicateBuilder.EnumerableMethodName.Any,
                                     "Attributes",
                                     expression,
                                     keyValueParam)
                                 : expression;

                var ee =
                    elementParam.CreateElementsWhereExpression<DomElement>(
                        PredicateBuilder.EnumerableMethodName.Where,
                        "Descendants",
                        expression2);

                yield return Expression.Lambda<Func<DomElement, IEnumerable<DomElement>>>(ee, new[] { elementParam }).Compile(); // DebugInfoGenerator.CreatePdbGenerator());
            }
        }
 
        public IEnumerable<Expression> ParseHtmlQuery(ParameterExpression elementParm, ParameterExpression keyValueParm, ParameterExpression valueParm, string htmlQueryString)
        {
            var expressions = new List<Expression>();
            Expression expression = null;
            var builder = new StringBuilder();
            using (var stream = new MemoryStream(Encoding.Default.GetBytes(htmlQueryString ?? string.Empty)))
            {
                using (var reader = new StreamReader(stream))
                {
                    while (reader.Peek() != -1)
                    {
                        if (reader.Peek() == '[')
                        {
                            reader.Read();
                            var str = GetNestedElement(reader, new[] { ']' });
                            var delimitors = new[] { "*=", "~=", "^=", "|=", "$=", "!=", "=" };
                            var values = new List<string>();

                            if (delimitors.Any(str.Contains))
                            {
                                var delimitor = delimitors.First(str.Contains);
                                values.AddRange(str.Split(new[] { delimitor }, StringSplitOptions.RemoveEmptyEntries));
                                values = values.Select(v => v.Trim()).ToList();
                                values[1] = values[1].Substring(1, values[1].Length - 2); // removes parenthesis surrounding value
                                values.Add(delimitor);
                            } 

                            if (!values.Any())
                            {
                                expression = keyValueParm.CreateStringPropertyCompareExpression(str, "Key", PredicateBuilder.StringTransform.ToLowerInvariant, PredicateBuilder.StringComparison.Equals);
                            }
                            else
                            {
                                var keyExpression = keyValueParm.CreateStringPropertyCompareExpression(values.First(), "Key", PredicateBuilder.StringTransform.ToLowerInvariant, PredicateBuilder.StringComparison.Equals);
                                Expression valueExpression;
                                var enumerableMethodName = PredicateBuilder.EnumerableMethodName.Any;
                                if (values.Last() == "|=")
                                {
                                    var valueExpression1 = keyValueParm.CreateStringPropertyCompareExpression(values[1], valueParm, PredicateBuilder.StringTransform.None, PredicateBuilder.StringComparison.Equals);
                                    var valueExpression2 = keyValueParm.CreateStringPropertyCompareExpression(values[1] + "-", valueParm, PredicateBuilder.StringTransform.None, PredicateBuilder.StringComparison.StartsWith);
                                    valueExpression = Expression.OrElse(valueExpression1, valueExpression2);
                                }
                                else
                                {
                                    var stringComparison = PredicateBuilder.StringComparison.Equals;
                                    switch (values.Last())
                                    {
                                        case "!=":
                                            stringComparison = PredicateBuilder.StringComparison.NotEquals;
                                            break;
                                        case "$=":
                                            stringComparison = PredicateBuilder.StringComparison.EndsWith;
                                            break;
                                        case "^=":
                                            stringComparison = PredicateBuilder.StringComparison.StartsWith;
                                            break;
                                        case "*=":
                                            stringComparison = PredicateBuilder.StringComparison.Contains;
                                            break;
                                        case "~=":
                                            stringComparison = PredicateBuilder.StringComparison.Equals;
                                            break;
                                    }

                                    if(stringComparison == PredicateBuilder.StringComparison.NotEquals)
                                    {
                                        valueExpression = Expression.Not(keyValueParm.CreateStringPropertyCompareExpression(values[1], valueParm, PredicateBuilder.StringTransform.None, PredicateBuilder.StringComparison.Equals));
                                    }
                                    else
                                    {
                                        valueExpression = keyValueParm.CreateStringPropertyCompareExpression(values[1], valueParm, PredicateBuilder.StringTransform.None, stringComparison);
                                    }
                                }

                                if (values.Last() == "!=")
                                {
                                    var valuesExpression = keyValueParm.CreateKeyValueBoolExpression<string>(valueParm, valueExpression, enumerableMethodName);
                                    var expression2 = Expression.OrElse(Expression.Not(keyExpression), valuesExpression);
                                    expression = Expression.OrElse(Expression.Not(elementParm.CreateAnyExpression()), expression2);
                                }
                                else
                                {
                                    var valuesExpression = keyValueParm.CreateKeyValueBoolExpression<string>(valueParm, valueExpression, enumerableMethodName);
                                    expression = Expression.AndAlso(keyExpression, valuesExpression);
                                }
                            }
                        }
                        else if (reader.Peek() == ':')
                        {
                            var str = GetNestedElement(reader, new[] { ' ', ',', '.', '>', '+', '~', '#' });
                        }
                        else if (reader.Peek() == ',')
                        {
                            reader.Read();
                            expression = builder.ToString().Trim().Length > 0 ? CreateStringPropertyCompareExpressionHelper(builder, elementParm, keyValueParm, valueParm) : expression;
                            builder = new StringBuilder();
                            var expression1 = ParseHtmlQuery(elementParm, keyValueParm, valueParm, reader.ReadToEnd());
                            expression = Expression.OrElse(expression, expression1.First());
                        }
                        else if (reader.Peek() == ' ')
                        {
                            var delimitors = new[] { '*', '~', '^', '|', '$', '!', '=', ' ', ',', '.', '>', '+', '~', '#' };
                            reader.Read();
 
                            if (!delimitors.Any(d => d == reader.Peek()))
                            {
                                if (builder.ToString().Trim().Length > 0)
                                {
                                    expression = CreateStringPropertyCompareExpressionHelper(builder, elementParm, keyValueParm, valueParm);
                                    builder = new StringBuilder();
                                }
 
                                expressions.Add(expression);
                                expression = ParseHtmlQuery(elementParm, keyValueParm, valueParm, reader.ReadToEnd()).First();
                            }
                        }
                        else
                        {
                            builder.Append((char)reader.Read());
                        }
                    }
 
                    if (builder.ToString().Trim().Length > 0)
                    {
                        expression = CreateStringPropertyCompareExpressionHelper(builder, elementParm, keyValueParm, valueParm);
                    }
                }
            }
 
            expressions.Add(expression);
            return expressions;
        }
 
        public Expression CreateStringPropertyCompareExpressionHelper(StringBuilder builder, ParameterExpression elementParm, ParameterExpression keyValueParm, ParameterExpression valueParm)
        {
            var str = builder.ToString().Trim();
            if (!str.StartsWith(".") && !str.StartsWith("#"))
            {
                return elementParm.CreateStringPropertyCompareExpression(builder.ToString(), "TagName", PredicateBuilder.StringTransform.ToLowerInvariant, PredicateBuilder.StringComparison.Equals);
            }
 
            var keyCompareValue = string.Empty;
            if (str.StartsWith("#"))
            {
                keyCompareValue = "id";
            }
            else if (str.StartsWith("."))
            {
                keyCompareValue = "class";
            }
 
            var keyCompareExpression = keyValueParm.CreateStringPropertyCompareExpression(
                            keyCompareValue,
                            "Key",
                            PredicateBuilder.StringTransform.ToLowerInvariant,
                            PredicateBuilder.StringComparison.Equals);
 
            var valueCompareExpression = keyValueParm.CreateStringPropertyCompareExpression(
                str.Substring(1),
                valueParm,
                PredicateBuilder.StringTransform.ToLowerInvariant,
                PredicateBuilder.StringComparison.Equals);
 
            var valuesCompareExpression = keyValueParm.CreateKeyValueBoolExpression<string>(
                valueParm,
                valueCompareExpression,
                PredicateBuilder.EnumerableMethodName.Any);
 
            return Expression.AndAlso(keyCompareExpression, valuesCompareExpression);
        }
 
        public string GetNestedElement(TextReader reader, IEnumerable<char> delimiters)
        {
            var builder = new StringBuilder();
            while (reader.Peek() != -1 && delimiters.Any(delimiter => reader.Peek() != delimiter))
            {
                // handles escape sequences
                if (reader.Peek() == '\\') 
                {
                    reader.Read();
                }
 
                builder.Append((char)reader.Read());
            }
 
            if (reader.Peek() != -1)
            {
                reader.Read();
            }
            
            return builder.ToString().Trim();
        }
    }
}