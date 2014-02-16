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
            return View();
        }

        [Authorize]
        public ActionResult Settings()
        {
            return View();
        }

        /// <summary>
        /// Adds a new entry to the Teams table.
        /// </summary>
        /// <param name="teamname">Name of the team.</param>
        /// <param name="coursetoken">A code mapping to a course.</param>
        public void CreateTeam(string teamname, string coursetoken)
        {
            int uid;

            using (var usersContext = new UsersContext()) {
                uid = usersContext.GetUserId(User.Identity.Name);
            }

            using (var teamscontext = new PM_Entities()) {
                var course = teamscontext.Courses
                                .Where(x => x.CourseToken == coursetoken)
                                .Single();

                var newTeam = new Team { TeamName = teamname, FK_CourseID = course.CourseID };
                teamscontext.Teams.Add(newTeam);

                var newMember = new TeamMember { FK_UserID = uid, FK_TeamID = newTeam.TeamID };
                teamscontext.TeamMembers.Add(newMember);

                teamscontext.SaveChanges();
            }
        }
        
        /// <summary>
        /// Retrieve a complete list of teams and team details for the current user.
        /// </summary>
        /// <returns>Json object of type TeamTableItem.</returns>
        public JsonResult GetTeamList()
        {
            int userid;
            List<TeamTableItem> teaminfo = new List<TeamTableItem>();
            List<string> teammembers2 = new List<string>();

            var usersContext = new UsersContext();
            var teamsContext = new PM_Entities();

            userid = usersContext.GetUserId(User.Identity.Name);
            
            var teamlist = teamsContext.TeamMembers
                                   .Where(x => x.FK_UserID == userid)
                                   .ToList();
            
            foreach (var team in teamlist) {
                Team usersteam = teamsContext.Teams
                                       .Where(x => x.TeamID == team.FK_TeamID)
                                       .Single();
                

                string coursename = teamsContext.Courses
                                          .Where(x => x.CourseID == usersteam.FK_CourseID)
                                          .Select(y => y.CourseName)
                                          .SingleOrDefault();

                var teammembers = teamsContext.TeamMembers
                                        .Where(x => x.FK_TeamID == team.FK_TeamID)
                                        .Select(y => y.FK_UserID)
                                        .ToArray();

                foreach (var memberID in teammembers) {
                    string membername = usersContext.GetUserName(memberID);
                    teammembers2.Add(membername);
                }
                
                string[] myarray = teammembers2.ToArray();
                teammembers2.Clear();

                TeamTableItem teamitem = new TeamTableItem { TeamID = usersteam.TeamID, TeamName = usersteam.TeamName, Course = coursename, TeamMembers = myarray };

                teaminfo.Add(teamitem);
            }

            usersContext.Dispose();
            teamsContext.Dispose();

            return Json(teaminfo, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Adds an entry to the Invitations table for each team in the list.
        /// </summary>
        /// <param name="sendto">Username of the user the invitation is being sent to.</param>
        /// <param name="teams">A list of teams for which the user is being invited to.</param>
        public void SendInvite(string sendto, string teams)
        {
            int recipient, sender;
            
            using (var usersContext = new UsersContext()) {
                recipient = usersContext.GetUserId(sendto);
                sender = usersContext.GetUserId(User.Identity.Name);
            }

            string[] teamArray = teams.Split(',');

            using (var teamsContext = new PM_Entities()) {
                foreach (var team in teamArray)
                {
                    Invitation inv = new Invitation { Team = Convert.ToInt32(team), Recipient = recipient, Sender = sender };
                    teamsContext.Invitations.Add(inv);
                }
                teamsContext.SaveChanges();
            }
        }

        /// <summary>
        /// Find the total number of invitations awaiting action by the current user.
        /// </summary>
        /// <returns>An integer representing the total number of invitations.</returns>
        public JsonResult GetInviteCount()
        {
            int uid, count;
            
            using (var usersContext = new UsersContext()) {
                uid = usersContext.GetUserId(User.Identity.Name);
            }

            using (var teamsContext = new PM_Entities()) {
                count = teamsContext.Invitations
                                    .Where(x => x.Recipient == uid)
                                    .Count();
            }

            return Json(count, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get a list of all invitations sent to the current user.
        /// </summary>
        /// <returns>List on InvitationListItems</returns>
        public JsonResult GetInvites()
        {
            List<InvitationListItem> inviteList = new List<InvitationListItem>();

            var usersContext = new UsersContext();
            var teamsContext = new PM_Entities();

            int uid = usersContext.GetUserId(User.Identity.Name);
            
            List<Invitation> invites = teamsContext.Invitations
                                .Where(x => x.Recipient == uid)
                                .ToList();

            foreach (var invite in invites)
            {
                string sender = usersContext.GetUserName(invite.Sender);
                string teamname = teamsContext.Teams
                                              .Where(x => x.TeamID == invite.Team)
                                              .Select(x => x.TeamName)
                                              .First();

                InvitationListItem item = new InvitationListItem { InviteID = invite.InvitationID, Sender = sender, TeamName =  teamname };

                inviteList.Add(item);
            }

            usersContext.Dispose();
            teamsContext.Dispose();

            return Json(inviteList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add the current user to a team if they accept, or just remove invitation from database if they decline.
        /// </summary>
        /// <param name="inviteid">ID of the invitation being responded to.</param>
        /// <param name="response">The users response to the invitation (ACCEPT or DECLINE).</param>
        public void InviteResponse(string inviteid, string response)
        {
            int uid, tid;

            int iid = Convert.ToInt32(inviteid);

            var teamsContext = new PM_Entities();

            using (var usersContext = new UsersContext())
            {
                uid = usersContext.GetUserId(User.Identity.Name);
            }

            if (response == "acc")
            {
                tid = teamsContext.Invitations
                                    .Where(x => x.InvitationID == iid)
                                    .Select(x => x.Team)
                                    .First();

                TeamMember newmember = new TeamMember { FK_TeamID = tid, FK_UserID = uid };
                teamsContext.TeamMembers.Add(newmember);
            }
            Invitation deleteinv = teamsContext.Invitations
                                               .Where(x => x.InvitationID == iid)
                                               .First();

            teamsContext.Invitations.Remove(deleteinv);

            teamsContext.SaveChanges();
            teamsContext.Dispose();
        }

        /// <summary>
        /// Not finished. 
        /// Retrieves list of tasks for current User
        /// </summary>
        /// TODO add logic for tasks of just one team.
        /// TODO make table allow sorting
        /// TODO add button to create tasks and delete them. Task Editing page and a button for task insertion.
        /// <returns>Json object of type TeamTableItem.</returns>
        public JsonResult GetTaskList(int teamid=0)
        {
            int userid;
            List<TaskTableItem> taskinfo = new List<TaskTableItem>();
            DateTime randTime = new DateTime(2014, 1, 30);

            var usersContext = new UsersContext();
            var teamsContext = new PM_Entities();

            userid = usersContext.GetUserId(User.Identity.Name);

            //list of all teams that this user belongs to
            var teamlist = teamsContext.TeamMembers
                                   .Where(x => x.FK_UserID == userid)
                                   .ToList();

            foreach (var team in teamlist)
            {
                //The team and all its subdata. Not useful for this function
                Team usersteam = teamsContext.Teams
                                       .Where(x => x.TeamID == team.FK_TeamID)
                                       .Single();

                //list of tasks for each team
                var tasklist = teamsContext.Tasks.Where(x => x.FKTeamID == team.FK_TeamID).ToList();
                foreach (var task in tasklist)
                {
                    TaskTableItem taskitem = new TaskTableItem { TaskID = task.TaskID, TaskName = task.TaskName, TaskStartTime = task.TaskStartTime.ToString(), TaskDeadline = task.TaskDeadline.ToString(), Status = task.FK_AssigneeID.Value };

                    taskinfo.Add(taskitem);
                }
            }

            usersContext.Dispose();
            teamsContext.Dispose();
            
            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Not finished. 
        /// Creates a task for user with user data.
        /// </summary>
        /// Todo add parameters to function
        /// Todo: remove unnecessary comments
        /// Todo: check that startTime is before deadline
        /// TODO add teamID parameter. This is not given directly by the user with a form.
        public void CreateTask(string taskName, DateTime taskStartTime, DateTime taskDeadline)
        {
            int teamID = 10;//active team, teamid10 = "team 2"

            int userid;

            var usersContext = new UsersContext();

            userid = usersContext.GetUserId(User.Identity.Name);

            using (var teamscontext = new PM_Entities())
            {
                var newTask = new Task { TaskName=taskName, TaskStartTime=taskStartTime, TaskDeadline=taskDeadline, FKTeamID=teamID};
                teamscontext.Tasks.Add(newTask);

                teamscontext.SaveChanges();
            }
        }

    }
}
