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
            var action = new DataAction(actionValues);
            var changedEvent = (Event)DHXEventsHelper.Bind(typeof(Event), actionValues);
            changedEvent.user = uid.Value;
            
            
            if (changedEvent.type != "task")
            {
                changedEvent.type = "event";
            }
            
            var data = new EventDataContext();

            switch (action.Type)
            {
                case DataActionTypes.Insert: // define here your Insert logic
                    data.Events.InsertOnSubmit(changedEvent);
                    break;
                case DataActionTypes.Delete: // define here your Delete logic
                    changedEvent = data.Events.SingleOrDefault(ev => ev.id == action.SourceId);
                    data.Events.DeleteOnSubmit(changedEvent);
                    break;
                default:// "update" // define here your Update logic
                    var eventToUpdate = data.Events.SingleOrDefault(ev => ev.id == action.SourceId);
                    DHXEventsHelper.Update(eventToUpdate, changedEvent, new List<string>() { "id" });//update all properties, except for id
                    break;
            }
            data.SubmitChanges();
            action.TargetId = changedEvent.id;

            var result = new AjaxSaveResponse(action);

            return result;
        }
    }
}