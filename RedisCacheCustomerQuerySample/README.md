Demo how Redis Cache can improve performance by Caching data
=====================================================================
This sample shows how a Cache can make an app more performant since retreivals from Cache are much faster as compared to a persistent storage.

## Pre-req
You should run the SeedCacheForCustomerQuerySample for seeding the Azure Storage and Caches. Use the same connectionstring that you used in the Seeding Sample

## How To Run This Sample
You need the following information from Azure Portal
.Use the same connectionstring that you used in the Seeding Sample
- Create a Azure Storage Account.
- Create two Caches. Read more how How-To here https://msdn.microsoft.com/en-us/library/dn690516.aspx
- Copy the Storage Connection, Cache Name and Keys and set in App.config. You can also set it as AppSetting for your Azure Website
- Enable Non-SSL port (6379) in your Cache settings
- Run the app (Note this will override any existing data in your storage account/ Cache)

## Demoes
- Search by Customer (Aaron)
- Do autoComplete in the text box to get the Customername
- Naviate to Product and browse by Category
