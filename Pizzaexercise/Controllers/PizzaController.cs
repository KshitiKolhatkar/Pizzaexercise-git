using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Pizzaexercise.Controllers
{
    public class PizzaController : Controller
    {
        // GET: Pizza
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult GetToppings()
        {
            try
            {
                //json url for pizza toppings
                string url = "http://files.olo.com/pizzas.json";
                using (WebClient wc = new WebClient())
                {
                    var json = wc.DownloadString(url); //download json 
                    var jsonobjects = JArray.Parse(json); // load array from json string

                    //Deserialize the json object to .net type 
                    var pizza_toppings = JsonConvert.DeserializeObject<clsPizzaTopings[]>(JsonConvert.SerializeObject(jsonobjects));                   

                    //query class to group the toppings by by particular type and get  the count of each type
                    var toppings = from t in pizza_toppings.SelectMany(i => i.toppings)
                                   group t by t
                                   into pt
                                   orderby pt.Count() descending
                                   select new { toppings = pt.Key, Count = pt.Count() };

                     // create a datatable to list only 20 frequently used toppings with their total counts, also rank them accordingly
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Rank",typeof(int));
                    dataTable.Columns.Add("Toppings");
                    dataTable.Columns.Add("Count", typeof(int));
                    for (int i=0;i<20;i++)
                    {
                        dataTable.Rows.Add(i+1, toppings.ElementAt(i).toppings, toppings.ElementAt(i).Count);
                    }
                    //convert dataTable to json string to list inn a table 
                    string strtoppings = JsonConvert.SerializeObject(dataTable, Newtonsoft.Json.Formatting.Indented);

                    return Json(strtoppings, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                return Json(ex.StackTrace);
            }
        }
    }
    /* This class stores toppings 
     */
    public class clsPizzaTopings
    {
        public string[] toppings { get; set; }
    }
}