Access Redis Cache Monitoring data
====================================

This sample demonstrates how you can access monitoring data for your Azure Redis Cache outside of the Azure portal.
Azure Redis Cache has a new monitoring system gives you the capability to store monitoring and diagnostics data for your cache in your own Azure Storage account, which means that you can now use the tools of your choice to access this data. To ensure you continue to receive monitoring and diagnostics data for your cache and alert rules, we recommend that you configure an Azure Storage account where all the monitoring and diagnostics data will be stored.

## Prereq
This sample uses the Microsoft Azure Insights SDK which is used to access the monitoring data outside the portal. The SDK right now is in preview
Please take a look at the SDK here: https://www.nuget.org/packages/Microsoft.Azure.Insights/ 
The documentation is here: http://msdn.microsoft.com/en-us/library/azure/dn802153.aspx

## How To Run This Sample

You need the following information from Azure Portal
- Setup your app for Authenticating Azure Resource Manager with Active Directory. For How-To instructions please read https://msdn.microsoft.com/en-us/library/azure/dn790557.aspx#bk_portal
- Your SubscriptionId
- ResourceGroupName where the Redis is CacheA
- Cache Name. This is just the name (without redis.cache.windows.net)

