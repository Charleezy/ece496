using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CustomMembershipEF.Models;
using CustomMembershipEF.Contexts;

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
            int uid, cid;

            using (var usersContext = new UsersContext())
            {
                uid = usersContext.GetUserId(User.Identity.Name);
            }

            using (var context = new PM_Entities())
            {
                var course = context.Courses
                                .Where(x => x.CourseToken == coursetoken)
                                .Single();

                cid = course.CourseID;
                var newTeam = new Team { TeamName = teamname, CourseID = cid };


                context.Teams.Add(newTeam);

                var newMember = new TeamMember { FK_UserID = uid, FK_TeamID = newTeam.TeamID };
                context.TeamMembers.Add(newMember);
                context.SaveChanges();
            }
        }
        
        public JsonResult GetTeamList()
        {
            int userid;
            List<TeamTable> teaminfo = new List<TeamTable>();
            List<string> teammembers2 = new List<string>();

            var usersContext = new UsersContext();
            var teamsContext = new PM_Entities();

            userid = usersContext.GetUserId(User.Identity.Name);
            
            var teamlist = teamsContext.TeamMembers
                                   .Where(x => x.FK_UserID == userid)
                                   .ToList();
            
            foreach (var team in teamlist)
            {
                Team usersteam = teamsContext.Teams
                                       .Where(x => x.TeamID == team.FK_TeamID)
                                       .Single();

                string coursename = teamsContext.Courses
                                          .Where(x => x.CourseID == usersteam.CourseID)
                                          .Select(y => y.CourseName)
                                          .SingleOrDefault();

                var teammembers = teamsContext.TeamMembers
                                        .Where(x => x.FK_TeamID == team.FK_TeamID)
                                        .Select(y => y.FK_UserID)
                                        .ToArray();

                foreach (var memberID in teammembers)
                {
                    string membername = usersContext.GetUserName(memberID);
                    teammembers2.Add(membername);
                }
                
                string[] myarray = teammembers2.ToArray();
                teammembers2.Clear();

                TeamTable teamitem = new TeamTable { TeamID = usersteam.TeamID, TeamName = usersteam.TeamName, Course = coursename, TeamMembers = myarray };

                teaminfo.Add(teamitem);
            }

            return Json(teaminfo, JsonRequestBehavior.AllowGet);
        }

        public void SendInvite(string username, string teams)
        {
            var usersContext = new UsersContext();
            var teamsContext = new PM_Entities();

            int uid = usersContext.GetUserId(username);
        }
    }
}
