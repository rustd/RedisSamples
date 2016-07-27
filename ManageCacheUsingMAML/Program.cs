using Hyak.Common;
using Microsoft.Azure;
using Microsoft.Azure.Management.Redis;
using Microsoft.Azure.Management.Redis.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
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
        public static string redisSKUName = "Premium"; //Basic/ Standard/ Premium
        public static string redisSKUFamily = "P";
        public static int redisSKUCapacity = 1; //Cache Size 0-6. More details http://azure.microsoft.com/en-us/pricing/details/cache/
        public static string redisCacheRegion = "West US";

        // Use the following link to learn how to setup this app to access Active Directory
        // https://msdn.microsoft.com/en-us/library/azure/dn790557.aspx#bk_portal
        public static string tenantId = "";
        public static string clientId = "";
        public static string redirectUri = "";
        public static string clientSecrets = "";

        static void Main(string[] args)
        {
            //https://msdn.microsoft.com/en-us/library/azure/dn790557.aspx#bk_portal
            string token = GetAuthorizationHeader();

            TokenCredentials creds = new TokenCredentials(token);
            RedisManagementClient client = new RedisManagementClient(creds);
            client.SubscriptionId = subscriptionId;
            var redisParams = new RedisCreateOrUpdateParameters()
            {
                Sku = new Sku(redisSKUName, redisSKUFamily, redisSKUCapacity),
                Location = redisCacheRegion
            };
            client.Redis.CreateOrUpdate(resourceGroupName,cacheName, redisParams);
        }

        private static string GetAuthorizationHeader()
        {
            AuthenticationResult result = null;

            var context = new AuthenticationContext(String.Format("https://login.windows.net/{0}", tenantId));

            var thread = new Thread(() =>
            {
                var authParam = new PlatformParameters(PromptBehavior.Never, null);
                result = context.AcquireTokenAsync(
                    "https://management.core.windows.net/",
                    new ClientCredential(clientId, clientSecrets)).Result;
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
