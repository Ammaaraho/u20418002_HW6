using u20418002_HW6.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Collections;
using System.Data.Entity;

namespace u20418002_HW6.Controllers
{
    public class HomeController : Controller
    {
        private readonly BikeStoresEntities1 db = new BikeStoresEntities1();
        public ActionResult Index()
        {
            return View();           
        }

        public ActionResult About()
        {
            return PartialView();
        }

        public string GetCatergoryNames()
        {
            object catergoryData = db.categories.Select(p => new { id = p.category_id, name = p.category_name }).ToList();
            return JsonConvert.SerializeObject(catergoryData);
        }

        public string GetProducts(int? i)
        {
            db.Configuration.ProxyCreationEnabled = false;
            object productDatas = db.products.Select(p => new { id = p.product_id, name = p.product_name, brand = p.brand.brand_name, catergory = p.category.category_name, model = p.model_year, price = p.list_price }).ToList().ToPagedList(i ?? 1,10);
            return JsonConvert.SerializeObject(productDatas);
        }

        public string Search(string text)
        {
            db.Configuration.ProxyCreationEnabled = false;
            object productDatas = db.products.Where(o => o.product_name.Contains(text)).Select(p => new { id = p.product_id, name = p.product_name, brand = p.brand.brand_name, catergory = p.category.category_name, model = p.model_year, price = p.list_price }).ToList();
            return JsonConvert.SerializeObject(productDatas);
        }
        public string GetBrandData()
        {
            db.Configuration.ProxyCreationEnabled = false;

            List<brand> data = db.brands.ToList();

            return JsonConvert.SerializeObject(data);
        }
        public ActionResult Orders()
        {                                                
            
            List<OrderVM> productDatas = db.order_items.Select(p => new OrderVM{ orderid = p.order_id, category = p.product.category.category_name , product = db.products.Where(x => x.product_id == p.product_id).FirstOrDefault(), quantity = p.quantity, price = p.list_price, total = (p.list_price * p.quantity), orderdate = db.orders.Where( o => o.order_id == p.order_id).FirstOrDefault().order_date }).ToList();
            return View(productDatas);           
        }

        public ActionResult SearchOrders(DateTime date)
        {
            string day = date.ToShortDateString();
           
            List<OrderVM> productDatas = db.order_items.Where(y => y.order.order_date <= date).Select(p => new OrderVM { orderid = p.order_id, category = p.product.category.category_name, product = db.products.Where(x => x.product_id == p.product_id).FirstOrDefault(), quantity = p.quantity, price = p.list_price, total = (p.list_price * p.quantity), orderdate = db.orders.Where(o => o.order_id == p.order_id).FirstOrDefault().order_date }).ToList();
            return View("Orders", productDatas);
        }

        public ActionResult  Report()
        {

            return View();
        }

        public ActionResult Index(int page = 0)
        {
            const int PageSize = 3; // you can always do something more elegant to set this

            var count = this.db.Count();

            var data = this.db.Skip(page * PageSize).Take(PageSize).ToList();

            this.ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            this.ViewBag.Page = page;

            return this.View(data);
        }
        public string ProductDetails(int id )
        {
           
            object productDetial = db.stocks.Where(y => y.product_id == id).Include( v => v.product).Select(p => new {
                productname = p.product.product_name,
                year = p.product.model_year,
                price = p.product.list_price,
                brand = p.product.brand.brand_name,
                catergory = p.product.category.category_name,             
                stores = db.stocks.Where(s => s.product_id == id).Select(n => new {storename = n.store.store_name, quantity = n.quantity}) 

            }).FirstOrDefault();


            return JsonConvert.SerializeObject(productDetial);

        }

        public string GetReports()
        {
            db.Configuration.ProxyCreationEnabled = false;
            object bikes = db.orders.Select(o => new
            {
                month = o.order_date.Month,
                bike = db.order_items.Where(x => x.order_id == o.order_id && x.product.category.category_id == 6).ToList() ,
            }).ToList();

            return JsonConvert.SerializeObject(bikes);
        }

    
    }
}