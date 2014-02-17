using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CustomMembershipEF.Models;
using CustomMembershipEF.Contexts;
using System.Data.SqlClient;
using System.Data;

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
            return View();
        }

        [Authorize]
        public ActionResult TaskManager()
        {
            return View();
        }

        [Authorize]
        public ActionResult Calendar()
        {
            int uid;

            using (var usersContext = new UsersContext())
            {
                uid = usersContext.GetUserId(User.Identity.Name);
            }

            ViewBag.uid = uid;

            return View();
        }

        [Authorize]
        public ActionResult Settings()
        {
            return View();
        }
    }
}
