using System;
using Microsoft.AspNetCore.WebUtilities;

using Test;
using Test.Requests.Query;

namespace Test.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class UriService : IUriService
    {
        private readonly string _baseUri;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUri"></param>
        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Uri GetUri(int id, string controller)
        {
            return new Uri($"{_baseUri}{ApiRoutes.Base}/{controller}/{id}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public Uri GetAllUri(PaginationQuery pagination = null)
        {
            var uri = new Uri(_baseUri);
            if (pagination == null) return uri;
            var modifiedUri = QueryHelpers.AddQueryString(_baseUri, "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());
            return new Uri(modifiedUri);
        }
    }
}