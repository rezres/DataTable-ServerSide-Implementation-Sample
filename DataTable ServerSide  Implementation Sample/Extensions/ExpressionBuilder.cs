using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataTable_ServerSide__Implementation_Sample.Extensions
{

    /// <summary>
    /// 
    /// </summary>
    public static class ExpressionBuilder
    {
        /// <summary>
        /// Build Search Predicate for property 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Search Value </param>
        /// <param name="comparer">Comparision Operator </param>
        /// <param name="property">Property Name </param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> BuildPredicate<T>(object value, OperatorComparer comparer, string property)
        {
            var parameterExpression = Expression.Parameter(typeof(T), typeof(T).Name);
            return (Expression<Func<T, bool>>)BuildCondition(parameterExpression, property, comparer, value);
        }

        private static Expression BuildSubQuery(Expression parameter, Type childType, Expression predicate)
        {
            var anyMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Length == 2);
            anyMethod = anyMethod.MakeGenericMethod(childType);
            predicate = Expression.Call(anyMethod, parameter, predicate);
            return MakeLambda(parameter, predicate);
        }

        /// <summary>
        /// Build Search Condition 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="property"></param>
        /// <param name="comparer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Expression BuildCondition(Expression parameter, string property, OperatorComparer comparer, object value)
        {

            Expression predicate = null;
            MemberExpression left = null;

            var childProperty = parameter.Type.GetProperty(property);
            if (childProperty == null)
            {
                if (property.Split('.').Count() > 1)
                {
                    var splitPropertyName = property.Split('.');

                    MemberExpression selector = null;
                    Expression propertyExp = parameter;
                    foreach (var x in splitPropertyName)
                    {
                        selector = Expression.Property(propertyExp, x.Split(' ')[0]);
                        propertyExp = selector;
                    }
                    left = selector;
                }
                else
                    return null;
            }
            else
            {
                left = Expression.Property(parameter, childProperty);
            }



            var right = Expression.Constant(value);

            bool isNumericSearch = long.TryParse(value.ToString(), out long testNumeric);

            var isNumericField = new string[] { "Int64", "Int32", "Int16", "Byte" }.Contains(left.Type.Name);

            //if it is numeric, ignore operator, always set to equals, other operator are not performance wise with numeric
            if (isNumericSearch && isNumericField)
            {
                if (left.Type.Name.Equals("Int64"))
                    right = Expression.Constant(Convert.ToInt64(value));
                else if (left.Type.Name.Equals("Int32"))
                    right = Expression.Constant(Convert.ToInt32(value));
                else if (left.Type.Name.Equals("Int16") || left.Type.Name.Equals("Byte"))
                    right = Expression.Constant(Convert.ToInt16(value));
                if (comparer == OperatorComparer.Contains)
                    predicate = Expression.MakeBinary((ExpressionType)OperatorComparer.Equals, left, right);
                else
                    predicate = Expression.MakeBinary((ExpressionType)comparer, left, right);
            }

            else if (!isNumericSearch && isNumericField)
            {
                //Skip building comparison for numeric field when the search string is not numeric
                return null;
            }
            else if (isNumericSearch && !isNumericField)
            {
                // Sometimes the field is not detected as numeric but data is a numeric type
                right = Expression.Constant(Convert.ToString(value));
                predicate = BuildComparsion(left, comparer, right);
            }
            else
            {
                if (left.Type.Name.Equals("DateTime"))
                    return null;
                predicate = BuildComparsion(left, comparer, right);
            }

            return MakeLambda(parameter, predicate);
        }

        private static Expression BuildComparsion(Expression left, OperatorComparer comparer, Expression right)
        {
            var stringOperatorMask = new List<OperatorComparer>{
                OperatorComparer.Contains,
                OperatorComparer.StartsWith,
                OperatorComparer.EndsWith
            };

            if (stringOperatorMask.Contains(comparer) && left.Type != typeof(string))
                comparer = OperatorComparer.Equals;

            if (!stringOperatorMask.Contains(comparer))
                return Expression.MakeBinary((ExpressionType)comparer, left, Expression.ConvertChecked(right, left.Type));

            return BuildStringCondition(left, comparer, right);
        }


        private static Expression BuildStringCondition(Expression left, OperatorComparer comparer, Expression right)
        {
            var compareMethod = typeof(string).GetMethods()
                .Single(m => m.Name.Equals(Enum.GetName(typeof(OperatorComparer), comparer)) 
                && m.GetParameters().Any(t => t.ParameterType == typeof(string))
                && m.GetParameters().Count() == 1);

            //we assume ignoreCase, so call ToLower on paramter and memberexpression
            var toLowerMethod = typeof(string).GetMethods().Single(m => m.Name.Equals("ToLower") && m.GetParameters().Count() == 0);
            left = Expression.Call(left, toLowerMethod);
            right = Expression.Call(right, toLowerMethod);
            return Expression.Call(left, compareMethod, right);
        }

        private static Expression MakeLambda(Expression parameter, Expression predicate)
        {
            var resultParameterVisitor = new ParameterVisitor();
            resultParameterVisitor.Visit(parameter);
            var resultParameter = resultParameterVisitor.Parameter;
            return Expression.Lambda(predicate, (ParameterExpression)resultParameter);
        }

        private class ParameterVisitor : ExpressionVisitor
        {
            public Expression Parameter
            {
                get;
                private set;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                Parameter = node;
                return node;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="merge"></param>
        /// <returns></returns>
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (map.TryGetValue(p, out ParameterExpression replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }

    public enum OperatorComparer
    {
        /// <summary>
        /// 
        /// </summary>
        Contains,
        /// <summary>
        /// 
        /// </summary>
        StartsWith,
        /// <summary>
        /// 
        /// </summary>
        EndsWith,
        /// <summary>
        /// 
        /// </summary>
        Equals = ExpressionType.Equal,
        /// <summary>
        /// 
        /// </summary>
        GreaterThan = ExpressionType.GreaterThan,
        /// <summary>
        /// 
        /// </summary>
        GreaterThanOrEqual = ExpressionType.GreaterThanOrEqual,
        /// <summary>
        /// 
        /// </summary>
        LessThan = ExpressionType.LessThan,
        ///
        LessThanOrEqual = ExpressionType.LessThanOrEqual,
        /// <summary>
        /// 
        /// </summary>
        NotEqual = ExpressionType.NotEqual
    }


}
