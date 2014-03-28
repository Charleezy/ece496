using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomMembershipEF.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Entry point to the home page of the application
        /// </summary>
        /// <returns>Index View</returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [Authorize]
        public ActionResult Protected()
        {
            return View();
        }
    }
}
