Scheduler.plugin(function(scheduler){
	scheduler.config.active_link_view = "day";
scheduler.attachEvent("onTemplatesReady", function() {
	var s_d = scheduler.date.str_to_date(scheduler.config.api_date);
	var d_s = scheduler.date.date_to_str(scheduler.config.api_date);

	var do_wrapper = function(key, fullname){
		fullname = fullname || (key+"_scale_date");
		var week_x = scheduler.templates[fullname];
		scheduler.templates[fullname] = function(date) {
			return "<a jump_to='" + d_s(date) + "' href='#'>" + week_x(date) + "</a>";
		};
	};

	do_wrapper("week");
	do_wrapper("", "month_day");
	if (this.matrix){
		for (var key in this.matrix)
			do_wrapper(key);
	}
	
	dhtmlxEvent(this._obj, "click", function(e) {
		var start = e.target || event.srcElement;
		var to = start.getAttribute("jump_to");
		if (to) {
			scheduler.setCurrentView(s_d(to), scheduler.config.active_link_view);
			if (e && e.preventDefault)
				e.preventDefault();
			return false;
		}
	})
});
});