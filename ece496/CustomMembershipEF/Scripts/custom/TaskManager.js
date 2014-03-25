// Initialize spinner and spinner visuals
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
    // Initialize spinner under table header
    var target = document.getElementById('myTasks').getElementsByTagName('tbody')[0];
    spinner = new Spinner(spinoptions).spin(target);

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
                if (i == 0) {
                    select.firstChild.selected = "selected";
                    var id = data[i].TeamID;
                    populateTaskList(id);
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
                select.appendChild(option);
                //After getting the first option, set it as the default project
                //TODO, in the future have the currently selected team (team dropdown outside modal) be the default team.
                if (i == 0) {
                    select.firstChild.selected = "selected";
                    $("#team").change();
                }
            }
        }
    });

    //Get a list of potential assignees for your new task
    //To be run whenever the team is changed
    $("#team").change(function () {
        var selectedTeam = document.forms['createtask-modal-form'].team.value;
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

    // When a user selects a new team in dropdown menu
    $("#Select1").change(function () {
        $('#myTasks > tbody > tr').each(function() {
            $(this).remove();
        });

        // Hide "No tasks" error message if its still displayed
        $('#notasks').hide();

        // Re-initialize the spinner and populate table for selected team
        var targ = document.getElementById('myTasks').getElementsByTagName('tbody')[0];
        spinner = new Spinner(spinoptions).spin(targ);
        var id = $(this).val();
        populateTaskList(id);
    });
});

//NAH add which team a task is for
//NAH task list should be initialized on page load, not just select
//DONE delete all rows and rebuild the table on change
//TODO spinner
//TODO remove console logs here and in teamManager.js
/**
* Populates task table on changing a team
**/
var populateTaskList = function (id) {
    $.ajax({
        url: '/Task/GetTaskList',
        data: { TeamID: id },
        success: function (data) {
            var table = document.getElementById("myTasks").getElementsByTagName('tbody')[0];

            // Clear the old table
            for (var i = table.rows.length - 1; i >= 0; i--) {
                table.deleteRow(i);
            }

            // If no tasks belong to this team, display message
            if (data.length == 0) {
                $('#notasks').show();
            }

            for (var i = 0, len = data.length; i < len; ++i) {
                var task = data[i];
                var newRow = table.insertRow(table.rows.length);//inserts row at last position

                var select = newRow.insertCell(0);
                var taskName = newRow.insertCell(1);
                var taskDescription = newRow.insertCell(2);
                var projectedStartDate = newRow.insertCell(3);
                var deadline = newRow.insertCell(4);
                var status = newRow.insertCell(5);
                var assignee = newRow.insertCell(6);

                var selectbox = document.createElement('input');
                selectbox.type = 'checkbox';
                selectbox.value = task.TaskID;

                var nametext = document.createTextNode(task.TaskName);
                var taskDescriptionText = document.createTextNode(task.TaskDescription);
                var projectedStartDateText = document.createTextNode(task.TaskStartTime);
                var deadlineText = document.createTextNode(task.TaskDeadline);
                var assigneeText = document.createTextNode(task.Assignee);

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

                var assigneeText = document.createTextNode(task.Assignee);

                select.appendChild(selectbox);
                taskName.appendChild(nametext);
                taskDescription.appendChild(taskDescriptionText);
                projectedStartDate.appendChild(projectedStartDateText);
                deadline.appendChild(deadlineText);
                status.appendChild(statusText);
                assignee.appendChild(assigneeText);
            }
            // Stop spinner after table is populated
            spinner.stop();
        }
    });
}

var createTask = function () {
    var taskName = document.forms['createtask-modal-form'].taskName.value;
    var taskDescription = document.forms['createtask-modal-form'].taskDescription.value;
    var taskStartTime = document.forms['createtask-modal-form'].taskStartTime.value;
    var taskDeadline = document.forms['createtask-modal-form'].taskDeadline.value;
    var team = document.forms['createtask-modal-form'].team.value;
    var assignee = document.forms['createtask-modal-form'].assignee.value;

    if (taskName == "" || taskName == null || taskStartTime == "" || taskStartTime == null || taskDeadline == "" || taskDeadline == null || team == "" || team == null || assignee == "" || assignee == null) {
        if (name == "" || name == null) {
            $('#taskName').parent('div').addClass('has-error');
            $('#taskName_err').show();
        }
        if (taskStartTime == "" || taskStartTime == null) {
            $('#taskStartTime ').parent('div').addClass('has-error');
            $('#taskStartTime_err ').show();
        }
        if (taskDeadline == "" || taskDeadline == null) {
            $('#taskDeadline ').parent('div').addClass('has-error');
            $('#taskDeadline_err ').show();
        }
        if (team == "" || team == null) {
            $('#team ').parent('div').addClass('has-error');
            $('#team_err ').show();
        }
        if (assignee == "" || assignee == null) {
            $('#assignee ').parent('div').addClass('has-error');
            $('#assignee_err ').show();
        }
    }
    else {
        $.ajax({
            url: '/Task/CreateTask',
            data: { taskName: taskName, taskDescription: taskDescription, taskStartTime: taskStartTime, taskDeadline: taskDeadline, assigneeID: assignee, teamID: team },
            success: function (msg) {
                if (msg) {
                    $('#createtask_alert').html(msg);
                    $('#createteam_alert').show();
                }
                else {
                    $('#createTaskModal').modal('hide');
                    $('#myTasks > tbody > tr').each(function () {
                        $(this).remove();
                    });
                    var targ1 = document.getElementById('myTasks').getElementsByTagName('tbody')[0];
                    spinner = new Spinner(spinoptions).spin(targ1);
                    populateTaskList();
                }
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