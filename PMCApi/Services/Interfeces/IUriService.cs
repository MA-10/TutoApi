using System;
using Test.Requests.Query;

namespace Test.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUriService
    {
        Uri GetUri(int id, string controller);

        Uri GetAllUri(PaginationQuery pagination = null);
    }
}