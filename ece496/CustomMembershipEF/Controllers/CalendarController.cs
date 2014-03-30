using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomMembershipEF.Models;
using CustomMembershipEF.Contexts;
using System.Globalization;

namespace CustomMembershipEF.Controllers
{
    public class CalendarController : Controller
    {
        /// <summary>
        /// Rounds the DateTime value up to the nearest hour
        /// </summary>
        /// <param name="dateTime">The DateTime to be rounded up</param>
        /// <returns>A new DateTime that is rounded up</returns>
        public DateTime RoundUp(DateTime dateTime)
        {
            // Don't round if we are already flat on the hour
            if (dateTime.Minute == 0)
                return dateTime;
            else
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour + 1, 0, 0, dateTime.Kind);
        }

        /// <summary>
        /// Rounds the DateTime value down to the nearest hour
        /// </summary>
        /// <param name="dateTime">The DateTime to be rounded down</param>
        /// <returns>A new DateTime that is rounded down</returns>
        public DateTime RoundDown(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, dateTime.Kind);
        }

        /// <summary>
        /// Searchs for common breaks in one or more schedules
        /// </summary>
        /// <param name="teamID">ID of the team for which to schedule a meeting for</param>
        /// <param name="startDate">Earliest date to hold a meeting</param>
        /// <param name="endDate">Latest date to hold a meeting</param>
        /// <param name="numResults">Max number of results to be returned</param>
        /// <param name="timeRequired">Amount of time in hours required for the meeting</param>
        /// <returns>A list of possible meeting time</returns>
        public JsonResult SearchResults(int teamID, DateTime startDate, DateTime endDate, int numResults, int timeRequired)
        {
            DateTime start_output;
            DateTime end_output;

            if (startDate >= endDate)
            {
                List<string> err = new List<string> { "Error", "Earliest date must be less than latest date." };
                return Json(err, JsonRequestBehavior.AllowGet);
            }

            if (DateTime.TryParse(startDate.ToString(), out start_output) || DateTime.TryParse(endDate.ToString(), out end_output))
            {
                // Dates are valid
            }
            else
            {
                List<string> err2 = new List<string> { "Error", "Your date formats are incorrect. Please try again." };
                return Json(err2, JsonRequestBehavior.AllowGet);
            }

            int count = 0, flag = 0, event_count = 0;
            DateTime startDate_rounded, endDate_rounded, endDate_compare;
            List<string> results = new List<string>();
            List<TeamMember> teamMembers = new List<TeamMember>();

            // Find team members of teamID
            using (var teamsContext = new PM_Entities())
            {
                teamMembers = teamsContext.TeamMembers
                                            .Where(x => x.FK_TeamID == teamID)
                                            .ToList();
            
                // Round datetimes for easier calculations
                startDate_rounded = RoundUp(startDate);
                endDate_rounded = RoundDown(endDate);
                endDate_compare = endDate_rounded.AddHours(-timeRequired);
            
                while (count < numResults && startDate_rounded <= endDate_compare)
                {
                    // Ignore 12AM -> 9AM
                    if ((startDate_rounded.Hour >= 0 && startDate_rounded.Hour < 9) || startDate_rounded.Hour + timeRequired > 24)
                    {
                        startDate_rounded = startDate_rounded.AddHours(1);
                    }
                    else
                    {
                        flag = 0;
                        event_count = 0;
                        DateTime endDate_temp = startDate_rounded.AddHours(timeRequired);

                        // Check team member schedules and compare with current startDate_rounded -> endDate_temp
                        foreach (var member in teamMembers)
                        {
                            // Check if there is an event where start_date > startDate_rounded and start_date < endDate_temp
                            // or if there is an event where end_date > startDate_rounded and end_date < endDate_temp
                            // If either is true, this time slot will not work for the team
                            event_count = teamsContext.Events
                                                        .Where(x => x.type == "event" &&
                                                                    x.user == member.FK_UserID &&
                                                                   ((x.start_date > startDate_rounded && x.start_date < endDate_temp) ||
                                                                   (x.end_date > startDate_rounded && x.end_date < endDate_temp) ||
                                                                   (x.start_date < startDate_rounded && x.end_date > endDate_temp))
                                                              )
                                                        .Count();
                            
                            if (event_count != 0)
                            {
                                flag = 1;
                                break;
                            }
                        }

                        // If no team member was busy during this time period, add result and increment count
                        if (flag == 0)
                        {
                            // Format string so it is in readable date format
                            string newResult = startDate_rounded.DayOfWeek.ToString() + " " +
                                                startDate_rounded.ToString("MMM", CultureInfo.InvariantCulture) + " " +
                                                startDate_rounded.Day.ToString() + ", " +
                                                startDate_rounded.Hour.ToString() + ":00 - " +
                                                (startDate_rounded.Hour + timeRequired).ToString() + ":00";

                            results.Add(newResult);
                            count++;
                        }

                        startDate_rounded = startDate_rounded.AddHours(1);
                    }
                }
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}
