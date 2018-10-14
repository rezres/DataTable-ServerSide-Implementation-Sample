using DataTable_ServerSide__Implementation_Sample.Data.Requests;
using DataTable_ServerSide__Implementation_Sample.Data.Responses;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataTable_ServerSide__Implementation_Sample.Extensions
{
    public static class DataTableHelper
    {

        /// <summary>
        /// Use this function to invoke selector in your options, 
        /// this might be good for performance when dealing with large tables. 
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="data">IQuerable of the data</param>
        /// <param name="option">Datatable Options</param>
        /// <param name="selector">Selector of type X => new object {...} </param>
        /// <returns>Datatable Response for provided DBSet</returns>
        public static async Task<DataTableResponse> GetOptionResponseAsync<T>(this IQueryable<T> data, DataTableOptions option, Expression<Func<T, object>> selector) where T : class
        {
            var countTotal = await data.CountAsync();
            var searchedEntities = SearchEntity<T>(option, data);
            var countFiltered = await searchedEntities.CountAsync();
            var theData = await searchedEntities
                .Skip(option.Start)
                .Take(option.Length)
                .Select(selector)
                .ToListAsync();
            return new DataTableResponse
            {
                Draw = Int32.Parse(option.Draw),
                Data = theData.ToList(),
                RecordsTotal = countTotal,
                RecordsFiltered = countFiltered,
            };
        }

        /// <summary>
        /// Returns DataTable Response, done asynchronously 
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="data">IQuerable of the data</param>
        /// <param name="option">Datatable Options</param>
        /// <returns>Datatable Response for provided DBSet</returns>
        public static async Task<DataTableResponse> GetOptionResponseAsync<T>(this IQueryable<T> data, DataTableOptions option) where T : class
        {
            var countTotal = await data.CountAsync();
            var searchedEntities = SearchEntity<T>(option, data);
            var countFiltered = await searchedEntities.CountAsync();
            var theData = await searchedEntities
                .Skip(option.Start)
                .Take(option.Length)
                .ToListAsync();
            return new DataTableResponse
            {
                Draw = Int32.Parse(option.Draw),
                Data = theData.ToList<object>(),
                RecordsTotal = countTotal,
                RecordsFiltered = countFiltered,
            };
        }

        /// <summary>
        /// Returns DataTable Response 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="option"></param>
        /// <returns>Datatable Response for provided DBSet</returns>
        public static DataTableResponse GetOptionResponse<T>(this IQueryable<T> data, DataTableOptions option) where T : class
        {
            var countTotal = data.Count();
            var searchedEntities = SearchEntity<T>(option, data);
            var countFiltered = searchedEntities.Count();
            var theData = searchedEntities
                .Skip(option.Start)
                .Take(option.Length)
                .ToList();

            return new DataTableResponse
            {
                Draw = Int32.Parse(option.Draw),
                Data = theData.ToList<object>(),
                RecordsTotal = countTotal,
                RecordsFiltered = countFiltered,
            };
        }

        /// <summary>
        /// Returns Iquerable after applying dynamically all search and orders provided by the datatable options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dtOptions"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IQueryable<T> SearchEntity<T>(DataTableOptions dtOptions, IQueryable<T> data) where T : class
        {
            if (dtOptions.Search != null)
            {
                //Perform search filter on all Searchable Coloumns
                if (!string.IsNullOrEmpty(dtOptions.Search.Value))
                {
                    List<Expression<Func<T, bool>>> predicatesSearh = new List<Expression<Func<T, bool>>>();
                    foreach (var col in dtOptions.Columns.Where(t => t.Searchable))
                    {
                        string filterTarget = col.Data;
                        //If Property Not Found, upper the First char, other people might not need this if you explicitly use camel case serializing.
                        if (!typeof(T).GetProperties().Any(t => t.Name == filterTarget))
                        {
                            filterTarget = filterTarget[0].ToString().ToUpper() + filterTarget.Substring(1);
                        }
                        var colPredicate = ExpressionBuilder.BuildPredicate<T>(dtOptions.Search.Value, OperatorComparer.Contains, filterTarget);
                        if (colPredicate != null)
                            predicatesSearh.Add(colPredicate);
                    }
                    if (predicatesSearh.Any())
                    {
                        //Init Expression of type T return bool (search filter)
                        Expression<Func<T, bool>> predicateCompiled = null;
                        //Combine all search conditions
                        foreach (var pre in predicatesSearh)
                            predicateCompiled = predicateCompiled == null ? pre : predicateCompiled.Or(pre);

                        data = data.Where<T>(predicateCompiled);
                    }
                }
            }
            //Perform column sorting
            IOrderedQueryable<T> dataOrder = null;
            if (dtOptions.Order != null)
            {
                var sortQuery = new List<string>();
                foreach (var order in dtOptions.Order)
                {
                    var col = dtOptions.Columns[order.Column];
                    if (col.Orderable)
                    {
                        var filterData = col.Data;
                        sortQuery.Add(filterData + " " + (order.Dir.ToLower().Equals("asc") ? "ASC" : "DESC"));
                    }
                }
                if (sortQuery.Any())
                    data = data.OrderBy(string.Join(",", sortQuery));
            }

            if (dataOrder != null)
                return dataOrder;
            else
                return data;
        }

        /// <summary>
        /// Used to build Expression for the specifications 
        /// </summary>
        /// <typeparam name="T"> The Type </typeparam>
        /// <param name="name"> (the name of the colomn to be filtered)</param>
        /// <param name="value">(the value to be used for the comparision)</param>
        /// <param name="comparer">type of comparer (Equal,Contains, NotEqual)</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> BuildPredicate<T>(string name, string value, OperatorComparer comparer)
        {
            if (typeof(T).GetProperties().Any(t => t.Name == name))
            {
                Expression<Func<T, bool>> predicateCompiled =
                    ExpressionBuilder.BuildPredicate<T>(value, comparer, name);

                return predicateCompiled;
            }
            else
                throw new Exception("Property Not Found, Please check property name used");
        }

        /// <summary>
        /// Serialize Data Table Response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string ToJsonString(this DataTableResponse response)
        {
            return JsonConvert.SerializeObject(response, Formatting.None, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None
            });
        }
    }
}
