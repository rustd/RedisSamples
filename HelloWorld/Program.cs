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
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("");
            IDatabase cachedb = connection.GetDatabase();
            cachedb.StringSet("time",DateTime.Now.ToString());
            Console.WriteLine("Current Time="+ cachedb.StringGet("time"));
        }
    }
}
