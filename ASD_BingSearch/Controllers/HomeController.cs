using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(FormCollection collection)
        {
            var query = collection["query"];
            
            List<SearchResult> results = new List<SearchResult>();
            if (!string.IsNullOrEmpty(query))
            {
                ViewBag.query = query;
                results = BingWebSearcher.Search(query);
            }

            return Results(results);
        }

        public ActionResult Results(List<SearchResult> results)
        {

            return View("Results",results);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}