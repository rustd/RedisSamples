using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("<cachename>.redis.cache.windows.net,ssl=true,password=password");
            IDatabase cachedb = connection.GetDatabase();
            for (int i = 0; i < 5; i++)
            {
                cachedb.StringSet(i.ToString()+"n", i);
                Console.WriteLine("Current Value=" + cachedb.StringGet(i.ToString() + "n"));    
            }
            
            
        }
    }
}
