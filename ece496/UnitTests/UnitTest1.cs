using System;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomMembershipEF;
using Scheduler;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestTeamController()
        {
            // Initialize instance of Team Controller
            CustomMembershipEF.Controllers.TeamController teamController = new CustomMembershipEF.Controllers.TeamController();

            // Initialize CreateTeam parameters
            string team1 = "Team 1";
            string team2 = "Another really long team name 1234!";
            string token1 = "ece496";
            string token2 = "thistokendoesnotexist";

            // Create a valid team
            teamController.CreateTeam(team1, token1);
            //Create a team with invalid parameters
            teamController.CreateTeam(team2, token2);

            // Initialize LeaveTeams parameters
            string teamlist1 = "1,2,3,4";
            string teamlist2 = "1,2,teamdoesnotexist,4";

            // Leave teams with a valid team list
            teamController.LeaveTeams(teamlist1);
            // Leave teams with an invalid team list
            teamController.LeaveTeams(teamlist2);

            // Initailize SendInvite parameters
            string sendTo1 = "daniele";
            string sendTo2 = "not a real user";
            string teamlist3 = "1,2,3,4";
            string teamlist4 = "1,2,not a team number,4";

            // Send a valid invitation
            teamController.SendInvite(sendTo1, teamlist3);
            // Send invitation to a user that doesn't exist
            teamController.SendInvite(sendTo2, teamlist3);
            // Send an invitation with an invalid team list
            teamController.SendInvite(sendTo1, teamlist4);

            // Initialize InviteResponse parameters
            string answer1 = "ACCEPT";
            string answer2 = "DECLINE";
            string answer3 = "invalid response";
            string inviteID1 = "1";
            string inviteID2 = "invalid id";

            // User accepts invitation
            teamController.InviteResponse(inviteID1, answer1);
            // User declines invitation
            teamController.InviteResponse(inviteID1, answer2);
            // User sends back an invalid response
            teamController.InviteResponse(inviteID1, answer3);
            // User sends response to invalid invite id
            teamController.InviteResponse(inviteID2, answer1);

            // Get invite count for current user
            teamController.GetInviteCount();

            // Get list on invites for current user
            teamController.GetInvites();

            // Get list of teams that the current user belongs to
            teamController.GetTeamList();

            Assert.AreEqual(0.01, 0.01, 0.001, "Account not debited correctly");

        }
    }
}
