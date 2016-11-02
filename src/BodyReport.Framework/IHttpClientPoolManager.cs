using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BodyReport.Framework
{
    public interface IHttpClientPoolManager<T>
    {
       Task<T> GetAsync<T>(string userId, Cookie userIdentityCookie, string relativeUrl, Dictionary<string, string> datas = null);
    }

    public interface IWebApi
    {
    }
}
