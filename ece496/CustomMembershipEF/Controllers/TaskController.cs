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
        /// Retrieve a complete list of teams and team details for the current user
        /// </summary>
        /// <returns>JSON of type TeamTableItem</returns>
        public JsonResult GetTeamList()
        {
            int userid;
            List<TeamDropdownMenu> teamlist = new List<TeamDropdownMenu>();

            var teamsContext = new PM_Entities();

            using (var usersContext = new UsersContext())
            {
                userid = usersContext.GetUserId(User.Identity.Name);
            }
            
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

            teamsContext.Dispose();

            return Json(teamlist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Retrieve a list of tasks belonging to a team
        /// </summary>
        /// <param name="TeamID">ID of the team for which to get a list of task from</param>
        /// <returns>JSON of a list of TaskTableItems</returns>
        public JsonResult GetTaskList(int TeamID = 0)
        {
            int userid;
            List<TaskTableItem> taskinfo = new List<TaskTableItem>();

            var usersContext = new UsersContext();
            var teamsContext = new PM_Entities();

            userid = usersContext.GetUserId(User.Identity.Name);

            // Get team with teamID
            var team = teamsContext.Teams
                                   .Where(x => x.TeamID == TeamID)
                                   .ToList();

            foreach (var task in team[0].Tasks)
            {
                string username = usersContext.GetUserName(task.FK_AssigneeID);
				TaskTableItem item = new TaskTableItem { TaskID = task.TaskID, TaskName = task.TaskName, TaskDescription = task.TaskDescription,TaskStartTime = task.TaskStartTime.ToString(), TaskDeadline = task.TaskDeadline.ToString(), Status = task.Status, Assignee = username };
                taskinfo.Add(item);
            }

            usersContext.Dispose();
            teamsContext.Dispose();

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        
        /// <summary>
        /// Gets a list of team members belonging to a team
        /// </summary>
        /// <param name="TeamID">ID of the team for which to get a list of team members from</param>
        /// <returns>JSON containing a list of TeamMembersDropdownMenus</returns>
        public JsonResult GetTeamMembers(int TeamID)
        {
            int userid;
            List<TeamMembersDropdownMenu> teamMembers = new List<TeamMembersDropdownMenu>();

            var usersContext = new UsersContext();
            var teamsContext = new PM_Entities();

            userid = usersContext.GetUserId(User.Identity.Name);

            // Retrieve the team with teamID
            var teamlist = teamsContext.Teams
                                   .Where(x => x.TeamID == TeamID)
                                   .ToList();

            foreach (var member in teamlist[0].TeamMembers)
            {
                TeamMembersDropdownMenu item = new TeamMembersDropdownMenu { TeamMember = member.User.Username, TeamMemberID = member.User.UserID };
                teamMembers.Add(item);
            }

            usersContext.Dispose();
            teamsContext.Dispose();

            return Json(teamMembers, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Create a task
        /// </summary>
        /// <param name="taskName">Name of the task</param>
        /// <param name="taskDescription">Description of the task</param>
        /// <param name="taskStartTime">Start time of the task</param>
        /// <param name="taskDeadline">Deadline for the task</param>
        /// <param name="assigneeID">UserID of the user assigned to the task</param>
        /// <param name="teamID">ID of the team that the task is created for</param>
        /// <returns>A message representing the status of the create procedure</returns>
        public string CreateTask(string taskName, string taskDescription, DateTime taskStartTime, DateTime taskDeadline, int assigneeID, int teamID)
        {
            if (taskStartTime >= taskDeadline)
            {
                string err = "Start Date must be less than Deadline.";
                return err;
            }

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

                    // Create a new event for the created task
                    var newEvent = new Event { user = assigneeID, text = taskName, start_date = taskStartTime, end_date = taskDeadline, TaskID = newTask.TaskID, type = "task", color = "#B5EBB5" };
                    teamsContext.Events.Add(newEvent);
                    teamsContext.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.Write("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:"+ eve.Entry.Entity.GetType().Name+ eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.Write("- Property: \"{0}\", Error: \"{1}\""+ ve.PropertyName+ ve.ErrorMessage);
                    }
                }
                throw;
            }

            //Return Successfully
            return null;
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
            if (taskStartTime >= taskDeadline)
            {
                string err = "Start Date must be less than Deadline.";
                return err;
            }

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

        /// <summary>
        /// Delete one or more tasks
        /// </summary>
        /// <param name="tasks">A list of task IDs that are to be deleted</param>
        /// <returns>A message representing the status of the delete procedure</returns>
        public string DeleteTasks(string tasks)
        {
            // Parse the task list
            string[] taskArray = tasks.Split(',');

            using (var teamsContext = new PM_Entities())
            {
                // Delete each task and its associated event
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

            // Return successfully
            return null;
        }
    }
}
