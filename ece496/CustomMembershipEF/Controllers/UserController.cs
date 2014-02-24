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
        public int GetUserID(string username)
        {
            int uid;

            using (var usersContext = new UsersContext())
            {
                uid = usersContext.GetUserId(username);
            }

            return uid;
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult TeamManager()
        {
            int count;
            int uid = GetUserID(User.Identity.Name);

            ViewModels.TeamManagerViewModel model = new ViewModels.TeamManagerViewModel();

            using (var teamsContext = new PM_Entities())
            {
                count = teamsContext.Invitations
                                    .Where(x => x.Recipient == uid)
                                    .Count();
            }

            model.inviteCount = count;

            return View(model);
        }

        [Authorize]
        public ActionResult TaskManager()
        {
            return View();
        }

        [Authorize]
        public ActionResult Calendar()
        {
            int uid = GetUserID(User.Identity.Name);
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
