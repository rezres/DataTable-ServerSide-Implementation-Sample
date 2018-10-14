using DataTable_ServerSide__Implementation_Sample.Data.Requests;
using DataTable_ServerSide__Implementation_Sample.Data.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataTable_ServerSide__Implementation_Sample.Interfaces
{
    public interface IRepo<T> : IDisposable  where T : class
    {
        /// <summary>
        /// Add object to the database
        /// </summary>
        /// <param name="data">the object to be added</param>
        /// <returns> the object after insertion</returns>
        Task<T> Add(T data);

        /// <summary>
        /// Add List Of Objects to the database
        /// </summary>
        /// <param name="data">List of type T</param>
        /// <returns>Inserted objects</returns>
        Task<IList<T>> AddBulk(IList<T> data);

        /// <summary>
        /// Get object by id
        /// </summary>
        /// <param name="id">id of the object to be retrieved</param>
        /// <returns>the object</returns>
        Task<T> GetById(int id);
        /// <summary>
        /// Get All Objects for a type T
        /// </summary>
        /// <returns>List of all inserted Objects</returns>
        Task<List<T>> GetAll();
        /// <summary>
        /// Get Objects by Specifications
        /// </summary>
        /// <param name="specs"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetBySpecsAsync(ISpecification<T> specs);
        Task<T> GetFirstBySpecsAsync(ISpecification<T> specs);
        /// <summary>
        /// Update Object
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<T> Update(T data);
        /// <summary>
        /// Delete Object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete (int id);
        /// <summary>
        /// Get Data Table Response, Providing Data Table Options which will be provided by the plugin
        /// </summary>
        /// <param name="options">Data Table Plugin Options, will be provided by the plugin</param>
        /// <returns></returns>
        Task<DataTableResponse> GetOptionResponse(DataTableOptions options);
        /// <summary>
        /// Get Data Table Response, Providing Data Table Options which will be provided by the plugin
        /// Accepts Specification for further preFiltering or Including navigation properties.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="specs"></param>
        /// <returns></returns>
        Task<DataTableResponse> GetOptionResponseWithSpec(DataTableOptions options,ISpecification<T> specs);
    }
}
