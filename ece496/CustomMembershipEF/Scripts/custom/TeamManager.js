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
    
    var target = document.getElementById('myTeams').getElementsByTagName('tbody')[0];
    spinner = new Spinner(spinoptions).spin(target);

    populateTeamList();

    $('#myTeams').on('click', 'input[type=checkbox]', function () {
        if ($('#myTeams input:checkbox:checked').length > 0) {
            $('#sendInvite_button').removeClass('disabled');
        }
        else {
            $('#sendInvite_button').addClass('disabled');
        }
    });

    //When closing create team modal, clear fields
    $('#createTeamModal').on('hidden.bs.modal', function () {
        $('#teamName').parent('div').removeClass('has-error');
        $('#teamName_err').hide();
        $('#teamName').val("");

        $('#courseToken').parent('div').removeClass('has-error');
        $('#courseToken').val("");
    });

    //When closing send invite modal, clear fields
    $('#sendInviteModal').on('hidden.bs.modal', function () {
        $('#userName').parent('div').removeClass('has-error');
        $('#userName_err').hide();
        $('#userName').val("");
    });

    $('#viewInviteModal').on('hidden.bs.modal', function () {
        $('li').each(function () {
            $(this).remove();
        });
        $('#accept').addClass('disabled');
        $('#decline').addClass('disabled');
    });

    $('#inviteCount').click(function () {
        if ($(this).text() != 0) {
            $('#viewInviteModal').modal('show');
            $.ajax({
                url: '/Team/GetInvites',
                success: function (data) {
                    for (var i = 0, len = data.length; i < len; ++i) {
                        var li = document.createElement('li');
                        li.className = 'list-group-item';
                        li.id = data[i].InviteID;
                        li.innerHTML = '<b>' + data[i].Sender + '</b>' + ' has sent you an invitation to join team ' + '<b>' + data[i].TeamName + '</b>';
                        
                        $('.list-group').append(li);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(errorThrown + textStatus);
                }
            });
        }
    });

    $('.list-group').on('click', 'li', function () {
        $('li').each(function () {
            $(this).css('background-color', 'white');
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            }
        });

        $(this).css('background-color', 'lightblue');
        $(this).addClass('selected');

        if ($('#accept').hasClass('disabled')) {
            $('#accept').removeClass('disabled');
            $('#decline').removeClass('disabled');
        }
    });
});

var inviteCount = function () {
    $.ajax({
        url: '/Team/GetInviteCount',
        success: function (count) {

            $('#inviteCount').html(count);
            if (count == 1) {
                $('#wording').html('invitation');
            }
            else {
                $('#wording').html('invitations');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown + textStatus);
        }
    });
}

var populateTeamList = function () {
    $.ajax({
        url: '/Team/GetTeamList',
        success: function (data) {
            var table = document.getElementById("myTeams").getElementsByTagName('tbody')[0];

            if (data.length == 0) {
                $('#noteams').show();
            }
            else {
                for (var i = 0, len = data.length; i < len; ++i) {
                    var team = data[i];
                    var newRow = table.insertRow(table.rows.length);

                    var select = newRow.insertCell(0);
                    var name = newRow.insertCell(1);
                    var coursecode = newRow.insertCell(2);
                    var coursename = newRow.insertCell(3);
                    var members = newRow.insertCell(4);

                    var selectbox = document.createElement('input');
                    //selectbox.className = 'test';
                    selectbox.type = 'checkbox';
                    selectbox.value = team.TeamID;

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
                    var coursecodetext = document.createTextNode(team.CourseCode);
                    var coursenametext = document.createTextNode(team.CourseName);
                    var memberstext = document.createTextNode(memberconcat);

                    select.appendChild(selectbox);
                    name.appendChild(nametext);
                    coursecode.appendChild(coursecodetext);
                    coursename.appendChild(coursenametext);
                    members.appendChild(memberstext);
                }
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
        url: '/Team/CreateTeam',
        data: { teamname: name, coursetoken: token },
        success: function (msg) {
            if (msg)
                alert(msg);
            else
                $('#createTeamModal').modal('hide');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown + textStatus);
        }
    });
}

var sendInvite = function () {
    var username = document.forms['sendinvite-modal-form'].userName.value;

    var selectedTeamsArray = new Array();
    var x = 0;
    $('#myTeams > tbody  > tr').each(function() {
        if ($(this).find('input').attr('checked')) {
            selectedTeamsArray[x] = $(this).find('input').attr('value');
            x++;
        }
    });

    var selectedTeams = selectedTeamsArray.join(',')

    $.ajax({
        url: '/Team/SendInvite',
        data: { sendto: username, teams: selectedTeams },
        success: function (msg) {
            if (msg) {
                alert(msg);
            }
            else {
                $('#sendInviteModal').modal('hide');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown + textStatus);
        }
    });
}

var InviteResponse = function (resp) {
    var id;
    $('li').each(function () {
        if ($(this).hasClass('selected'))
            id = $(this).attr('id');
    });

    $.ajax({
        url: '/Team/InviteResponse',
        data: { inviteid: id, response: resp },
        success: function () {
            inviteCount();
            $('#viewInviteModal').modal('hide');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown + textStatus);
        }
    });
}
