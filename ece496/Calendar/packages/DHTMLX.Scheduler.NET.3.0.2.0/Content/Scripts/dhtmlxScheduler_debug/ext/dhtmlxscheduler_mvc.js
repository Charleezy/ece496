Scheduler.plugin(function(scheduler){
	(function(){

	//remove private properties
	function sanitize(ev){
		var obj = {};
		for (var key in ev)
			if (key.indexOf("_") !== 0)
				obj[key] = ev[key];

		return obj;
	}

	var update_timer;
	function update_view(){
		clearTimeout(update_timer);
		update_timer = setTimeout(function(){
			scheduler.updateView();
		},1);
	}

	function _start_ext_load(cal){
		cal._loading = true;
		cal._not_render = true;

		cal.callEvent("onXLS", []);
	}
	function _finish_ext_load(cal){
		cal._not_render = false;
		if (cal._render_wait) 
			cal.render_view_data();
		cal._loading = false;

		cal.callEvent("onXLE", []);
	}

scheduler.backbone = function(events){
	events.bind("reset", function(){
		scheduler.clearAll();
		scheduler.parse(events.toJSON(), "json");
	});
	events.bind("change", function(model, info){
		//special handling for id change
		if (info.changes && info.changes.id){
			var old_id = model.previous("id");
			scheduler.changeEventId(old_id, model.id);
		}

		var id = model.id;
		scheduler._init_event( scheduler._events[id] = model.toJSON() );
		update_view();
	});
	events.bind("remove", function(model, changes){
		if (scheduler._events[model.id])
			scheduler.deleteEvent(model.id);
	});
	events.bind("add", function(model, changes){
		if (!scheduler._events[model.id]){
			var ev =  model.toJSON();
			scheduler._init_event(ev); 
			scheduler.addEvent(ev);
		}
	});

	events.bind("request", function(obj){
		if (obj instanceof Backbone.Collection)
			_start_ext_load(scheduler);
	});
	events.bind("sync", function(obj){
		if (obj instanceof Backbone.Collection)
			_finish_ext_load(scheduler);
	});
	events.bind("error", function(obj){
		if (obj instanceof Backbone.Collection)
			_finish_ext_load(scheduler);
	});


	scheduler.attachEvent("onEventCreated", function(id){
		var ev = new events.model(scheduler.getEvent(id));
		scheduler._events[id] = ev.toJSON();

		return true;
	});

	scheduler.attachEvent("onEventAdded", function(id){
		if (!events.get(id))
			events.add( new events.model(sanitize(scheduler.getEvent(id))) );
		
		return true;
	});
	scheduler.attachEvent("onEventChanged", function(id){
		var ev = events.get(id);
		var upd = sanitize(scheduler.getEvent(id));
		ev.set(upd);

		return true;
	});
	scheduler.attachEvent("onEventDeleted", function(id){
		if (events.get(id))
			events.remove(id);
		return true;
	});
};

})();
});