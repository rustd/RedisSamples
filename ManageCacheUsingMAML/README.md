Manage Azure Redis Cache using Azure Management Libraries
====================================

=====================
This sample demonstrates how can you use Azure Management Libraries to manage - (Create/ Update/ delete) your Cache.
It uses the following NuGet package http://www.nuget.org/packages/Microsoft.Azure.Management.Redis/0.14.1-preview

## How To Run This Sample


You need the following information from Azure Portal

- Setup your app for Authenticating Azure Resource Manager with Active Directory. For How-To instructions please read https://msdn.microsoft.com/en-us/library/azure/dn790557.aspx#bk_portal

- Your SubscriptionId

- ResourceGroupName where the Redis is CacheA
- Cache Name. This is just the name (without redis.cache.windows.net)


- You can set more values specific to your Cache such as Cache Size, Region etc. in code.