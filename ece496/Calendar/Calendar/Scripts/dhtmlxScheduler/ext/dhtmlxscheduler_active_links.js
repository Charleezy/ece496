/*
Copyright Dinamenta, UAB. http://www.dhtmlx.com
To use this component please contact sales@dhtmlx.com to obtain license
*/
Scheduler.plugin(function(a){a.config.active_link_view="day";a.attachEvent("onTemplatesReady",function(){var f=a.date.str_to_date(a.config.api_date),g=a.date.date_to_str(a.config.api_date),e=function(b,c){var c=c||b+"_scale_date",d=a.templates[c];a.templates[c]=function(a){return"<a jump_to='"+g(a)+"' href='#'>"+d(a)+"</a>"}};e("week");e("","month_day");if(this.matrix)for(var h in this.matrix)e(h);dhtmlxEvent(this._obj,"click",function(b){var c=b.target||event.srcElement,d=c.getAttribute("jump_to");
if(d)return a.setCurrentView(f(d),a.config.active_link_view),b&&b.preventDefault&&b.preventDefault(),!1})})});
