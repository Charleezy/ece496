var spinner;

var spinoptions = {
    lines: 10, // The number of lines to draw
    length: 10, // The length of each line
    width: 6, // The line thickness
    radius: 10, // The radius of the inner circle
    corners: 0.9, // Corner roundness (0..1)
    rotate: 45, // The rotation offset
    direction: 1, // 1: clockwise, -1: counterclockwise
    color: '#000', // #rgb or #rrggbb or array of colors
    speed: 1, // Rounds per second
    trail: 67, // Afterglow percentage
    shadow: false, // Whether to render a shadow
    hwaccel: false, // Whether to use hardware acceleration
    className: 'spinner', // The CSS class to assign to the spinner
    zIndex: 2e9, // The z-index (defaults to 2000000000)
    top: '50px', // Top position relative to parent in px
    left: 'auto' // Left position relative to parent in px
};

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
        $('#no_results').hide();
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
        // Clear previous result list
        $('#results li').each(function () {
            $(this).remove();
        });

        $('#no_results').hide();

        var targ = document.getElementById('results_body');
        spinner = new Spinner(spinoptions).spin(targ);

        $.ajax({
            url: '/Calendar/SearchResults',
            data: { teamID: teamID, startDate: startDate, endDate: endDate, numResults: numResults, timeRequired: timeRequired },
            success: function (data) {
                spinner.stop();

                if (data.length) {
                    if (data[0] == "Error") {
                        alert(data[1]);
                    }
                    else {
                        for (var i = 0; i < data.length; i++) {
                            $("#results").append('<li>' + data[i] + '</li>');
                        }
                    }
                    
                }
                else {
                    $('#no_results').show();
                }
                
            }
        });
    }
}