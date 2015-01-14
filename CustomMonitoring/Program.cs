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
        static void Main(string[] args)
        {
            // Define the base URI for management operations
            Uri baseUri = new Uri("https://management.azure.com");

            var subscriptionId = ""; //Get this from the Azure Portal
            string token = GetAuthorizationHeader();
            var ResourceGroupName = "";//Get this from the Azure Portal
            var startTime = DateTime.Now;
            var endTime = startTime.ToUniversalTime().AddHours(1.0).ToLocalTime();
            var cacheName = ""; //Get this from the Azure Portal. This is just the name (without redis.cache.windows.net)
            var redisConnection = "";
            
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(redisConnection);
            IDatabase cachedb = connection.GetDatabase();
            for (int i = 0; i < 10; i++)
            {
                cachedb.StringIncrement(i.ToString());
                Console.WriteLine("value=" + cachedb.StringGet(i.ToString()));
    
            }
            

            // Get the credentials
            // You can find instructions on how to get the token here:
            // http://msdn.microsoft.com/en-us/library/azure/dn790557.aspx
            SubscriptionCloudCredentials credentials = new TokenCloudCredentials(subscriptionId, token);

            // Create an instance of the InsightsClient from Microsoft.Azure.Insights
            InsightsClient client = new InsightsClient(credentials, baseUri);

            // Get the events for an Azure Resource (e.g. Website) (as described by the Azure Resource Manager APIs here:http://msdn.microsoft.com/en-us/library/azure/dn790569.aspx)
            // A resource URI looks like the following string: 
            //"/subscriptions/########-####-####-####-############/resourceGroups/resourcegroupname1/providers/resourceprovider1/resourcename1"
            string resourceUri = string.Format("subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Cache/redis/{2}", subscriptionId,ResourceGroupName, cacheName);

            //Define a FilterString
            string filterString = FilterString.Generate<ListEventsForResourceParameters>(eventData => (eventData.EventTimestamp >= startTime) && (eventData.EventTimestamp <= endTime) && (eventData.ResourceUri == resourceUri));

            //Get the events logs
            EventDataListResponse response = client.EventOperations.ListEvents(filterString, selectedProperties: null);


            //Check the status code of the response
            Console.WriteLine("HTTP Status Code returned for the call:" + response.StatusCode);

            var response2 = client.MetricDefinitionOperations.GetMetricDefinitions(resourceUri, "");
            var metrics = client.MetricOperations.GetMetrics(resourceUri, "startTime eq 2015-01-14T02:19:03.0712821Z and endTime eq 2015-01-14T03:19:03.0712821Z and timeGrain eq duration'PT5M'", response2.MetricDefinitionCollection.Value);
            Console.WriteLine("Print out the metrics logs");
            foreach (var item in metrics.MetricCollection.Value)
            {
                Console.WriteLine(item.Name.Value + "--" +item.MetricValues.Count);
            }

        }
        private static string GetAuthorizationHeader()
        {
            var tenantId = "";
            var clientId = "";
            var redirectUri = "";
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
