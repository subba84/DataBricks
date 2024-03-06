using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace DataBricksWeb
{
    public static class DataCache
    {
        static ObjectCache cache = new MemoryCache("Databricks");
        static object locker = new object();

        public const string CountryKey = "CountryKey";
        public const string CityKey = "CityKey";
        public const string CustomerKey = "CustomerKey";
        public const string ProductKey = "ProductKey";
        public const string ModuleKey = "ModuleKey";
        public const string PocKey = "PocKey";
        public const string RoleKey = "RoleKey";
        public const string VersionKey = "VersionKey";
        public const string PartnerKey = "PartnerKey";

        public static void Load()
        {
            var mapPath = MapPath;
            var country = Countries;
            var customer = Customers;
            var city = Cities;
            var product = Products;
            var module = Modules;
            var role = Roles;
            var v = Versions;
            var p = Partners;
        }

        public static string MapPath
        {
            get {
                return System.Web.HttpContext.Current.Server.MapPath("~/");
            }
        }

        public static List<User> Partners
        {
            get
            {
                var list = cache[PartnerKey] as List<User>;
                if (list == null)
                {
                    using (DbEntities context = new DbEntities())
                    {
                        var users = from c in context.Users where c.RoleId == UserRoleIds.Partner orderby c.FullName select c;
                        if (users != null && users.Count() > 0)
                        {
                            list = users.Distinct().ToList();
                        }
                        else
                        {
                            list = new List<User>();
                        }
                    }
                    Add(PartnerKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearPartner()
        {
            Clear(PartnerKey);
        }

        public static void RefreshPartner()
        {
            Clear(PartnerKey);
            var x = Partners;
        }

        public static List<string> Versions
        {
            get
            {
                var list = cache[VersionKey] as List<string>;
                if (list == null)
                {
                    using (DbEntities context = new DbEntities())
                    {
                        var versions = from c in context.ZipUploads orderby c.VersionNumber select c.VersionNumber;
                        if (versions != null && versions.Count() > 0)
                        {
                            list = versions.Distinct().ToList();
                        }
                        else
                        {
                            list = new List<string>();
                        }
                    }
                    Add(VersionKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearVersion()
        {
            Clear(VersionKey);
        }

        public static void RefreshVersion()
        {
            Clear(VersionKey);
            var x = Versions;
        }

        public static List<Customer> Customers
        {
            get
            {
                var list = cache[CustomerKey] as List<Customer>;
                if (list == null)
                {
                    using (DbEntities context = new DbEntities())
                    {
                        var customers = from c in context.Customers orderby c.CustomerName select c;
                        if (customers != null && customers.Count() > 0)
                        {
                            list = customers.ToList();
                        }
                        else
                        {
                            list = new List<Customer>();
                        }
                    }
                    Add(CustomerKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearCustomer()
        {
            Clear(CustomerKey);
        }

        public static void RefreshCustomer()
        {
            Clear(CustomerKey);
            var x = Customers;
        }

        public static List<Role> Roles
        {
            get
            {
                var list = cache[RoleKey] as List<Role>;
                if (list == null)
                {
                    using (DbEntities context = new DbEntities())
                    {
                        var roles = from c in context.Roles orderby c.RoleName select c;
                        if (roles != null && roles.Count() > 0)
                        {
                            list = roles.ToList();
                        }
                        else
                        {
                            list = new List<Role>();
                        }
                    }
                    Add(RoleKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearRole()
        {
            Clear(RoleKey);
        }

        public static void RefreshRole()
        {
            Clear(RoleKey);
            var x = Roles;
        }

        public static List<Product> Products
        {
            get
            {
                var list = cache[ProductKey] as List<Product>;
                if (list == null)
                {
                    using (DbEntities context = new DbEntities())
                    {
                        var products = from c in context.Products orderby c.ProductName select c;
                        if (products != null && products.Count() > 0)
                        {
                            list = products.ToList();
                        }
                        else
                        {
                            list = new List<Product>();
                        }
                    }
                    Add(ProductKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearProduct()
        {
            Clear(ProductKey);
        }

        public static void RefreshProduct()
        {
            Clear(ProductKey);
            var x = Products;
        }

        public static List<Module> Modules
        {
            get
            {
                var list = cache[ModuleKey] as List<Module>;
                if (list == null)
                {
                    using (DbEntities context = new DbEntities())
                    {
                        var modules = from c in context.Modules orderby c.ModuleName select c;
                        if (modules != null && modules.Count() > 0)
                        {
                            list = modules.ToList();
                        }
                        else
                        {
                            list = new List<Module>();
                        }
                    }
                    Add(ModuleKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearModule()
        {
            Clear(ModuleKey);
        }

        public static void RefreshModule()
        {
            Clear(ModuleKey);
            var x = Modules;
        }


        public static List<Country> Countries
        {
            get
            {
                var list = cache[CountryKey] as List<Country>;
                if (list == null)
                {
                    using (DbEntities context = new DbEntities())
                    {
                        var countries = from c in context.Countries orderby c.CountryName select c;
                        if (countries != null && countries.Count() > 0)
                        {
                            list = countries.ToList();
                        }
                        else
                        {
                            list = new List<Country>();
                        }
                    }
                    Add(CountryKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearCountry()
        {
            Clear(CountryKey);
        }

        public static void RefreshCountry()
        {
            Clear(CountryKey);
            var x = Countries;
        }

        public static List<City> Cities
        {
            get
            {
                var list = cache[CityKey] as List<City>;
                if (list == null)
                {
                    using (DbEntities context = new DbEntities())
                    {
                        var cities = from c in context.Cities orderby c.CityName select c;
                        if (cities != null && cities.Count() > 0)
                        {
                            list = cities.ToList();
                        }
                        else
                        {
                            list = new List<City>();
                        }
                    }
                    Add(CityKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearCity()
        {
            Clear(CityKey);
        }

        public static void RefreshCity()
        {
            Clear(CityKey);
            var x = Cities;
        }

        static void Add(string key, object value, DateTimeOffset expiration, CacheItemPriority priority = CacheItemPriority.Default)
        {
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = expiration;
            policy.Priority = priority;
            var item = new CacheItem(key, value);
            cache.Add(item, policy);
        }
        static void Clear(string key)
        {
            cache.Remove(key);
        }
    }
}