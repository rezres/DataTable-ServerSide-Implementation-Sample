using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using DataTable_ServerSide__Implementation_Sample.Interfaces;

namespace DataTable_ServerSide__Implementation_Sample.Extensions
{
    public static class IQueryableHelper
    {
        /// <summary>
        /// Order by Property Name, used to build dynamic ordered IQueryable 
        /// Read More @ https://stackoverflow.com/questions/34899933/sorting-using-property-name-as-string
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string propertyName)
        {
            // LAMBDA: x => x.[PropertyName]
            var parameter = Expression.Parameter(typeof(TSource), "x");
            var splitPropertyName = propertyName.Split('.');
            MemberExpression selector = null;
            Expression property = parameter;
            foreach (var x in splitPropertyName)
            {
                selector = Expression.Property(property, x.Split(' ')[0]);
                property = selector;
            }
            var lambda = Expression.Lambda(property, parameter);
            string orderdir = propertyName.Split(' ')[1].Equals("ASC", StringComparison.InvariantCultureIgnoreCase) ? "OrderBy" : "OrderByDescending";
            // REFLECTION: source.OrderBy(x => x.Property)
            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == orderdir && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(TSource), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<TSource>)result;
        }


        public static IQueryable<T> ProcessSpecification<T>(this IQueryable<T> Set, ISpecification<T> specification) where T : class
        {

            return    Set.IncludeExpressions(specification.Includes)
                        .IncludeByNames(specification.IncludeStrings)
                        .FilterExpressions(specification.Criteria);
        }


        public static IQueryable<T> IncludeExpressions<T>(this IQueryable<T> Set, IList<Expression<Func<T, object>>> IncludeExpression) where T : class
        {
            if (IncludeExpression.Count < 1)
                return Set;
            return IncludeExpression
                .Aggregate(Set, (current, include) => current.Include(include));
        }


        public static IQueryable<T> IncludeByNames<T>(this IQueryable<T> Set, IList<string> IncludeStrings) where T : class
        {
            if (IncludeStrings.Count < 1)
                return Set;
            return IncludeStrings
                .Aggregate(Set, (current, include) => current.Include(include));
        }


        public static IQueryable<T> FilterExpressions<T>(this IQueryable<T> Set, IList<Expression<Func<T,bool>>> filters) where T : class
        {
            if (filters.Count < 1)
                return Set;

            var filterPredicate = filters.Aggregate((current, set) => current.Or(set));

            return Set.Where(filterPredicate);
        }

    }
}
