using DataTable_ServerSide__Implementation_Sample.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataTable_ServerSide__Implementation_Sample.Services
{
    public class BaseSpecification<T> : ISpecification<T>
    {

        //Init with Include Expressions
        //Ex new BaseSpecification(e => e.NavigationProperty)
        public BaseSpecification(params Expression<Func<T, object>>[] includes)
        {
            this.Includes.AddRange(includes);
        }
        //Include with Filtering Condition
        //Ex new BaseSpecification(e => e.ID == 1)
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        //Include with both Search Expression and Include Expressions
        //Ex new BaseSpecification(e => e.ID == 1, e => e.NavigationProperty1,i => i.NavigationProperty2)
        public BaseSpecification(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            Criteria = criteria;
            this.Includes.AddRange(includes);
        }
        public Expression<Func<T, bool>> Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();

        public virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        public virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
    }
}
