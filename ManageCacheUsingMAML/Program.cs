using Hyak.Common;
using Microsoft.Azure;
using Microsoft.Azure.Management.Redis;
using Microsoft.Azure.Management.Redis.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManageCacheUsingMAML
{
    class Program
    {
        public static string subscriptionId = ""; //Get this from the Azure Portal
        public static string resourceGroupName = "";//Get this from the Azure Portal
        public static string cacheName = ""; // This is just the name (without redis.cache.windows.net)
        public static string redisVersion = "2.8";
        public static string redisSKUName = "Basic"; //Basic/ Standard
        public static string redisSKUFamily = "C";
        public static int redisSKUCapacity = 1; //Cache Size 0-6. More details http://azure.microsoft.com/en-us/pricing/details/cache/
        public static string redisCacheRegion = "North Central US";

        // Use the following link to learn how to setup this app to access Active Directory
        // https://msdn.microsoft.com/en-us/library/azure/dn790557.aspx#bk_portal
        public static string tenantId = "";
        public static string clientId = "";
        public static string redirectUri = "";

        static void Main(string[] args)
        {
            //https://msdn.microsoft.com/en-us/library/azure/dn790557.aspx#bk_portal
            string token = GetAuthorizationHeader();

            TokenCloudCredentials creds = new TokenCloudCredentials(subscriptionId,token);

            RedisManagementClient client = new RedisManagementClient(creds);
            var redisProperties = new RedisProperties();
            redisProperties.Sku = new Sku(redisSKUName,redisSKUFamily,redisSKUCapacity);
            redisProperties.RedisVersion = redisVersion;
            var redisParams = new RedisCreateOrUpdateParameters(redisCacheRegion, redisProperties);
            client.Redis.CreateOrUpdate(resourceGroupName,cacheName, redisParams);
        }
        private static string GetAuthorizationHeader()
        {
            AuthenticationResult result = null;

            var context = new AuthenticationContext(String.Format("https://login.windows.net/{0}", tenantId));

            var thread = new Thread(() =>
            {
                result = context.AcquireToken(
                  "https://management.core.windows.net/",
                  clientId,
                  new Uri(redirectUri));
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "AquireTokenThread";
            thread.Start();
            thread.Join();


            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.AccessToken;
            return token;
        }
    }
}
