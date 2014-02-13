

$(document).ready(function () {
    
    //Gets list of teams for the selection box
    $.ajax({
        url: '/User/GetTeamList',
        success: function (data) {
            console.log(data);
            for (var i = 0, len = data.length; i < len; ++i) {
                console.log(data[i].Course);

                var option = document.createElement("option");
                option.text = data[i].TeamName;
                option.value = data[i].TeamID;
                var select = document.getElementById("Select1");
                select.appendChild(option);
            }
        }
    });

    //TODO add which team a task is for
    //TODO task list should be initialized on page load, not just select
    //TODO delete all rows and rebuild the table on change
    $("#Select1").change(function () {
        var id = $(this).val();
        alert("Handler for .change() called.");
        $.ajax({
            url: '/User/GetTaskList',
            data: { teamid: id },
            success: function (data) {
                //debugging
                console.log(data);
                var table = document.getElementById("myTasks").getElementsByTagName('tbody')[0];

                if (data.length == 0) {
                
                }

                for (var i = 0, len = data.length; i < len; ++i) {
                    console.log(data[i].TaskName);
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
                var projectedStartDateText = document.createTextNode(task.ProjectedStartDate);
                var deadlineText = document.createTextNode(task.Deadline);
                var statusText = document.createTextNode(task.Status);

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
        url: '/User/GetTaskList',
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