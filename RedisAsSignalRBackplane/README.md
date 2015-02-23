Use Redis Cache as a Scale out Backplane for ASP.NET SignalR
=========================================================
This sample demonstrates how you can use Azure Redis Cache as a SignalR backplane.
More details about backplane are http://www.asp.net/signalr/overview/performance/scaleout-with-redis
This samples was used to demo this scenario

## How To Run This Sample


You need the following information from Azure Portal

- Create a Cache https://msdn.microsoft.com/en-us/library/dn690516.aspx
- Copy the Cache Name and Keys and set in Web.config. You can also set it as AppSetting for your Azure Website
- Enable Non-SSL port (6379) in your Cache settings
- Startup.cs has the code to wire up Redis as a backplane
