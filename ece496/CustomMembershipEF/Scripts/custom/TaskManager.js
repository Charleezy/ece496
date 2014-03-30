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

    $('#myTasks').on('click', 'input[type=checkbox]', function () {
        if ($('#myTasks input:checkbox:checked').length > 0) {
            $('#deleteTask_button').removeClass('disabled');
        }
        else {
            $('#deleteTask_button').addClass('disabled');
        }
    });

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

    $('#createTask_button').click(function () {
        // Populate team name for team that is currently selected
        var teamName = $('#Select1').find(":selected").text();
        var teamID = $('#Select1').find(":selected").val();
        $('#teamName').val(teamName);
        $('#teamID').html(teamID);

        // Remove previous user list
        $('#assignee').find('option').remove().end();

        // Update list of team members belonging to the selected team
        var selectedTeam = $('#Select1').val();
        getTeamMembers_create(selectedTeam);
    });

    $('#createTaskModal').on('hidden.bs.modal', function () {
        // Hide validation error messages and clear form fields
        $('#taskname_err').hide();
        $('#taskName').val("");

        $('#taskDescription').val("");

        $('#taskStartTime_err').hide();
        $('#taskStartTime').val("");

        $('#taskDeadline_err').hide();
        $('#taskDeadline').val("");
    });

    $('#editTaskModal').on('hidden.bs.modal', function () {
        // Hide validation error messages
        $('#edit_taskname_err').hide();
        $('#edit_taskStartTime_err').hide();
        $('#edit_taskDeadline_err').hide();
        $('#edittask_alert').hide();
    });

    $('#taskName').focus(function () {
        $('#taskname_err').hide();
    });

    $('#taskStartTime').focus(function () {
        $('#taskStartTime_err').hide();
        $('#createtask_alert').hide();
    });

    $('#taskDeadline').focus(function () {
        $('#taskDeadline_err').hide();
        $('#createtask_alert').hide();
    });

    $('#edit_taskName').focus(function () {
        $('#edit_taskname_err').hide();
    });

    $('#edit_taskStartTime').focus(function () {
        $('#edit_taskStartTime_err').hide();
        $('#edittask_alert').hide();
    });

    $('#edit_taskDeadline').focus(function () {
        $('#edit_taskDeadline_err').hide();
        $('#edittask_alert').hide();
    });

    $('#myTasks').on('mouseover', 'tr', function () {
        $(this).find('.edit_button').show();
    });

    $('#myTasks').on('mouseleave', 'tr', function () {
        $(this).find('.edit_button').hide();
    });

    $('#myTasks').on('click', '.edit_button', function () {
        // Get data for current row
        var teamName = $('#Select1').find(":selected").text();
        var teamID = $('#Select1').find(":selected").val();
        var taskID = $(this).parent().parent().find('td:eq(0) > input').val();
        var taskname = $(this).parent().parent().find('td:eq(1)').text();
        var taskDetails = $(this).parent().parent().find('td:eq(2)').text();
        var startDate = $(this).parent().parent().find('td:eq(3)').text();
        var deadline = $(this).parent().parent().find('td:eq(4)').text();
        var status = $(this).parent().parent().find('td:eq(5)').text();
        
        // Populate modal with current data
        $('#edit_teamName').val(teamName);
        $('#edit_teamID').html(teamID);
        $('#edit_taskID').html(taskID);
        $('#edit_taskName').val(taskname);
        $('#edit_taskDetails').val(taskDetails);
        $('#edit_taskStartTime').val(startDate);
        $('#edit_taskDeadline').val(deadline);

        // Remove previous user list
        $('#edit_assignee').find('option').remove().end();

        // Populate assignee list
        var selectedTeam = $('#Select1').val();
        getTeamMembers_edit(selectedTeam);
       
        $('#editTaskModal').modal('show');
    });
});

// Populates the assignee dropdown box for edit task modal
var getTeamMembers_edit = function (teamID) {
    $.ajax({
        url: '/Task/GetTeamMembers',
        data: { TeamID: teamID },
        success: function (data) {
            var select = document.getElementById("edit_assignee");

            for (var i = 0, len = data.length; i < len; ++i) {
                var option = document.createElement("option");
                option.text = data[i].TeamMember;
                option.value = data[i].TeamMemberID;
                select.appendChild(option);
                //After getting the first option, set it as the default assignee
                //TODO, in the future have the assignee be you, or the pm. Configurable
                if (i == 0) {
                    select.firstChild.selected = "selected";
                }
            }
        }
    });
}

// Populates the assignee dropdown box for create task modal
var getTeamMembers_create = function (teamID) {
    $.ajax({
        url: '/Task/GetTeamMembers',
        data: { TeamID: teamID },
        success: function (data) {
            var select = document.getElementById("assignee");

            for (var i = 0, len = data.length; i < len; ++i) {
                var option = document.createElement("option");
                option.text = data[i].TeamMember;
                option.value = data[i].TeamMemberID;
                select.appendChild(option);
                //After getting the first option, set it as the default assignee
                //TODO, in the future have the assignee be you, or the pm. Configurable
                if (i == 0) {
                    select.firstChild.selected = "selected";
                }
            }
        }
    });
}

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
                var edit = newRow.insertCell(7);

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
                    statusText = "To do";
                else if (task.Status == 1)
                    statusText = "In Progress";
                else if (task.Status == 2)
                    statusText = "Complete";
                else {
                    statusText = "Undefined"
                }
                statusText = document.createTextNode(statusText);

                var assigneeText = document.createTextNode(task.Assignee);

                var editButton = document.createElement("SPAN");
                editButton.className = "glyphicon glyphicon-edit edit_button";
                editButton.style.cursor = "pointer";
                editButton.style.display = "none";

                select.appendChild(selectbox);
                taskName.appendChild(nametext);
                taskDescription.appendChild(taskDescriptionText);
                projectedStartDate.appendChild(projectedStartDateText);
                deadline.appendChild(deadlineText);
                status.appendChild(statusText);
                assignee.appendChild(assigneeText);
                edit.appendChild(editButton);
            }
            // Stop spinner after table is populated
            spinner.stop();
        }
    });
}

