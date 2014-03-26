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

    $('#clear_results').click(function () {
        $('#results li').each(function () {
            $(this).remove();
        });
    });
});

var search = function () {
    var teamID = $('#teamList').val();
    var startDate = $('#earliestDate').val();
    var endDate = $('#latestDate').val();
    var numResults = $('#numResults').val();
    var timeRequired = $('#timeRequired').val();
    
    if (startDate == "" || startDate == null || endDate == "" || endDate == null) {
        $('#search_err').show();
    }
    else {
        $.ajax({
            url: '/Calendar/SearchResults',
            data: { teamID: teamID, startDate: startDate, endDate: endDate, numResults: numResults, timeRequired: timeRequired },
            success: function (data) {
                // Clear previous result list
                $('#results li').each(function () {
                    $(this).remove();
                });

                for (var i = 0; i < data.length; i++) {
                    $("#results").append('<li>' + data[i] + '</li>');
                }
            }
        });
    }
}