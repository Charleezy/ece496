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
        public string Course { get; set; }
        public string[] TeamMembers { get; set; }
    }

    public class InvitationListItem
    {
        public string Sender { get; set; }
        public string TeamName { get; set; }
    }
}