using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeedCacheForCustomerQuerySample
{
    class Program
    {
        public static string mStorageConnectionString = ConfigurationManager.AppSettings["mStorageConnectionString"];
        //cache for customers
        public static string mCacheUrl = ConfigurationManager.AppSettings["mCacheUrl"];
        public static string mCachePassword = ConfigurationManager.AppSettings["mCachePassword"];
        //cache for products
        public static string mCacheUrl2 = ConfigurationManager.AppSettings["mCacheUrl2"];
        public static string mCachePassword2 = ConfigurationManager.AppSettings["mCachePassword2"];
        
        
        static void Main(string[] args)
        {
            generateCustomerRecords();
            generateProuctRecords();
            primeCustomerCache();
            primeProuctCache();
        }
        static void generateCustomerRecords()
        {
            if (!confirmOperation("YOUR CUSTOMER TABLE WILL BE RECREATED! ALL EXISTING DATA WILL BE LOST! Are you sure?"))
                return;
           CloudStorageAccount account = CloudStorageAccount.Parse(mStorageConnectionString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("customers");
            table.DeleteIfExists();
            while (true)
            {
                try
                {
                    table.CreateIfNotExists();
                    break;
                }
                catch
                {
                    Console.WriteLine("RETRY TABLE CREATION");
                    Thread.Sleep(5000);
                }
            }
            int count = 0;
            int totalRecords = 1000000;
            string[] firstNames = File.ReadAllLines("CSV_Database_of_First_Names.csv");
            string[] lastNames = File.ReadAllLines("CSV_Database_of_Last_Names.csv");
            Random rand = new Random();
            Dictionary<string, TableBatchOperation> batches = new Dictionary<string, TableBatchOperation>();
            while (count < totalRecords)
            {
                count++;
                var company = "Company " + rand.Next(1, 11);
                if (!batches.ContainsKey(company))
                    batches.Add(company, new TableBatchOperation());

                Customer cust = new Customer(company, count.ToString())
                {
                    Value = (rand.NextDouble() - 0.5) * 99999.0,
                    ContractDate = DateTime.Now,
                    Name = firstNames[rand.Next(0, firstNames.Length)] + " " + lastNames[rand.Next(0, lastNames.Length)]
                };
                batches[company].Insert(cust);
                if (batches[company].Count() >= 100)
                {
                    Console.WriteLine("Committing " + batches[company].Count + " recrods to " + company);
                    table.ExecuteBatch(batches[company]);
                    batches[company].Clear();
                }
            }
            foreach(var batch in batches.Values)
            {
                if (batch.Count > 0)
                {
                    Console.WriteLine("Committing " + batch.Count + "records...");
                    table.ExecuteBatch(batch);
                }
            }
        }
        static void generateProuctRecords()
        {
            if (!confirmOperation("YOUR PRODUCTS TABLE WILL BE RECREATED! ALL EXISTING DATA WILL BE LOST! Are you sure?"))
                return;
            CloudStorageAccount account = CloudStorageAccount.Parse(mStorageConnectionString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("products");
            table.DeleteIfExists();
            while (true)
            {
                try
                {
                    table.CreateIfNotExists();
                    break;
                }
                catch
                {
                    Console.WriteLine("RETRY TABLE CREATION");
                    Thread.Sleep(5000);
                }
            }
            int count = 0;
            int totalRecords = 1000000;
            string[] categories = File.ReadAllLines("CSV_Database_of_Categories.csv");
            string[] products = File.ReadAllLines("CSV_Database_of_Products.csv");
            string[] prefixes = { "", "Super ", "Ultimate ", "New ", "", "", ""};
            string[] postfixes = { "", " Mini", " Pro", " Standard", " Lite", " Enterprise", " One", " 2", " X" , " Zero", " 3", " III", " IV", "", "", ""};
            Random rand = new Random();
            Dictionary<string, TableBatchOperation> batches = new Dictionary<string, TableBatchOperation>();
            Dictionary<string, Tuple<int,int>> topRated = new Dictionary<string, Tuple<int,int>>();
            while (count < totalRecords)
            {
                count++;
                var category = categories[rand.Next(0, categories.Length)];
                if (!batches.ContainsKey(category))
                    batches.Add(category, new TableBatchOperation());
                if (!topRated.ContainsKey(category))
                    topRated.Add(category, new Tuple<int,int>(0, rand.Next(4,11)));

                Product prod = new Product(category, count.ToString())
                {
                    Price = (rand.NextDouble() + 0.1) * 99999.0,
                    Name = prefixes[rand.Next(0, prefixes.Length)] + 
                            products[rand.Next(0, products.Length)] +
                            postfixes[rand.Next(0, postfixes.Length)],
                    Category = category,
                };
                int rate = rand.Next(1, 11);
                if (rate == 10)
                {
                    if (topRated[category].Item1 < topRated[category].Item2)
                    {
                        topRated[category] = new Tuple<int, int>(topRated[category].Item1 + 1, topRated[category].Item2);
                        var color = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\n\n\n************\n* TOP RANK!*\n************\n\n\n");
                        Console.ForegroundColor = color;
                    }
                    else
                        rate = rand.Next(1, 10);
                }
                prod.Rate = rate;
                Console.WriteLine(count + " " + prod.Name + " " + prod.Rate + (prod.Rate == 10? "<----------------":""));
                batches[category].Insert(prod);
                if (batches[category].Count() >= 100)
                {
                    Console.WriteLine("Committing " + batches[category].Count + " recrods to " + category);
                    table.ExecuteBatch(batches[category]);
                    batches[category].Clear();
                }
            }
            foreach (var batch in batches.Values)
            {
                if (batch.Count > 0)
                {
                    Console.WriteLine("Committing " + batch.Count + " records...");
                    table.ExecuteBatch(batch);
                }
            }
        }
        static void primeCustomerCache()
        {
            if (!confirmOperation("YOUR CACHE WILL BE FLUSHED! ALL EXISTING INDEXES WILL BE REPLACED! Are you sure?"))
                return;

            CloudStorageAccount account = CloudStorageAccount.Parse(mStorageConnectionString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("customers");
            var query = from c in table.CreateQuery<Customer>()
                        select c;

            var connection = new BookSleeve.RedisConnection(mCacheUrl, allowAdmin: true,
                password: mCachePassword);
            connection.Open();
            connection.Server.FlushDb(0);
            foreach (var c in query)
            {
                string key = string.Format("{0}:{1}", c.PartitionKey, c.RowKey);
                Console.WriteLine("{0}:{1}", key, c.Name);
                connection.SortedSets.Add(0, "customervalues", key, c.Value);
                connection.Strings.Set(0, "cust:" + c.Name.Replace(' ', ':'), key);
            }
            connection.Close(false);
        }
        static void primeProuctCache()
        {
            if (!confirmOperation("YOUR CACHE WILL BE FLUSHED! ALL EXISTING INDEXES WILL BE REPLACED! Are you sure?"))
                return;

            CloudStorageAccount account = CloudStorageAccount.Parse(mStorageConnectionString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("products");
            var query = from c in table.CreateQuery<Product>()
                        select c;

            var connection = new BookSleeve.RedisConnection(mCacheUrl2, allowAdmin: true,
                password: mCachePassword2);
            connection.Open();
            connection.Server.FlushDb(0);
            foreach (var c in query)
            {
                string key = string.Format("{0}:{1}:{2}:{3}:{4}", c.PartitionKey, c.RowKey, c.Name, c.Price, c.Rate);
                Console.WriteLine("{0}:{1}", key, c.Name);
                connection.SortedSets.Add(0, "cat:" + c.Category, key, c.Price);
                connection.SortedSets.Add(0, "rate:" + c.Category, key, c.Rate);
                connection.Strings.Set(0, "prod:" + c.Category + ":" + c.Name.Replace(' ', ':'), key);
            }
            connection.Close(false);
        }
        static bool confirmOperation(string message)
        {
            return true;
            //var color = Console.ForegroundColor;
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.Write("\n\n\n" + message + "\n\n\nPlease confirm (Y/N):");
            //Console.ForegroundColor = color;
            //string input = Console.ReadLine();
            //if (input != "Y" && input != "y")
            //    return false;
            //else
            //    return true;
        }
    }
    public class Customer : TableEntity
    {
        public string Id { get; set; }
        public string Company { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string Comment { get; set; }
        public DateTime ContractDate { get; set; }
        public Customer(string company, string id)
        {
            PartitionKey = company;
            RowKey = id;
            Company = company;
            Id = id; 
        }
        public Customer()
        {

        }
    }
    public class Product: TableEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Manufactor { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }

        public int Rate { get; set; }
        public Product (string manufactor, string id)
        {
            PartitionKey = manufactor;
            RowKey = id;
            Id = id;
            Manufactor = manufactor;
        }
        public Product()
        {

        }
    }
}
