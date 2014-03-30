using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomMembershipEF.Models;
using CustomMembershipEF.Contexts;

namespace CustomMembershipEF.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Get a user's userID
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <returns>ID of the user</returns>
        public int GetUserID(string username)
        {
            int uid;
            using (var usersContext = new UsersContext())
            {
                uid = usersContext.GetUserId(username);
            }
            return uid;
        }

        /// <summary>
        /// Entry point to the home page of the application
        /// </summary>
        /// <returns>Index View</returns>
        public ActionResult Index()
        {
            // If authenticated, go to team manager page, otherwise go to home page
            if (User.Identity.IsAuthenticated)
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

                return View("~/Views/User/TeamManager.cshtml", model);
            }
            else
            {
                return View();
            }
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
