Use Redis Cache as a Scale out Backplane for ASP.NET SignalR
====================================

=====================
This sample demonstrates how you can use Azure Redis Cache as a SignalR backplane.
More details about backplane are http://www.asp.net/signalr/overview/performance/scaleout-with-redis
This samples was used to demo this scenario

## How To Run This Sample


You need the following information from Azure Portal

- Setup your app for Authenticating Azure Resource Manager with Active Directory. For How-To instructions please read https://msdn.microsoft.com/en-us/library/azure/dn790557.aspx#bk_portal

- Your SubscriptionId

- ResourceGroupName where the Redis is CacheA
- Cache Name. This is just the name (without redis.cache.windows.net)


- You can set more values specific to your Cache such as Cache Size, Region etc. in code.