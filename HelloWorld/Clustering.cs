using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace HelloWorld
{
    /// <summary>
    /// Connect to individual shard shard: https://azure.microsoft.com/en-us/documentation/articles/cache-how-to-premium-clustering/
    /// redis-cli.exe -h cachename.redis.cache.windows.net -a password -p 13000
    /// </summary>
    public static class Clustering
    {
        // All keys will be distributed across the shards in the cluster.
        public static string clusterKey = "democluster";
        
        // All customers will be on the same shard.
        public static string clusterAllCustomersKey = "{demoallcustomer}id";

        // Customers and Order details of the customer will be on the same shard.
        public static string clusterCustomersWithOrders = "{democustomer:id}";
        public static string clusterCustomerOrderKey = "{democustomer:id}.orders";

        public static string keyValue = DateTime.Now.ToString();
        public static void Run()
        {
            IDatabase cache = Helper.Connection.GetDatabase();
            DemoSetup(cache);
            for (int i = 0; i < 10; i++)
            {
                cache.StringSet(clusterKey + i.ToString(), keyValue+clusterKey + i.ToString());
                cache.StringSet(clusterAllCustomersKey.Replace("id",i.ToString()), keyValue + clusterAllCustomersKey.Replace("id", i.ToString()));
            }
            for (int i = 10; i < 20; i++)
            {
                cache.StringSet(clusterCustomersWithOrders.Replace("id", i.ToString()), keyValue + clusterCustomersWithOrders.Replace("id", i.ToString()));
                cache.StringSet(clusterCustomerOrderKey.Replace("id", i.ToString()), keyValue + clusterCustomerOrderKey.Replace("id", i.ToString()));
            }

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(clusterKey+i+"="+cache.StringGet(clusterKey + i.ToString()));
                Console.WriteLine(clusterAllCustomersKey.Replace("id", i.ToString()) +"=" + cache.StringGet(clusterAllCustomersKey.Replace("id", i.ToString())));
                Console.WriteLine(clusterCustomerOrderKey.Replace("id", i.ToString()) + "=" + cache.StringGet(clusterCustomerOrderKey.Replace("id", i.ToString())));
            }
            for (int i = 10; i < 20; i++)
            {
                Console.WriteLine(clusterCustomersWithOrders.Replace("id", i.ToString()) + "=" + cache.StringGet(clusterCustomersWithOrders.Replace("id", i.ToString())));
                Console.WriteLine(clusterCustomerOrderKey.Replace("id", i.ToString()) + "=" + cache.StringGet(clusterCustomerOrderKey.Replace("id", i.ToString())));
            }
        }

        private static void DemoSetup(IDatabase cache)
        {
            // Need to wait until the shard (nodes) in the cluster are discovered. This is an issue with the client: https://github.com/StackExchange/StackExchange.Redis/issues/300
            Thread.Sleep(500);

            for (int i = 0; i < 20; i++)
            {
                cache.KeyDelete(clusterKey + i.ToString());
                cache.KeyDelete(clusterAllCustomersKey.Replace("id", i.ToString()));
                cache.KeyDelete(clusterCustomerOrderKey.Replace("id", i.ToString()));
                cache.KeyDelete(clusterCustomersWithOrders.Replace("id", i.ToString()));
            }
        }
    }
}
