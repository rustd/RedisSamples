using CustomerQuery.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CustomerQuery.Controllers
{
    public class HomeController : Controller
    {
        string storageConnectionString = ConfigurationManager.AppSettings["mStorageConnectionString"];
        //cache for customers
         string redisUrl = ConfigurationManager.AppSettings["mCacheUrl"];
         string redisPassword = ConfigurationManager.AppSettings["mCachePassword"];
        //cache for products
        string redisUrl2 = ConfigurationManager.AppSettings["mCacheUrl2"];
         string redisPassword2 = ConfigurationManager.AppSettings["mCachePassword2"];
        const int redisPort = 6379;
        const int redisPortSSL = 6380;

        public ActionResult Index()
        {
            return View(new HomeViewModel());
        }
        public ActionResult Products()
        {
            return View(new HomeViewModel());
        }
        public ActionResult SearchCustomers(HomeViewModel data)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("customers");
            data.MatchedCustomers.Clear();
            Stopwatch watch = new Stopwatch();

            if (data.UseTable)
            {
                watch.Start();
                var query = from c in table.CreateQuery<Customer>()
                            where data.SearchString == c.Name
                            select c;   
                foreach(var c in query)
                {
                    c.Value = (int)(c.Value * 100) / 100.0;
                    data.TableCustomers.Add(c);
                }
                watch.Stop();
                data.TableResponseTime = watch.ElapsedMilliseconds;
            }

            var connection = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { {redisUrl, redisPortSSL} },
                Password = redisPassword,
                Ssl=true
            });

            var db = connection.GetDatabase();
            watch.Restart();

            var record = db.StringGet("cust:" + data.SearchString.Replace(' ', ':'));
            if (!record.IsNullOrEmpty)
            {
                string[] parts = Encoding.ASCII.GetString(record).Split(':');
                if (parts.Length == 2)
                {
                    var quickQuery = from c in table.CreateQuery<Customer>()
                                     where c.PartitionKey == parts[0] && c.RowKey == parts[1]
                                     select c;
                    foreach (var c in quickQuery)
                    {
                        c.Value = (int)(c.Value * 100) / 100.0;
                        data.MatchedCustomers.Add(c);
                    }
                }
            }
            watch.Stop();
            data.CachedResponseTime = watch.ElapsedMilliseconds;
            connection.Close(false);
            return View("Index", data);
        }
        public ActionResult SearchProducts(HomeViewModel data)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("products");
            data.MatchedProducts.Clear();
            Stopwatch watch = new Stopwatch();

            SearchType searchType = SearchType.Name;
            
            double lowerRange = 0;
            double higherRange = 0;
            int top = 5;

            if (data.SearchString.IndexOf('-')>0)
            {
                string[] parts = data.SearchString.Split('-');
                if (parts.Length == 2 && double.TryParse(parts[0], out lowerRange) && double.TryParse(parts[1], out higherRange))
                    searchType = SearchType.Range;
            }
            else if (data.SearchString.StartsWith("top:"))
            {
                searchType = SearchType.Top;
                top = int.Parse(data.SearchString.Substring(4));
            }
            if (data.UseTable)
            {
                watch.Start();
                IQueryable<Product> query = null;

                switch (searchType)
                {
                    case SearchType.Range:
                        query = from c in table.CreateQuery<Product>()
                                where c.Price >= lowerRange && c.Price <= higherRange && c.PartitionKey == data.ProductCategory
                                select c;
                        break;
                    case SearchType.Name:
                        query = from c in table.CreateQuery<Product>()
                                where c.Name == data.SearchString && c.PartitionKey == data.ProductCategory
                                select c;
                        break;
                    case SearchType.Top:
                        query = from c in table.CreateQuery<Product>()
                                where c.PartitionKey == data.ProductCategory
                                select c;
                        break;
                }
                foreach (var c in query)
                {
                    c.Price = (int)(c.Price * 100) / 100.0;
                    data.TableProducts.Add(c);
                }
                switch (searchType)
                {
                    case SearchType.Range:
                        data.TableProducts.Sort(new Comparison<Product>((p1, p2) => { return p2.Price.CompareTo(p1.Price); })); //descending comparision
                        break;
                    case SearchType.Top:
                        data.TableProducts.Sort(new Comparison<Product>((p1, p2) => { return p2.Rate.CompareTo(p1.Rate); })); //descending comparision
                        data.TableProducts = data.TableProducts.Take(top).ToList<Product>();
                        break;
                }
                watch.Stop();
                data.TableResponseTime = watch.ElapsedMilliseconds;
            }

            var connection = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { { redisUrl2, redisPortSSL } },
                Password = redisPassword2,
                Ssl=true
            });
            var db = connection.GetDatabase();
            watch.Restart();

            switch (searchType)
            {
                case SearchType.Range:
                    List<Product> products = getProducts(data.ProductCategory, lowerRange, higherRange);
                    data.MatchedProducts = products;
                    break;
                case SearchType.Top:
                    List<string> keys = getTopKeys(data.ProductCategory, top);
                    foreach(string key in keys)
                    {
                        string[] parts = key.Split(':');
                        if (parts.Length == 5)
                        {
                            var quickQuery = from c in table.CreateQuery<Product>()
                                             where c.PartitionKey == parts[0] && c.RowKey == parts[1]
                                             select c;
                            foreach (var c in quickQuery)
                            {
                                c.Price = (int)(c.Price * 100) / 100.0;
                                data.MatchedProducts.Add(c);
                            }
                        }
                    }
                    break;
                case SearchType.Name:
                    var record = db.StringGet("prod:" + data.ProductCategory + ":" + data.SearchString.Replace(' ', ':'));
                    if (!record.IsNullOrEmpty)
                    {
                        string[] parts = Encoding.ASCII.GetString(record).Split(':');
                        if (parts.Length == 5)
                        {
                            var quickQuery = from c in table.CreateQuery<Product>()
                                             where c.PartitionKey == parts[0] && c.RowKey == parts[1]
                                             select c;
                            foreach (var c in quickQuery)
                            {
                                c.Price = (int)(c.Price * 100) / 100.0;
                                data.MatchedProducts.Add(c);
                            }
                        }
                    }
                    break;
            }
            
            watch.Stop();
            data.CachedResponseTime = watch.ElapsedMilliseconds;
            connection.Close(false);
            return View("Products", data);
        }
        private List<Product> getProducts(string category, double start, double end)
        {
            List<Product> ret = new List<Product>();
            BookSleeve.RedisConnection connection = new BookSleeve.RedisConnection(redisUrl2,
                password: redisPassword2);
            connection.Open();
            var list = connection.SortedSets.Range(0, "cat:" + category, min:start, max:end, ascending:false).Result;
            foreach (var item in list)
            {
                string[] parts = Encoding.ASCII.GetString(item.Key).Split(':');
                if (parts.Length == 5)
                    ret.Add(new Product
                    {
                        Category = parts[0],
                        Name = parts[2],
                        Price = (int)(double.Parse(parts[3]) * 100) / 100.0,
                        Rate = int.Parse(parts[4])
                    });
            }
            connection.Close(false);
            return ret;
        }
        private List<string> getTopKeys(string category, int top)
        {
            List<string> ret = new List<string>();
            BookSleeve.RedisConnection connection = new BookSleeve.RedisConnection(redisUrl2,
                password: redisPassword2);
            connection.Open();
            var list = connection.SortedSets.Range(0, "rate:" + category, start:0,  stop:top-1, ascending: false).Result;
            foreach (var item in list)
                ret.Add(Encoding.ASCII.GetString(item.Key));
            connection.Close(false);
            return ret;
        }
    }
    internal enum SearchType
    {
        Name,
        Range,
        Top
    }
}