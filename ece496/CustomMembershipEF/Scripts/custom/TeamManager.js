var spinner;

$(document).ready(function () {


    var options = {
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
    var target = document.getElementById('myTeams').getElementsByTagName('tbody')[0];
    spinner = new Spinner(options).spin(target);

    populateTeamList();

    //When closing create team modal, clear fields
    $('#createTeamModal').on('hidden.bs.modal', function () {
        $('#teamName').parent('div').removeClass('has-error');
        $('#teamName_err').hide();
        $('#teamName').val("");

        $('#courseToken').parent('div').removeClass('has-error');
        $('#courseToken').val("");
    })
});

var populateTeamList = function () {
    $.ajax({
        url: '/User/GetTeamList',
        success: function (data) {
            var table = document.getElementById("myTeams").getElementsByTagName('tbody')[0];

            for (var i = 0, len = data.length; i < len; ++i) {
                var team = data[i];
                var newRow = table.insertRow(table.rows.length);

                var name = newRow.insertCell(0);
                var course = newRow.insertCell(1);
                var members = newRow.insertCell(2);

                var memberconcat = "";
                for (var j = 0; j < team.TeamMembers.length; j++) {
                    memberconcat = memberconcat.concat(team.TeamMembers[j]);
                }

                var nametext = document.createTextNode(team.TeamName);
                var coursetext = document.createTextNode(team.Course);
                var memberstext = document.createTextNode(memberconcat);

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

var createTeam = function () {
    var name = document.forms['createteam-modal-form'].teamName.value;
    var token = document.forms['createteam-modal-form'].courseToken.value;

    $.ajax({
        url: '/User/CreateTeam',
        data: { teamname: name, coursetoken: token },
        success: function () {
            $('#createTeamModal').modal('hide');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown + textStatus);
        }
    });
}
