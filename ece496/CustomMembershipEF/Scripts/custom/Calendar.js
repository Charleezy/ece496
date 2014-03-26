$(function () {
    $('#earliestDate').datetimepicker();
    $('#latestDate').datetimepicker();
});

$(document).ready(function () {
    $('#earliestDate').focus(function () {
        $('#search_err').hide();
    });

    $('#latestDate').focus(function () {
        $('#search_err').hide();
    });
});

var search = function () {
    var teamID = $('#teamList').val();
    var startDate = $('#earliestDate').val();
    var endDate = $('#latestDate').val();
    var numResults = $('#numResults').val();

    if (startDate == "" || startDate == null || endDate == "" || endDate == null) {
        $('#search_err').show();
    }
    else {
        $.ajax({
            url: '/Calendar/SearchResults',
            data: { teamID: teamID, startDate: startDate, endDate: endDate, numResults: numResults },
            success: function (msg) {
                alert("success");
            }
        });
    }
}