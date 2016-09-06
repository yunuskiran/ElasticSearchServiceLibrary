using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using ElasticSearchServiceLibrary;

namespace ElasticSearchServiceLibrary
{
    public interface IElasticSearchServiceLibrary
    {
        void Insert<T>(T model) where T : class;

        void Delete<T>(DocumentPath<T> documentPath, Func<DeleteDescriptor<T>, DeleteDescriptor<T>> selector) where T : class;

        void Update<T>(DocumentPath<T> documentPath, Func<UpdateDescriptor<T, T>, UpdateDescriptor<T, T>> selector) where T : class;

        Result<IEnumerable<T>> Find<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> termQuery, string groupName="") where T : class;

        void DeleteByQuery<T>(IDeleteByQueryRequest<T> deleteByQuerySelector) where T : class;

        void DeleteBulk<T>(IEnumerable<T> objects) where T : class;

        List<string> Suggest<T>(Func<SuggestDescriptor<T>, ISuggestRequest> selector, string suggestionName) where T : class;

        Task InsertAsync<T>(T model) where T : class;

        Task DeleteAsync<T>(DocumentPath<T> documentPath, Func<DeleteDescriptor<T>, DeleteDescriptor<T>> selector) where T : class;

        Task UpdateAsync<T>(DocumentPath<T> documentPath, Func<UpdateDescriptor<T, T>, UpdateDescriptor<T, T>> selector) where T : class;

        Task<Result<IEnumerable<T>>> FindAsync<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> termQuery, string groupName="") where T : class;

        Task DeleteByQueryAsync<T>(IDeleteByQueryRequest<T> deleteByQuerySelector) where T : class;

        Task DeleteBulkAsync<T>(IEnumerable<T> objects) where T : class;

        Task<List<string>> SuggestAsync<T>(Func<SuggestDescriptor<T>, ISuggestRequest> selector, string suggestionName) where T : class;
    }
}
