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
            int uid = GetUserID(User.Identity.Name);
            List<TeamMember> users_teams = new List<TeamMember>();
            List<Team> team_list = new List<Team>();

            ViewModels.TaskManagerViewModel model = new ViewModels.TaskManagerViewModel();

            using (var teamsContext = new PM_Entities())
            {
                users_teams = teamsContext.TeamMembers
                                   .Where(x => x.FK_UserID == uid)
                                   .ToList();

                foreach (TeamMember team in users_teams)
                {
                    Team t = teamsContext.Teams
                                        .Where(x => x.TeamID == team.FK_TeamID)
                                        .Single();

                    team_list.Add(t);
                }
            }

            model.teamList = team_list;

            return View(model);
        }

        [Authorize]
        public ActionResult Calendar()
        {
            int uid = GetUserID(User.Identity.Name);
            ViewBag.uid = uid;

            List<TeamMember> users_teams = new List<TeamMember>();
            List<Team> team_list = new List<Team>();

            using (var teamsContext = new PM_Entities())
            {
                users_teams = teamsContext.TeamMembers
                                   .Where(x => x.FK_UserID == uid)
                                   .ToList();

                foreach (TeamMember team in users_teams)
                {
                    Team t = teamsContext.Teams
                                        .Where(x => x.TeamID == team.FK_TeamID)
                                        .Single();
                    
                    team_list.Add(t);
                }
            }

            return View(team_list);
        }

        [Authorize]
        public ActionResult Settings()
        {
            return View();
        }
    }
}
