using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CustomMembershipEF.Models;
using CustomMembershipEF.Contexts;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Validation;

namespace CustomMembershipEF.Controllers
{
    public class TeamController : Controller
    {
        /// <summary>
        /// Retrieve a complete list of teams and team details for the current user.
        /// </summary>
        /// <returns>Json object of type TeamTableItem.</returns>
        public JsonResult GetTeamList()
        {
            int userid;
            List<TeamTableItem> teaminfo = new List<TeamTableItem>();
            List<string> teammembers = new List<string>();

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

                var course = teamsContext.Courses
                                          .Where(x => x.CourseID == usersteam.FK_CourseID)
                                          .Single();

                var members = teamsContext.TeamMembers
                                        .Where(x => x.FK_TeamID == team.FK_TeamID)
                                        .Select(y => y.FK_UserID)
                                        .ToArray();

                foreach (var memberID in members)
                {
                    string membername = usersContext.GetUserName(memberID);
                    teammembers.Add(membername);
                }

                string[] myarray = teammembers.ToArray();
                teammembers.Clear();

                TeamTableItem teamitem = new TeamTableItem { TeamID = usersteam.TeamID, TeamName = usersteam.TeamName, CourseCode = course.SchoolCourseCode, CourseName = course.CourseName, TeamMembers = myarray };

                teaminfo.Add(teamitem);
            }

            usersContext.Dispose();
            teamsContext.Dispose();

            return Json(teaminfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adds a new entry to the Teams table. Populate tasks with template tasks for coursetoken.
        /// </summary>
        /// <param name="teamname">Name of the team.</param>
        /// <param name="coursetoken">A code mapping to a course.</param>
        public string CreateTeam(string teamname, string coursetoken)
        {
            int uid;

            try
            {
                using (var usersContext = new UsersContext())
                {
                    uid = usersContext.GetUserId(User.Identity.Name);
                }

                using (var teamscontext = new PM_Entities())
                {
                    var course = teamscontext.Courses
                                    .Where(x => x.CourseToken == coursetoken)
                                    .SingleOrDefault();

                    if (course == null)
                    {
                        return "The course token you entered does not exist. Please try again.";
                    }

                    var newTeam = new Team { TeamName = teamname, FK_CourseID = course.CourseID };
                    teamscontext.Teams.Add(newTeam);
                    teamscontext.SaveChanges();
                    var newMember = new TeamMember { FK_UserID = uid, FK_TeamID = newTeam.TeamID };
                    teamscontext.TeamMembers.Add(newMember);
                    teamscontext.SaveChanges();

                    // Add all template tasks for coursetoken into tasks table
                    List<CourseTemplate> template_tasks = teamscontext.CourseTemplates
                                                                .Where(x => x.CourseID == course.CourseID)
                                                                .ToList();

                    foreach (var task in template_tasks)
                    {
                        var newTask = new Task { TaskName = task.TaskName, TaskDescription = task.TaskDescription, FKTeamID = newTeam.TeamID, TaskDeadline = task.TaskDeadline, Status = 0 };
                        teamscontext.Tasks.Add(newTask);
                        teamscontext.SaveChanges();
                        var newEvent = new Event { user = 0, text = task.TaskName, start_date = DateTime.Now, end_date = task.TaskDeadline, TaskID = newTask.TaskID, type = "task", color = "#B5EBB5" };
                        teamscontext.Events.Add(newEvent);
                        teamscontext.SaveChanges();
                    }

                    // Returns successfully
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Adds an entry to the Invitations table for each team in the list.
        /// </summary>
        /// <param name="sendto">Username of the user the invitation is being sent to.</param>
        /// <param name="teams">A list of teams for which the user is being invited to.</param>
        public string SendInvite(string sendto, string teams)
        {
            int recipient, sender;

            try
            {
                using (var usersContext = new UsersContext())
                {
                    recipient = usersContext.GetUserId(sendto);
                    sender = usersContext.GetUserId(User.Identity.Name);

                    if (recipient < 0)
                    {
                        return "The username you entered does not exist. Please try again.";
                    }
                }

                string[] teamArray = teams.Split(',');

                using (var teamsContext = new PM_Entities())
                {
                    foreach (var team in teamArray)
                    {
                        int team_num = Convert.ToInt32(team);
                        // If an invite to this team has already been sent to recipient or recipient is already apart of team, don't send it again
                        if (!teamsContext.Invitations.Any(x => x.Team == team_num && x.Recipient == recipient)
                            && !teamsContext.TeamMembers.Any(x => x.FK_TeamID == team_num && x.FK_UserID == recipient))
                        {
                            Invitation inv = new Invitation { Team = team_num, Recipient = recipient, Sender = sender };
                            teamsContext.Invitations.Add(inv);
                        }
                    }
                    teamsContext.SaveChanges();
                }

                // Returns successfully
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Find the total number of invitations awaiting action by the current user.
        /// </summary>
        /// <returns>An integer representing the total number of invitations.</returns>
        public JsonResult GetInviteCount()
        {
            int uid, count;

            using (var usersContext = new UsersContext())
            {
                uid = usersContext.GetUserId(User.Identity.Name);
            }

            using (var teamsContext = new PM_Entities())
            {
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

                InvitationListItem item = new InvitationListItem { InviteID = invite.InvitationID, Sender = sender, TeamName = teamname };

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

        public string LeaveTeams(string teams)
        {
            int uid;
            try
            {
                using (var usersContext = new UsersContext())
                {
                    uid = usersContext.GetUserId(User.Identity.Name);
                }

                string[] teamArray = teams.Split(',');

                using (var teamsContext = new PM_Entities())
                {
                    foreach (var team in teamArray)
                    {
                        int team_num = Convert.ToInt32(team);
                        int member_count = teamsContext.TeamMembers.Count(x => x.FK_TeamID == team_num);

                        // If there are still other members in the team, just remove this user
                        if (member_count > 1)
                        {
                            TeamMember mem = teamsContext.TeamMembers.Where(x => x.FK_TeamID == team_num && x.FK_UserID == uid).First();
                            teamsContext.TeamMembers.Remove(mem);
                        }
                        // If this is the only user in the team, delete the team, any associated tasks, and pending invites
                        else
                        {
                            TeamMember mem = teamsContext.TeamMembers.Where(x => x.FK_TeamID == team_num && x.FK_UserID == uid).First();
                            teamsContext.TeamMembers.Remove(mem);

                            List<Invitation> invitelist = teamsContext.Invitations.Where(x => x.Team == team_num).ToList();
                            foreach (Invitation invite in invitelist)
                            {
                                teamsContext.Invitations.Remove(invite);
                            }

                            List<Task> tasklist = teamsContext.Tasks.Where(x => x.FKTeamID == team_num).ToList();
                            foreach (Task task in tasklist)
                            {
                                teamsContext.Tasks.Remove(task);
                                if (teamsContext.Events.Any(x => x.TaskID == task.TaskID))
                                {
                                    Event associated_event = teamsContext.Events
                                                                .Where(x => x.TaskID == task.TaskID)
                                                                .Single();
                                    teamsContext.Events.Remove(associated_event);
                                }
                            }

                            Team t = teamsContext.Teams.Where(x => x.TeamID == team_num).First();
                            teamsContext.Teams.Remove(t);
                        }
                    }

                    teamsContext.SaveChanges();
                }

                // Returns successfully
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
