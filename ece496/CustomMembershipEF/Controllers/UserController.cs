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

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Entry point to the Team Manager View
        /// </summary>
        /// <returns>Team Manager View</returns>
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

        /// <summary>
        /// Entry point to the Task Manager View
        /// </summary>
        /// <returns>Task Manager View</returns>
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

        /// <summary>
        /// Entry point to the Calendar view
        /// </summary>
        /// <returns>Calendar View</returns>
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

        /// <summary>
        /// Entry point to the Settings view
        /// </summary>
        /// <returns>Settings View</returns>
        [Authorize]
        public ActionResult Settings()
        {
            return View();
        }
    }
}
