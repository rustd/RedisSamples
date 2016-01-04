using Microsoft.Azure;
using Microsoft.Azure.Common.OData;
using Microsoft.Azure.Insights;
using Microsoft.Azure.Insights.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomMonitoring
{
    class Program
    {
        public static string subscriptionId = ""; // Get this from the Azure Portal
        public static string resourceGroupName = ""; //,Get this from the Azure Portal
        public static string cacheName = ""; // This is just the name (without redis.cache.windows.net)

        // Use the following link to learn how to setup this app to access Active Directory
        // https://msdn.microsoft.com/en-us/library/azure/dn790557.aspx#bk_portal
        public static string tenantId = "";
        public static string clientId = "";
        public static string redirectUri = ""; // Your app name

        static void Main(string[] args)
        {
            // Define the base URI for management operations
            var azureUrl = "https://management.azure.com";  // or "https://management.chinacloudapi.cn/" for China
            Uri baseUri = new Uri(azureUrl);

            string token = GetAuthorizationHeader();
            var startTime = DateTime.Now;
            var endTime = startTime.ToUniversalTime().AddHours(1.0).ToLocalTime();
            var redisConnection = "";

            
            // Get the credentials
            // You can find instructions on how to get the token here:
            // http://msdn.microsoft.com/en-us/library/azure/dn790557.aspx
            SubscriptionCloudCredentials credentials = new TokenCloudCredentials(subscriptionId, token);

            // Create an instance of the InsightsClient from Microsoft.Azure.Insights
            InsightsClient client = new InsightsClient(credentials, baseUri);

            // Get the events for an Azure Resource (e.g. Website) (as described by the Azure Resource Manager APIs here:http://msdn.microsoft.com/en-us/library/azure/dn790569.aspx)
            // A resource URI looks like the following string: 
            //"/subscriptions/########-####-####-####-############/resourceGroups/resourcegroupname1/providers/resourceprovider1/resourcename1"
            string resourceUri = string.Format("subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Cache/redis/{2}", subscriptionId, resourceGroupName, cacheName);

            //Define a FilterString
            string filterString = FilterString.Generate<ListEventsForResourceParameters>(eventData => (eventData.EventTimestamp >= startTime) && (eventData.EventTimestamp <= endTime) && (eventData.ResourceId == resourceUri));

            //Get the events logs
            EventDataListResponse response = client.EventOperations.ListEvents(filterString, selectedProperties: null);


            //Check the status code of the response
            Console.WriteLine("HTTP Status Code returned for the call:" + response.StatusCode);

            var response2 = client.MetricDefinitionOperations.GetMetricDefinitions(resourceUri, "");
            var metrics = client.MetricOperations.GetMetrics(resourceUri, "startTime eq " + startTime.ToString("yyyy-MM-ddThh:mm:ss") + "and endTime eq " + endTime.ToString("yyyy-MM-ddThh:mm:ss") + " and timeGrain eq duration'PT5M'", response2.MetricDefinitionCollection.Value);
            Console.WriteLine("Print out the metrics logs");
            foreach (var item in metrics.MetricCollection.Value)
            {
                Console.WriteLine(item.Name.Value + "--" + item.MetricValues.Count);
            }

        }
        private static string GetAuthorizationHeader()
        {
            AuthenticationResult result = null;

            var url1 = "https://login.windows.net/"; // or "https://login.chinacloudapi.cn" for China
            var url2 = "https://management.core.windows.net/"; // or "https://management.core.chinacloudapi.cn/" for China
            var context = new AuthenticationContext(new Uri(new Uri(url1), tenantId).AbsoluteUri);
            var thread = new Thread(() =>
            {

                result = context.AcquireToken(
                  url2,
                  "1950a258-227b-4e31-a9cf-717495945fc2", // clientId
                  new Uri("urn:ietf:wg:oauth:2.0:oob"));
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
