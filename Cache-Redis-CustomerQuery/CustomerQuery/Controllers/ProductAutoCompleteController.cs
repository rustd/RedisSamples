using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CustomerQuery.Controllers
{
    public class ProductAutoCompleteController : ApiController
    {
        public List<string> Get(string prefix, string category)
        {
            List<string> ret = new List<string>();
            BookSleeve.RedisConnection connection = new BookSleeve.RedisConnection("productquerycache.redis.cache.windows.net",
                password: "jH2EXEDhfYAG82LzvhZvX0EPRzE8nzyKbrApC1p83wQ=");
            connection.Open();
            var list = connection.Wait(connection.Keys.Find(0, "prod:" + category + ":" + prefix.Replace(' ', ':') + "*"));
            for (int i = 0; i < Math.Min(5, list.Length); i++)
            {
                string s = list[i].Substring(5);
                s = s.Substring(s.IndexOf(':') + 1);
                ret.Add(s.Replace(':',' '));
            }
            connection.Close(false);
            return ret;
        }
    }
}
