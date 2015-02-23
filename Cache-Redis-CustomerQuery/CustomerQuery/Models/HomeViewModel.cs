using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CustomerQuery.Models
{
    public class HomeViewModel
    {
        [Display(Name="Customer name")]
        public string SearchString { get; set; }
        [Display(Name="Directly seach table")]
        public bool UseTable { get; set; }
        [Display(Name = "Response time with cache")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public long CachedResponseTime { get; set; }
        [Display(Name="Response time without cache")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public long TableResponseTime { get; set; }
        public List<Customer> MatchedCustomers { get; set; }
        public List<Product> MatchedProducts { get; set; }
        public List<Customer> TableCustomers { get; set; }
        public string ProductCategory { get; set; }
        public List<Product> TableProducts { get; set; }
        public List<string> ProductCategories { get; set; }
        public HomeViewModel()
        {
            MatchedCustomers = new List<Customer>();
            TableCustomers = new List<Customer>();
            MatchedProducts = new List<Product>();
            TableProducts = new List<Product>();
            ProductCategories = new List<string>{
                "Apparel", "Automotive", "Baby", "Beauty", "Books", "Classical", "DVD",
                "Electronics", "GourmetFood", "Grocery", "HealthPersonalCare", "HomeGarden",
                "Industrial", "Jewelry", "KindleStore", "Kitchen", "Magazines", "MP3Downloads",
                "Music", "MusicalInstruments", "OfficeProducts", "OutdoorLiving", "PCHardware",
                "PetSupplies", "Photo", "Shoes", "Software", "SportingGoods", "Tools",
                "Toys", "UnboxVideo", "VHS", "VideoGames", "Wireless", "WirelessAccessories"};
            ProductCategory = ProductCategories.First();
        }
    }
    public class Customer: TableEntity
    {
        public string Id { get; set; }
        public string Company { get; set; }
        public string Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Value { get; set; }
        public string Comment { get; set; }
        public DateTime ContractDate { get; set; }
    }
     public class Product: TableEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Manufactor { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public int Rate { get; set; }
    }
}