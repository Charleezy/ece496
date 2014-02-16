$(document).ready(function () {
    
    //Gets list of teams for the selection box
    $.ajax({
        url: '/Task/GetTeamList',
        success: function (data) {
            for (var i = 0, len = data.length; i < len; ++i) {
                var option = document.createElement("option");
                option.text = data[i].TeamName;
                option.value = data[i].TeamID;
                var select = document.getElementById("Select1");
                select.appendChild(option);
            }
        }
    });

    //NAH add which team a task is for
    //NAH task list should be initialized on page load, not just select
    //DONE delete all rows and rebuild the table on change
    //TODO spinner
    //TODO remove console logs here and in teamManager.js
    $("#Select1").change(function () {
        var id = $(this).val();
        //alert("Handler for .change() called.");
        $.ajax({
            url: '/Task/GetTaskList',
            data: { teamid: id },
            success: function (data) {

                //debugging
                console.log(data);
                var table = document.getElementById("myTasks").getElementsByTagName('tbody')[0];

                //Clear the old table
                for (var i = table.rows.length - 1; i >= 0; i--) {
                    table.deleteRow(i);
                }

                if (data.length == 0) {
                
                }

                for (var i = 0, len = data.length; i < len; ++i) {
                    console.log(data[i].TaskStartTime);
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