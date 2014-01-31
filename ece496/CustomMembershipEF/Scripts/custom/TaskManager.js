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
    top: '20px', // Top position relative to parent in px
    left: 'auto' // Left position relative to parent in px
};

$(document).ready(function () {
    
    var target = document.getElementById('myTasks').getElementsByTagName('tbody')[0];
    spinner = new Spinner(spinoptions).spin(target);

    populateTaskList();

});

var populateTaskList = function () {
    $.ajax({
        url: '/User/GetTaskList',
        success: function (data) {
            var table = document.getElementById("myTeams").getElementsByTagName('tbody')[0];

            if (data.length == 0) {

            }

            for (var i = 0, len = data.length; i < len; ++i) {
                var team = data[i];
                var newRow = table.insertRow(table.rows.length);

                var select = newRow.insertCell(0);
                var name = newRow.insertCell(1);
                var course = newRow.insertCell(2);
                var members = newRow.insertCell(3);

                var selectbox = document.createElement('input');
                selectbox.className = 'test';
                selectbox.type = 'checkbox';
                selectbox.id = team.TeamID;

                var memberconcat = "";
                for (var j = 0; j < team.TeamMembers.length; j++) {
                    if (team.TeamMembers.length - j > 1) {
                        memberconcat = memberconcat.concat(team.TeamMembers[j]);
                        memberconcat = memberconcat.concat(", ");
                    }
                    else {
                        memberconcat = memberconcat.concat(team.TeamMembers[j]);
                    }

                }

                var nametext = document.createTextNode(team.TeamName);
                var coursetext = document.createTextNode(team.Course);
                var memberstext = document.createTextNode(memberconcat);

                select.appendChild(selectbox);
                name.appendChild(nametext);
                course.appendChild(coursetext);
                members.appendChild(memberstext);
            }
            spinner.stop();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("error");
        }
    });
}