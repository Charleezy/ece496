using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomMembershipEF.Models
{
    public class TeamTableItem
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string[] TeamMembers { get; set; }
    }

    public class TaskTableItem
    {
        public int? TaskID { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string TaskStartTime { get; set; }
        public string TaskDeadline { get; set; }
        public int? Status { get; set; }
        public string Assignee { get; set; }
    }

    public class InvitationListItem
    {
        public int InviteID { get; set; }
        public string Sender { get; set; }
        public string TeamName { get; set; }
    }

    public class TeamDropdownMenu
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }
    }

    //Used for returning json of teamMembers to populate assignees dropdown
    public class TeamMembersDropdownMenu
    {
        public string TeamMember { get; set; }
        public int TeamMemberID { get; set; }
    }
}