using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CustomMembershipEF.Models;

namespace CustomMembershipEF.Models
{
    public class ViewModels
    {
        public class TeamManagerViewModel
        {
            public int inviteCount { get; set; }
        }

        public class TaskManagerViewModel
        {
            public List<Team> teamList { get; set; }
        }
    }
}