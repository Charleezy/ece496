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
    public class TaskController : Controller
    {

        /// <summary>
        /// Retrieve a complete list of teams and team details for the current user.
        /// </summary>
        /// <returns>Json object of type TeamTableItem.</returns>
        public JsonResult GetTeamList()
        {
            int userid;
            List<TeamDropdownMenu> teamlist = new List<TeamDropdownMenu>();

            var usersContext = new UsersContext();
            var teamsContext = new PM_Entities();

            userid = usersContext.GetUserId(User.Identity.Name);

            var teams = teamsContext.TeamMembers
                                   .Where(x => x.FK_UserID == userid)
                                   .ToList();

            foreach (var team in teams)
            {
                Team usersteam = teamsContext.Teams
                                       .Where(x => x.TeamID == team.FK_TeamID)
                                       .Single();


                TeamDropdownMenu teamitem = new TeamDropdownMenu { TeamID = usersteam.TeamID, TeamName = usersteam.TeamName };

                teamlist.Add(teamitem);
            }

            usersContext.Dispose();
            teamsContext.Dispose();

            return Json(teamlist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Not finished. 
        /// Retrieves list of tasks for current User
        /// </summary>
        /// TODO add logic for tasks of just one team.
        /// TODO make table allow sorting
        /// TODO add button to create tasks and delete them. Task Editing page and a button for task insertion.
        /// <returns>Json object of type TeamTableItem.</returns>
        public JsonResult GetTaskList(int TeamID = 0)
        {
            int userid;
            List<TaskTableItem> taskinfo = new List<TaskTableItem>();
            DateTime randTime = new DateTime(2014, 1, 30);

            var usersContext = new UsersContext();
            var teamsContext = new PM_Entities();

            userid = usersContext.GetUserId(User.Identity.Name);

            //Just the single team selected and all the data under it
            var team = teamsContext.Teams
                                   .Where(x => x.TeamID == TeamID)
                                   .ToList();

            String assignee = "";
            String taskDescription;
            int taskDescriptionLength;
            int numDescriptChars = 200;
            foreach (var task in team[0].Tasks)
            {
                string username = usersContext.GetUserName(task.FK_AssigneeID);
				
				taskDescription = task.TaskDescription;
                taskDescriptionLength = taskDescription.Length;
                if(taskDescriptionLength < numDescriptChars)
                {
                }
                else if (taskDescriptionLength > numDescriptChars)
                {
                    taskDescription = taskDescription.Substring(0, numDescriptChars) + "...";
                }
				
				TaskTableItem item = new TaskTableItem { TaskID = task.TaskID, TaskName = task.TaskName, TaskDescription = taskDescription,TaskStartTime = task.TaskStartTime.ToString(), TaskDeadline = task.TaskDeadline.ToString(), Status = task.Status, Assignee = username };
                taskinfo.Add(item);
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
        public string CreateTask(string taskName, string taskDescription, DateTime taskStartTime, DateTime taskDeadline, int assigneeID, int teamID)
        {
            try
            {
                int userid;

                var usersContext = new UsersContext();

                userid = usersContext.GetUserId(User.Identity.Name);

                using (var teamsContext = new PM_Entities())
                {
                    //Just the single team selected and all the data under it
                    var team = teamsContext.Teams
                                           .Where(x => x.TeamID == teamID)
                                           .ToList();



                    //var newTask = new Task {TaskName = taskName, TaskDescription = taskDescription,  TaskStartTime = taskStartTime, TaskDeadline = taskDeadline, FKTeamID = teamID, Status = 0, FK_AssigneeID=assigneeID };
                    //teamsContext.Tasks.Add(newTask);

                    //teamsContext.SaveChanges();
                    int InsertedRows = teamsContext.Database.ExecuteSqlCommand("INSERT INTO PM.dbo.Tasks (\"TaskName\", \"TaskDescription\", \"TaskStartTime\", \"TaskDeadline\", \"FKTeamID\", \"Status\", \"FK_AssigneeID\") VALUES ('" + taskName + "' , '" + taskDescription + "' , '" + taskStartTime + "', '" + taskDeadline + "', + " + teamID + ", 0, " + assigneeID + ");");

                    return null;
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.Write("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:"+
                        eve.Entry.Entity.GetType().Name+ eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.Write("- Property: \"{0}\", Error: \"{1}\""+
                            ve.PropertyName+ ve.ErrorMessage);
                    }
                }
                throw;
            }
            
        }

        /// Gets the team members for a selected team
        /// Used for populating a select dropdown for creating a task
        /// </summary>
        public JsonResult GetTeamMembers(int TeamID)
        {
            int userid;
            List<TeamMembersDropdownMenu> teamMembers= new List<TeamMembersDropdownMenu>();

            var usersContext = new UsersContext();
            var teamsContext = new PM_Entities();

            userid = usersContext.GetUserId(User.Identity.Name);
            
            //Just the single team selected
            var teamlist = teamsContext.Teams
                                   .Where(x => x.TeamID == TeamID)
                                   .ToList();

            foreach (var member in teamlist[0].TeamMembers)
            {
                //TODO ask Daniele, if I'm only returning one type of parameter (maybe 3 entries of it for 3 team members), do I need to define a TeamMembersDropdown class?
                TeamMembersDropdownMenu item = new TeamMembersDropdownMenu { TeamMember = member.User.Firstname, TeamMemberID = member.User.UserID};
                teamMembers.Add(item);
            }

            /*foreach (var team in teamlist)
            {
                Team usersteam = teamsContext.Teams
                                       .Where(x => x.TeamID == team.FK_TeamID)
                                       .Single();

                //all team members for a given team
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

                TeamTableItem teamitem = new TeamTableItem { TeamMembers = myarray };

                teaminfo.Add(teamitem);
            }

            usersContext.Dispose();
            teamsContext.Dispose();*/

            return Json(teamMembers, JsonRequestBehavior.AllowGet);
        }
    }
}
