$(document).ready(function () {
    //When closing create team modal, clear fields
    $('#createTeamModal').on('hidden.bs.modal', function () {
        $('#teamName').parent('div').removeClass('has-error');
        $('#teamName_err').hide();
        $('#teamName').val("");

        $('#courseToken').parent('div').removeClass('has-error');
        $('#courseToken').val("");
    })
});

var createTeam = function () {
    var name = document.forms['createteam-modal-form'].teamName.value;
    var token = document.forms['createteam-modal-form'].courseToken.value;

    $.ajax({
        url: '/User/CreateTeam',
        data: { teamname: name, coursetoken: token},
        success: function () {
            $('#createTeamModal').modal('hide');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus);
        }
    });
}
