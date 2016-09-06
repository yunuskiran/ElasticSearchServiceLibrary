using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace ElasticSearchServiceLibrary
{
    public class ElasticSearchServiceLibrary<TModel>
        : IElasticSearchServiceLibrary where TModel : class
    {
        public static ConnectionSettings _settings;
        public static ElasticClient _client;

        public ElasticSearchServiceLibrary()
        {

        }

        public ElasticSearchServiceLibrary(string indexName, string node, int numberOfReplicas = 5, int numberOfShards = 5)
        {
            var uri = new Uri(node);
            var settings = new ConnectionSettings(uri);
            settings.DefaultIndex(indexName);

            var indexSettings = new IndexSettings();
            indexSettings.NumberOfReplicas = numberOfReplicas;
            indexSettings.NumberOfShards = numberOfShards;

            _client = new ElasticClient(settings);
            var indexDescriptor = new CreateIndexDescriptor(indexName).Mappings(ms => ms.Map<TModel>(m => m.AutoMap()));

            var request = new IndexExistsRequest(indexName);
            if (!_client.IndexExists(request).Exists)
                _client.CreateIndex(indexName, i => indexDescriptor);
        }

        public void Insert<T>(T model)
            where T : class
        {
            _client.Index(model);
        }

        public void Delete<T>(DocumentPath<T> documentPath, Func<DeleteDescriptor<T>, DeleteDescriptor<T>> selector)
            where T : class
        {
            _client.Delete<T>(documentPath, selector);
        }

        public void Update<T>(DocumentPath<T> documentPath, Func<UpdateDescriptor<T, T>, UpdateDescriptor<T, T>> selector) where T : class
        {
            _client.Update<T>(documentPath, selector);
        }

        public Result<IEnumerable<T>> Find<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> termQuery, string groupName = "") where T : class
        {
            ISearchResponse<T> result = null;
            Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher = null;

            if (termQuery != null)
            {
                searcher = termQuery;
            }

            result = _client.Search<T>(searcher);
            List<KeyValuePair<string, long?>> categoryList = null;

            if (!string.IsNullOrWhiteSpace(groupName))
            {
                var bucket = result.Aggs.Terms(groupName).Buckets;
                categoryList = new List<KeyValuePair<string, long?>>();
                categoryList = bucket.Select(item => new KeyValuePair<string, long?>(item.Key, item.DocCount)).ToList();
            }

            return new Result<IEnumerable<T>>
            {
                TotalCount = result.Total,
                Data = result.Documents.Count() > default(int) ? result.Documents : null,
                Second = TimeSpan.FromMilliseconds(result.Took),
                Category = categoryList
            };
        }

        public void DeleteByQuery<T>(IDeleteByQueryRequest<T> deleteByQuerySelector) where T : class
        {
            _client.DeleteByQuery(deleteByQuerySelector);
        }

        public void DeleteBulk<T>(IEnumerable<T> objects) where T : class
        {
            _client.DeleteMany<T>(objects);
        }

        public List<string> Suggest<T>(Func<SuggestDescriptor<T>, ISuggestRequest> selector, string suggestionName)
            where T : class
        {
            var response = _client.Suggest(selector);
            var result = new List<string>();

            if (response.Suggestions != null && response.Suggestions.Count > default(int))
                result.AddRange(response.Suggestions[suggestionName].SelectMany(x => x.Options).Select(y => y.Text));

            return result;
        }

        public async Task InsertAsync<T>(T model) where T : class
        {
            await _client.IndexAsync(model);
        }

        public async Task DeleteAsync<T>(DocumentPath<T> documentPath, Func<DeleteDescriptor<T>, DeleteDescriptor<T>> selector) where T : class
        {
            await _client.DeleteAsync<T>(documentPath, selector);
        }

        public async Task UpdateAsync<T>(DocumentPath<T> documentPath, Func<UpdateDescriptor<T, T>, UpdateDescriptor<T, T>> selector) where T : class
        {
            await _client.UpdateAsync<T>(documentPath, selector);
        }

        public async Task<Result<IEnumerable<T>>> FindAsync<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> termQuery, string groupName="") where T : class
        {
            ISearchResponse<T> result = null;
            Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher = null;

            if (termQuery != null)
            {
                searcher = termQuery;
            }

            result = await _client.SearchAsync<T>(searcher);
            List<KeyValuePair<string, long?>> categoryList = null;

            if (!string.IsNullOrWhiteSpace(groupName))
            {
                var bucket = result.Aggs.Terms(groupName).Buckets;
                categoryList = new List<KeyValuePair<string, long?>>();
                categoryList = bucket.Select(item => new KeyValuePair<string, long?>(item.Key, item.DocCount)).ToList();
            }

            return new Result<IEnumerable<T>>
            {
                TotalCount = result.Total,
                Data = result.Documents.Count() > default(int) ? result.Documents : null,
                Second = TimeSpan.FromMilliseconds(result.Took),
                Category = categoryList
            };
        }

        public async Task DeleteByQueryAsync<T>(IDeleteByQueryRequest<T> deleteByQuerySelector) where T : class
        {
            await _client.DeleteByQueryAsync(deleteByQuerySelector);
        }

        public async Task DeleteBulkAsync<T>(IEnumerable<T> objects) where T : class
        {
            await _client.DeleteManyAsync<T>(objects);
        }

        public async Task<List<string>> SuggestAsync<T>(Func<SuggestDescriptor<T>, ISuggestRequest> selector, string suggestionName) where T : class
        {
            var response = await _client.SuggestAsync(selector);
            var result = new List<string>();

            if (response.Suggestions != null && response.Suggestions.Count > default(int))
                result.AddRange(response.Suggestions[suggestionName].SelectMany(x => x.Options).Select(y => y.Text));

            return result;
        }
    }
}
