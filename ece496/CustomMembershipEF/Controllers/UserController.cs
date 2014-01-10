using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomMembershipEF.Controllers
{
    public class UserController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult TeamManager()
        {
            string username = User.Identity.Name;
            return View();
        }

        [Authorize]
        public ActionResult TaskManager()
        {
            string username = User.Identity.Name;
            return View();
        }

        [Authorize]
        public ActionResult Calendar()
        {
            string username = User.Identity.Name;
            return View();
        }

        [Authorize]
        public ActionResult Settings()
        {
            string username = User.Identity.Name;
            return View();
        }
    }
}
