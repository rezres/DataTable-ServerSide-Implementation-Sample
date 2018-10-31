using DataTable_ServerSide__Implementation_Sample.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataTable_ServerSide__Implementation_Sample.Services
{
    public sealed class BaseSpecification<T> : ISpecification<T>
    {
        private IList<Expression<Func<T, bool>>> _Criteria;
        private IList<Expression<Func<T, object>>> _Includes;
        private IList<string> _IncludeStrings;
        public BaseSpecification() {
            this._Criteria = new List<Expression<Func<T, bool>>>();
            this._Includes = new List<Expression<Func<T, object>>>();
            this._IncludeStrings = new List<string>();
        }

        IList<Expression<Func<T, bool>>> ISpecification<T>.Criteria => this._Criteria;

        IList<Expression<Func<T, object>>> ISpecification<T>.Includes => this._Includes;

        IList<string> ISpecification<T>.IncludeStrings => this._IncludeStrings;

        public  void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            this._Includes.Add(includeExpression);
        }
        public  void AddInclude(string includeString)
        {
            this._IncludeStrings.Add(includeString);
        }
        public  void AddFilter(Expression<Func<T, bool>> criteria)
        {
            this._Criteria.Add(criteria);
        }
    }
}
