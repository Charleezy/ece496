using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CustomMembershipEF.Models;
using CustomMembershipEF.Contexts;
using System.Data.Objects;

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
        
        public void CreateTeam(string teamname, string coursetoken)
        {
            using (var context = new PM_Entities())
            {
                var course = context.Courses
                                .Where(x => x.CourseToken == coursetoken)
                                .Single();

                int cid = course.CourseID;

                var newTeam = new Team { TeamName = teamname, CourseID = cid };

                context.Teams.Add(newTeam);
                context.SaveChanges();
            }
        }

        public JsonResult GetTeamList()
        {
            List<Team> teams = new List<Team>();
            List<int> teamlist = new List<int>();

            using (var usersContext = new UsersContext())
            {
                int id = usersContext.GetUserId(User.Identity.Name);
            }

            using (var pmContext = new PM_Entities())
            {
                
            }

            return Json(teams);
        }
    }
}
