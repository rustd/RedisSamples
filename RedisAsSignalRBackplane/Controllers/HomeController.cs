using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Firework.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(int type = 1, int color = 1)
        {
            ViewBag.FireworkType = type;
            ViewBag.FireworkColorIndex = color;
            return View();
        }
    }
}
