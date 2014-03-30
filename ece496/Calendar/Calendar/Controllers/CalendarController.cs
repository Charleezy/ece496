using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DHTMLX.Scheduler;
using DHTMLX.Common;
using DHTMLX.Scheduler.Data;
using Calendar.Models;

namespace Calendar.Controllers
{
    public class CalendarController : Controller
    {
        static int? uid;

        public ActionResult Index(int? userid)
        {
            if (userid.HasValue)
            {
                uid = userid.Value;
            }

            var scheduler = new DHXScheduler(this);
            scheduler.Extensions.Add(SchedulerExtensions.Extension.Readonly);

            scheduler.Skin = DHXScheduler.Skins.Glossy;
            scheduler.InitialDate = DateTime.Now;

            scheduler.Config.multi_day = true;//render multiday events

            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;
            scheduler.Data.DataProcessor.UpdateFieldsAfterSave = true;

            scheduler.Config.first_hour = 9;
            
            return View(scheduler);
        }

        public ContentResult Data()
        {
            var data = new SchedulerAjaxData();

            if (uid != null)
            {
                data = new SchedulerAjaxData(
                        new EventDataContext().Events.Where(x => x.user == uid)
                    );
            }

            return (ContentResult)data;
        }

        public ContentResult Save(int? id, FormCollection actionValues)
        {
            var event_data = new EventDataContext();
            var task_data = new TaskDataContext();
            Task this_task;

            var action = new DataAction(actionValues);
            var changedEvent = (Event)DHXEventsHelper.Bind(typeof(Event), actionValues);
            changedEvent.user = uid.Value;

            if (changedEvent.type != "task")
            {
                changedEvent.type = "event";
            }      
            
            switch (action.Type)
            {
                case DataActionTypes.Insert: // define here your Insert logic
                    event_data.Events.InsertOnSubmit(changedEvent);
                    break;
                case DataActionTypes.Delete: // define here your Delete logic
                    if (changedEvent.type == "task")
                    {
                        this_task = task_data.Tasks.Where(t => t.TaskID == changedEvent.TaskID).Single();
                        task_data.Tasks.DeleteOnSubmit(this_task);
                    }
                    changedEvent = event_data.Events.SingleOrDefault(ev => ev.id == action.SourceId);
                    event_data.Events.DeleteOnSubmit(changedEvent);
                    break;
                default:// "update" // define here your Update logic
                    if (changedEvent.type == "task")
                    {
                        this_task = task_data.Tasks.Where(t => t.TaskID == changedEvent.TaskID).Single();
                        this_task.TaskName = changedEvent.text;
                        this_task.TaskStartTime = changedEvent.start_date;
                        this_task.TaskDeadline = changedEvent.end_date;
                    }
                    var eventToUpdate = event_data.Events.SingleOrDefault(ev => ev.id == action.SourceId);
                    DHXEventsHelper.Update(eventToUpdate, changedEvent, new List<string>() { "id" });//update all properties, except for id
                    break;
            }
            event_data.SubmitChanges();
            task_data.SubmitChanges();
            action.TargetId = changedEvent.id;

            var result = new AjaxSaveResponse(action);

            return result;
        }
    }
}