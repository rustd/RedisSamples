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
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("pranavraebc1.redis.cache.windows.net,ssl=true,password=8ZzVFNGCjQS/O89sOZkQ/oew1OEk2m/iFROkCfELbV8=");
            IDatabase cachedb = connection.GetDatabase();
            for (int i = 0; i < 5; i++)
            {
                cachedb.StringSet(i.ToString()+"n", i);
                Console.WriteLine("Current Value=" + cachedb.StringGet(i.ToString() + "n"));    
            }
            
            
        }
    }
}
