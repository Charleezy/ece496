using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomMembershipEF.Models
{
    public class TeamTable
    {
        public string TeamName { get; set; }
        public int? Course { get; set; }
        public string[] TeamMembers { get; set; }
    }
}