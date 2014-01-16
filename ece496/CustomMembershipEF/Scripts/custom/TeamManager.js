$(document).ready(function () {
    //When closing create team modal, clear fields
    $('#createTeamModal').on('hidden.bs.modal', function () {
        $('#teamName').parent('div').removeClass('has-error');
        $('#teamName_err').hide();
        $('#teamName').val("");
    })
});

var createTeam = function () {
    
}
