using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
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

    public static class Helper
    {


        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect("pranavracachedemo1.redis.cache.windows.net,ssl=true,password=zvwaNabayb2bhOJ0tRxSRYWLZIfVAqzvknXs6u0ziYU=");
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }

}
