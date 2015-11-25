using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    public static class Helper
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisCacheName"]+",abortConnect=false,ssl=true,password="+ ConfigurationManager.AppSettings["RedisCachePassword"]);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }

    public class BlogPost
    {
        private HashSet<string> tags = new HashSet<string>();

        public BlogPost(int id, string title, int score, IEnumerable<string> tags)
        {
            this.Id = id;
            this.Title = title;
            this.Score = score;
            this.tags = new HashSet<string>(tags);
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int Score { get; set; }
        public ICollection<string> Tags { get { return this.tags; } }
    }
}
