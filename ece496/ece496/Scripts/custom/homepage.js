$(document).ready(function () {
    $("#teamDetails").popover({
        html: true,
        trigger: "hover",
        content: function () {
            return $('#teamDetails_msg').html();
        }
    });
    $("#taskDetails").popover({
        html: true,
        trigger: "hover",
        content: function () {
            return $('#taskDetails_msg').html();
        }
    });
    $("#calendarDetails").popover({
        html: true,
        trigger: "hover",
        content: function () {
            return $('#calendarDetails_msg').html();
        }
    });
});