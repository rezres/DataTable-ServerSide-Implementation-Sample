using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataTable_ServerSide__Implementation_Sample.Interfaces
{

    public interface ISpecification<T>
    {
        IList<Expression<Func<T, bool>>> Criteria { get; }
        IList<Expression<Func<T, object>>> Includes { get; }
        IList<string> IncludeStrings { get; }

        void AddInclude(Expression<Func<T, object>> includeExpression);
        void AddInclude(string includeString);
        void AddFilter(Expression<Func<T, bool>> criteria);
    }
}

