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

            String taskDescription;
            //Stuff used to shrink length of task description. Removed because editing gets task description from front end.
            /*int taskDescriptionLength;
            int numDescriptChars = 200;*/
            foreach (var task in team[0].Tasks)
            {
                string username = usersContext.GetUserName(task.FK_AssigneeID);
				taskDescription = task.TaskDescription;
                /*
                taskDescriptionLength = taskDescription.Length;
                if(taskDescriptionLength < numDescriptChars)
                {
                }
                else if (taskDescriptionLength > numDescriptChars)
                {
                    taskDescription = taskDescription.Substring(0, numDescriptChars) + "...";
                }*/
				
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

                using (var usersContext = new UsersContext())
                {
                    userid = usersContext.GetUserId(User.Identity.Name);
                }

                using (var teamsContext = new PM_Entities())
                {
                    //Just the single team selected and all the data under it
                    var team = teamsContext.Teams
                                           .Where(x => x.TeamID == teamID)
                                           .ToList();

                    // Create a new task in Tasks table
                    var newTask = new Task { TaskName = taskName, TaskDescription = taskDescription,  TaskStartTime = taskStartTime, TaskDeadline = taskDeadline, FKTeamID = teamID, Status = 0, FK_AssigneeID = assigneeID };
                    teamsContext.Tasks.Add(newTask);
                    teamsContext.SaveChanges();
                    //int InsertedRows = teamsContext.Database.ExecuteSqlCommand("INSERT INTO PM.dbo.Tasks (\"TaskName\", \"TaskDescription\", \"TaskStartTime\", \"TaskDeadline\", \"FKTeamID\", \"Status\", \"FK_AssigneeID\") VALUES ('" + taskName + "' , '" + taskDescription + "' , '" + taskStartTime + "', '" + taskDeadline + "', + " + teamID + ", 0, " + assigneeID + ");");

                    // Create a new event for the created task
                    var newEvent = new Event { user = assigneeID, text = taskName, start_date = taskStartTime, end_date = taskDeadline, TaskID = newTask.TaskID, type = "task", color = "#B5EBB5" };
                    teamsContext.Events.Add(newEvent);
                    teamsContext.SaveChanges();
                }
                
                //Return Successfully
                return null;
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

        /// <summary>
        /// Updates an existing task with the provided parameters
        /// </summary>
        /// <param name="taskID">TaskID of the updated task</param>
        /// <param name="taskName">Name of the updated task</param>
        /// <param name="taskDescription">Description of the updated task</param>
        /// <param name="taskStartTime">Start time of the updated task</param>
        /// <param name="taskDeadline">Deadline of the updated task</param>
        /// <param name="status">Current status of the updated task</param>
        /// <param name="assigneeID">UserID of the user assigned to the task</param>
        /// <returns>A message representing the status of the update procedure</returns>
        public string UpdateTask(int taskID, string taskName, string taskDescription, DateTime taskStartTime, DateTime taskDeadline, int status, int assigneeID)
        {
            try
            {
                using (var tasksContext = new PM_Entities())
                {
                    // Get task with taskID and update its parameters
                    Task task = tasksContext.Tasks
                                            .Where(x => x.TaskID == taskID)
                                            .FirstOrDefault();

                    task.TaskName = taskName;
                    task.TaskDescription = taskDescription;
                    task.TaskStartTime = taskStartTime;
                    task.TaskDeadline = taskDeadline;
                    task.Status = status;
                    task.FK_AssigneeID = assigneeID;
                    
                    // Update the task's corresponding event
                    Event updated_event = tasksContext.Events
                                                      .Where(x => x.TaskID == taskID)
                                                      .FirstOrDefault();

                    updated_event.text = taskName;
                    updated_event.start_date = taskStartTime;
                    updated_event.end_date = taskDeadline;
                    updated_event.user = assigneeID;

                    tasksContext.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.Write("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" + eve.Entry.Entity.GetType().Name + eve.Entry.State);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.Write("- Property: \"{0}\", Error: \"{1}\"" + ve.PropertyName + ve.ErrorMessage);
                    }
                }
                throw;
            }

            //Return Successfully
            return null;
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
                TeamMembersDropdownMenu item = new TeamMembersDropdownMenu { TeamMember = member.User.Username, TeamMemberID = member.User.UserID};
                teamMembers.Add(item);
            }


            usersContext.Dispose();
            teamsContext.Dispose();

            return Json(teamMembers, JsonRequestBehavior.AllowGet);
        }

        public string DeleteTasks(string tasks)
        {
            string[] taskArray = tasks.Split(',');

            using (var teamsContext = new PM_Entities())
            {
                foreach (var task in taskArray)
                {
                    int taskID = Convert.ToInt32(task);

                    Task delete_task = teamsContext.Tasks.Where(x => x.TaskID == taskID).Single();
                    teamsContext.Tasks.Remove(delete_task);

                    Event associated_event = teamsContext.Events.Where(x => x.TaskID == taskID).Single();
                    teamsContext.Events.Remove(associated_event);
                }
                teamsContext.SaveChanges();
            }

            return null;
        }
    }
}
