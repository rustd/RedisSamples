Seed the Database and Cache for the demo
=========================================================
This sample will seed the Azure Storage account and populate 2 caches with the data.
This data is used in the RedisCacheCustomerQuerySample sample. 

## How To Run This Sample
You need the following information from Azure Portal

- Create a Storage Account and set the connection in app.config
- Create 2 caches as shown http://azure.microsoft.com/en-us/documentation/articles/cache-dotnet-how-to-use-azure-redis-cache/
- Copy the Cache Name and Keys and set them in app.config
- Run the app. Note this will delete any existing data in the Cache
