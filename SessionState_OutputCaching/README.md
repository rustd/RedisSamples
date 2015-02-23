Use Azure Redis Cache to store ASP.NET SessionState and OutputCache
==============================================================
This sample demonstrates how you to use Azure Redis Cache to store ASP.NET Session and Output Cache. This app uses the SessionState and OutputCache providers for Redis.

## How To Run This Sample


You need the following information from Azure Portal
.Use the same connectionstring that you used in the Seeding Sample
- Create a Cache. Read more how How-To here https://msdn.microsoft.com/en-us/library/dn690516.aspx
- Copy Cache Name and Keys and set in web.config in the providers section for Session & OutputCache.
- Run the app
