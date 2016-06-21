using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Words.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public ActionResult getWord()
        {
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
           //My function to get URL and WORD. Replace it with SQL query.
            var fileName = Server.MapPath(Url.Content("~/App_Data/wordsDB.txt"));
            var line = "";
            Random r = new Random();
            var rndNum = r.Next(1, System.IO.File.ReadLines(fileName).Count());

            line = System.IO.File.ReadLines(fileName).Skip(rndNum).Take(1).First(); ;
           
            String[] substrings = line.Split(';');
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            var newWord = substrings[0]; //set New Word 
            var newLink = substrings[1]; //set New Link
            var data = new Words.Models.DataModel()
            {
                Word = newWord,
                Link = newLink
            };
            var jWord = Json(data, JsonRequestBehavior.AllowGet);
            return jWord;
        }
    }
}