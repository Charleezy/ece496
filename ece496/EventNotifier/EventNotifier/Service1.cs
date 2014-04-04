using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Objects.SqlClient;
using System.Timers;

namespace EventNotifier
{
    public partial class Service1 : ServiceBase
    {
        Timer createOrderTimer;

        public Service1()
        {
            InitializeComponent();
        }

        public void SendMail(string sendTo, string subject, string body)
        {
            MailMessage message = new System.Net.Mail.MailMessage();
            string fromEmail = "groupup.ece496@gmail.com";
            string fromPW = "groupup_ece496";
            message.From = new MailAddress(fromEmail);
            message.To.Add(sendTo);
            message.Subject = subject;
            message.Body = body;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(fromEmail, fromPW);

            smtpClient.Send(message.From.ToString(), message.To.ToString(), message.Subject, message.Body);

            smtpClient.Dispose();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            createOrderTimer = new Timer();
            createOrderTimer.Elapsed += new System.Timers.ElapsedEventHandler(PollDatabase);
            createOrderTimer.Interval = 900000; // 15 min
            createOrderTimer.Enabled = true;
            createOrderTimer.AutoReset = true;
            createOrderTimer.Start();
        }

        public void PollDatabase(object sender, ElapsedEventArgs args)
        {
            RegexUtilities myRegEx = new RegexUtilities();

            using (var taskContext = new TaskDataContext())
            {
                // Convert DateTime.Now from UTC to EST
                List<Task> notify_tasks = taskContext.Tasks.Where(x => (x.TaskDeadline <= DateTime.Now.AddHours(-3) && x.TaskDeadline >= DateTime.Now.AddHours(-4)) && (x.alerted == false || x.alerted == null)).ToList();
                
                foreach (var task in notify_tasks)
                {
                    using (var userContext = new UserDataContext())
                    {
                        // If the task is assigned to someone, send them an email
                        if (task.FK_AssigneeID != null || task.FK_AssigneeID != 0)
                        {
                            string sendTo = userContext.Users.Where(x => x.UserID == task.FK_AssigneeID).Select(x => x.Email).Single();
                            
                            if (!myRegEx.IsValidEmail(sendTo))
                            {
                                continue;
                            }
                            string subject = "Reminder: " + task.TaskName;
                            string body = "Just a friendly reminder that the deadline for your task,  " + task.TaskName + ", is approaching.\r\n\r\n -The GroupUp team";
                            SendMail(sendTo, subject, body);

                            task.alerted = true;
                        }
                    }
                }
                taskContext.SubmitChanges();
            }
        }

        protected override void OnStop()
        {

        }
    }
}
