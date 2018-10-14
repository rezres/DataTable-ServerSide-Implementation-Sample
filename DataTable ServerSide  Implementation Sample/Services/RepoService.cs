using DataTable_ServerSide__Implementation_Sample.Data.Requests;
using DataTable_ServerSide__Implementation_Sample.Data.Responses;
using DataTable_ServerSide__Implementation_Sample.Extensions;
using Microsoft.EntityFrameworkCore;
using DataTable_ServerSide__Implementation_Sample.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTable_ServerSide__Implementation_Sample.Data.Model;
using System;

namespace DataTable_ServerSide__Implementation_Sample.Services
{
    public class RepoService<T> : IRepo<T> where T : class
    {
        protected ProductContext context;

        public RepoService(ProductContext _context)
        {
            context = _context;
        }

        public async Task<T> Add(T data)
        {
            context.Set<T>().Add(data);
            await context.SaveChangesAsync();
            return data;
        }
        public async Task<IList<T>> AddBulk(IList<T> data)
        {
            context.Set<T>().AddRange(data);
            await context.SaveChangesAsync();
            return data;
        }
        public async Task<T> GetById(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }
        public async Task<List<T>> GetAll()
        {
            var objList = await context.Set<T>().ToListAsync();
            return objList;
        }
        public async Task<IEnumerable<T>> GetBySpecsAsync(ISpecification<T> spec)
        {
            // return the result of the query using the specification's criteria expression
            return await this.context.Set<T>()
                                    .IncludeExpressions(spec.Includes)
                                    .IncludeByNames(spec.IncludeStrings)
                                    .Where(spec.Criteria)
                                    .ToListAsync();
        }
        public async Task<T> GetFirstBySpecsAsync(ISpecification<T> spec)
        {
            return await this.context.Set<T>()
                                    .IncludeExpressions(spec.Includes)
                                    .IncludeByNames(spec.IncludeStrings)
                                    .Where(spec.Criteria)
                                    .FirstOrDefaultAsync();
        }
        public async Task<T> Update(T data)
        {
            context.Entry(data).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();
            return data;
        }
        public async Task Delete(int id)
        {
            var obj = await context.Set<T>().FindAsync(id);
            context.Set<T>().Remove(obj);
            await context.SaveChangesAsync();
        }
        public async Task<DataTableResponse> GetOptionResponse(DataTableOptions options)
        {
            return await context.Set<T>().GetOptionResponseAsync<T>(options);
        }
        public async Task<DataTableResponse> GetOptionResponseWithSpec(DataTableOptions options, ISpecification<T> spec)
        {
            var data = await context.Set<T>()
                                                .IncludeExpressions(spec.Includes)
                                                .IncludeByNames(spec.IncludeStrings)
                                                .GetOptionResponseAsync<T>(options);

            return data;
        }

        public void Dispose()
        {
            //Implement this for non in memory Data base 
            context.Dispose();
        }
    }
}
