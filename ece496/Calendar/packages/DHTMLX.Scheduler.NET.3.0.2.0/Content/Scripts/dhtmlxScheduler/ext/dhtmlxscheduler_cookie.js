/*
Copyright Dinamenta, UAB. http://www.dhtmlx.com
To use this component please contact sales@dhtmlx.com to obtain license
*/
Scheduler.plugin(function(a){(function(){function i(a,b,c){var d=a+"="+c+(b?"; "+b:"");document.cookie=d}function j(a){var b=a+"=";if(document.cookie.length>0){var c=document.cookie.indexOf(b);if(c!=-1){c+=b.length;var d=document.cookie.indexOf(";",c);if(d==-1)d=document.cookie.length;return document.cookie.substring(c,d)}}return""}var h=!0;a.attachEvent("onBeforeViewChange",function(g,b,c,d){var f=(a._obj.id||"scheduler")+"_settings";if(h){h=!1;var e=j(f);if(e){if(!a._min_date)a._min_date=d;e=unescape(e).split("@");
e[0]=this.templates.xml_date(e[0]);var k=this.isViewExists(e[1])?e[1]:c,l=!isNaN(+e[0])?e[0]:d;window.setTimeout(function(){a.setCurrentView(l,k)},1);return!1}}var m=escape(this.templates.xml_format(d||b)+"@"+(c||g));i(f,"expires=Sun, 31 Jan 9999 22:00:00 GMT",m);return!0});var f=a._load;a._load=function(){var g=arguments;if(!a._date&&a._load_mode){var b=this;window.setTimeout(function(){f.apply(b,g)},1)}else f.apply(this,g)}})()});