var createTask = function () {
    var teamID = $('#teamID').text();
    var taskName = $('#taskName').val();
    var taskDescription = $('#taskDescription').val();
    var taskStartTime = $('#taskStartTime').val();
    var taskDeadline = $('#taskDeadline').val();
    var assignee = $('#assignee').val();

    if (taskName == "" || taskName == null || taskStartTime == "" || taskStartTime == null || taskDeadline == "" || taskDeadline == null) {
        if (taskName == "" || taskName == null) {
            $('#taskname_err').show();
        }
        if (taskStartTime == "" || taskStartTime == null) {
            $('#taskStartTime_err').show();
        }
        if (taskDeadline == "" || taskDeadline == null) {
            $('#taskDeadline_err').show();
        }
    }
    else {
        $('#createButton').addClass("disabled");

        $.ajax({
            url: '/Task/CreateTask',
            data: { taskName: taskName, taskDescription: taskDescription, taskStartTime: taskStartTime, taskDeadline: taskDeadline, assigneeID: assignee, teamID: teamID },
            success: function (msg) {
                if (msg) {
                    $('#createtask_alert').html(msg);
                    $('#createtask_alert').show();
                }
                else {
                    $('#createTaskModal').modal('hide');
                    $('#myTasks > tbody > tr').each(function () {
                        $(this).remove();
                    });
                    $('#notasks').hide();
                    var targ1 = document.getElementById('myTasks').getElementsByTagName('tbody')[0];
                    spinner = new Spinner(spinoptions).spin(targ1);
                    var selectedTeam = $('#Select1').val();
                    populateTaskList(selectedTeam);
                }
                $('#createButton').removeClass("disabled");
            }
        });
    }
}

var editTask = function () {
    var taskID = $('#edit_taskID').text();
    var taskName = $('#edit_taskName').val();
    var taskDescription = $('#edit_taskDetails').val();
    var taskStartTime = $('#edit_taskStartTime').val();
    var taskDeadline = $('#edit_taskDeadline').val();
    var status = $('#edit_status').val();
    var assignee = $('#edit_assignee').val();

    // Validate form
    if (taskName == "" || taskName == null || taskStartTime == "" || taskStartTime == null || taskDeadline == "" || taskDeadline == null) {
        if (taskName == "" || taskName == null) {
            $('#edit_taskname_err').show();
        }
        if (taskStartTime == "" || taskStartTime == null) {
            $('#edit_taskStartTime_err').show();
        }
        if (taskDeadline == "" || taskDeadline == null) {
            $('#edit_taskDeadline_err').show();
        }
    }
    else {
        $('#editButton').addClass("disabled");

        $.ajax({
            url: '/Task/UpdateTask',
            data: { taskID: taskID, taskName: taskName, taskDescription: taskDescription, taskStartTime: taskStartTime, taskDeadline: taskDeadline, status: status, assigneeID: assignee },
            success: function (msg) {
                if (msg) {
                    $('#edittask_alert').html(msg);
                    $('#edittask_alert').show();
                }
                else {
                    $('#editTaskModal').modal('hide');

                    $('#myTasks > tbody > tr').each(function () {
                        $(this).remove();
                    });

                    $('#deleteTask_button').addClass('disabled');
                    $('#editButton').removeClass("disabled");

                    var targ = document.getElementById('myTasks').getElementsByTagName('tbody')[0];
                    spinner = new Spinner(spinoptions).spin(targ);
                    var selectedTeam = $('#Select1').val();
                    populateTaskList(selectedTeam);
                }
                $('#editButton').removeClass("disabled");
            }
        });
    }
}

var deleteTasks = function () {
    var answer = confirm("Are you sure you want to permanently delete these tasks?");

    if (answer == true) {
        $('#deleteTask_button').addClass("disabled");
        $('#createTask_button').addClass("disabled");

        var selectedTasksArray = new Array();
        var x = 0;
        $('#myTasks > tbody  > tr').each(function () {
            if ($(this).find('input').is(':checked')) {
                selectedTasksArray[x] = $(this).find('input').attr('value');
                x++;
            }
        });

        var selectedTasks = selectedTasksArray.join(',');
        
        $.ajax({
            url: '/Task/DeleteTasks',
            data: { tasks: selectedTasks },
            success: function (msg) {
                $('#myTasks > tbody > tr').each(function () {
                    $(this).remove();
                });

                $('#createTask_button').removeClass("disabled");
                
                var targ = document.getElementById('myTasks').getElementsByTagName('tbody')[0];
                spinner = new Spinner(spinoptions).spin(targ);
                var selectedTeam = $('#Select1').val();
                populateTaskList(selectedTeam);
            }
        });
    }
    else {

    }
}

// Initialize date pickers for modal forms
$(function () {
    $('#taskStartTime').datetimepicker();
    $('#taskDeadline').datetimepicker();

    $('#edit_taskStartTime').datetimepicker();
    $('#edit_taskDeadline').datetimepicker();
});
