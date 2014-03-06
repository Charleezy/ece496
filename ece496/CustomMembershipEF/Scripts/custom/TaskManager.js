$(document).ready(function () {
    
    //Gets list of teams for the team selection box to let you pick a project
    $.ajax({
        url: '/Task/GetTeamList',
        success: function (data) {
            for (var i = 0, len = data.length; i < len; ++i) {
                var option = document.createElement("option");
                option.text = data[i].TeamName;
                option.value = data[i].TeamID;
                var select = document.getElementById("Select1");
                select.appendChild(option);
                //After getting the first option, set it as the default project
                if(i==0){
                    select.firstChild.selected = "selected";
                    $("#Select1").change();
                }
            }
        }
    });

    //Gets a list of teams for when you are creating a task
    $.ajax({
        url: '/Task/GetTeamList',
        success: function (data) {
            for (var i = 0, len = data.length; i < len; ++i) {
                var option = document.createElement("option");
                option.text = data[i].TeamName;
                option.value = data[i].TeamID;
                var select = document.getElementById("team");
                //console.log(select);
                select.appendChild(option);
                //After getting the first option, set it as the default project
                //TODO, in the future have the currently selected team (team dropdown outside modal) be the default team.
                if (i == 0) {
                    select.firstChild.selected = "selected";
                    //console.log(select.options[select.selectedIndex].text);
                    $("#team").change();
                }
            }
        }
    });

    //Get a list of potential assignees for your new task
    //To be run whenever the team is changed
    $("#team").change(function () {
        var selectedTeam = document.forms['createtask-modal-form'].team.value;
        console.log("selectedTeam" + selectedTeam);
        $.ajax({
            url: '/Task/GetTeamMembers',
            data: { TeamID: selectedTeam },
            success: function (data) {
                for (var i = 0, len = data.length; i < len; ++i) {
                    var option = document.createElement("option");
                    option.text = data[i].TeamMember;
                    option.value = data[i].TeamMemberID;
                    var select = document.getElementById("assignee");
                    select.appendChild(option);
                    //After getting the first option, set it as the default assignee
                    //TODO, in the future have the assignee be you, or the pm. Configurable
                    if (i == 0) {
                        select.firstChild.selected = "selected";
                    }
                }
            }
        });
    })

    //NAH add which team a task is for
    //NAH task list should be initialized on page load, not just select
    //DONE delete all rows and rebuild the table on change
    //TODO spinner
    //TODO remove console logs here and in teamManager.js
    /**
    * Populates task table on changing a team
    **/
    $("#Select1").change(function () {
        var id = $(this).val();
        //alert("Handler for .change() called.");
        $.ajax({
            url: '/Task/GetTaskList',
            data: { TeamID: id },
            success: function (data) {

                //debugging
                //console.log(data);
                var table = document.getElementById("myTasks").getElementsByTagName('tbody')[0];

                //Clear the old table
                for (var i = table.rows.length - 1; i >= 0; i--) {
                    table.deleteRow(i);
                }

                //TODO, shouldn't this be handled?
                if (data.length == 0) {
                    
                }

                for (var i = 0, len = data.length; i < len; ++i) {
                    //console.log(data[i].TaskStartTime);
                    var task = data[i];
                var newRow = table.insertRow(table.rows.length);//inserts row at last position

                var taskName = newRow.insertCell(0);
                var projectedStartDate = newRow.insertCell(1);
                var deadline = newRow.insertCell(2);
                var status = newRow.insertCell(3);

                /*var selectbox = document.createElement('input');
                selectbox.className = 'test';
                selectbox.type = 'checkbox';
                selectbox.id = team.TeamID;*/

                /*var memberconcat = "";
                for (var j = 0; j < team.TeamMembers.length; j++) {
                    if (team.TeamMembers.length - j > 1) {
                        memberconcat = memberconcat.concat(team.TeamMembers[j]);
                        memberconcat = memberconcat.concat(", ");
                    }
                    else {
                        memberconcat = memberconcat.concat(team.TeamMembers[j]);
                    }
                    
                }*/

                var nametext = document.createTextNode(task.TaskName);
                var projectedStartDateText = document.createTextNode(task.TaskStartTime);
                var deadlineText = document.createTextNode(task.TaskDeadline);

                var statusText;
                if (task.Status == 0)
                    statusText = "Not Started";
                else if (task.Status == 1)
                    statusText = "InProgress";
                else if (task.Status == 2)
                    statusText = "Finished";
                else {
                    statusText = "Undefined"
                }
                statusText = document.createTextNode(statusText);

                taskName.appendChild(nametext);
                projectedStartDate.appendChild(projectedStartDateText);
                deadline.appendChild(deadlineText );
                status.appendChild(statusText);
                }
                //spinner.stop();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("error");
                }
        });
    });

});

var populateTaskList = function () {
    $.ajax({
        url: '/Task/GetTaskList',
        success: function (data) {
            var table = document.getElementById("myTasks").getElementsByTagName('tbody')[0];

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

                var nametext = document.createTextNode(team.TaskName);
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

var createTask = function () {
    var name = document.forms['createtask-modal-form'].taskName.value;
    var token = document.forms['createtask-modal-form'].taskDetails.value;

    if (name == "" || name == null || token == "" || token == null) {
        if (name == "" || name == null) {
            $('#teamName').parent('div').addClass('has-error');
            $('#teamname_err').show();
        }
        if (token == "" || token == null) {
            $('#courseToken').parent('div').addClass('has-error');
            $('#coursetoken_err').show();
        }
    }
    else {
        $.ajax({
            url: '/Team/CreateTeam',
            data: { teamname: name, coursetoken: token },
            success: function (msg) {
                if (msg) {
                    $('#createteam_alert').html(msg);
                    $('#createteam_alert').show();
                }
                else {
                    $('#createTeamModal').modal('hide');
                    $('#myTeams > tbody > tr').each(function () {
                        $(this).remove();
                    });
                    var targ1 = document.getElementById('myTeams').getElementsByTagName('tbody')[0];
                    spinner = new Spinner(spinoptions).spin(targ1);
                    populateTeamList();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown + textStatus);
            }
        });
    }
}

$(function () {
    $('#taskStartTime').datetimepicker();
    $('#taskDeadline').datetimepicker();
});

/**populates assignees dropdown when creating tasks
* JUNK
**/
/*var populateAssignee = function () {

    var assignee = document.getElementById("assignee");//.getElementsByTagName('assignee');
    //console.log(assignee);

    $.ajax({
    url: '/Task/GetTeamMembers',
        success: function (data) {
            var option = document.createElement("option");
            option.text = "Charlie";
            option.value = "7";
            assignee.appendChild(option);
        }
    });
}*/