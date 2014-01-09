$(document).ready(function () {

    //Validation of inputs on focusout
    $('#signinEmail').focusout(function () {
        if (this.value != "") {
            $(this).parent('div').removeClass('has-error');
            $('#signinEmail_err').hide();
        }
    });

    $('#signinPassword').focusout(function () {
        if (this.value != "") {
            $(this).parent('div').removeClass('has-error');
            $('#signinPassword_err').hide();
        }
    });

    //When closing signin modal, clear fields
    $('#signinModal').on('hidden.bs.modal', function () {
        $('#signinEmail').parent('div').removeClass('has-error');
        $('#signinEmail_err').hide();
        $('#signinEmail').val("");

        $('#signinPassword').parent('div').removeClass('has-error');
        $('#signinPassword_err').hide();
        $('#signinPassword').val("");
    })

    //Detailed hover boxes
    $('#teamDetails').popover({
        html: true,
        trigger: "hover",
        content: function () {
            return $('#teamDetails_msg').html();
        }
    });
    $('#taskDetails').popover({
        html: true,
        trigger: "hover",
        content: function () {
            return $('#taskDetails_msg').html();
        }
    });
    $('#calendarDetails').popover({
        html: true,
        trigger: "hover",
        content: function () {
            return $('#calendarDetails_msg').html();
        }
    });
});

//Triggered when login button clicked inside signin modal
var signin = function () {
    //If email or password is empty, highlight box and display error message
    if (document.forms['signin-modal-form'].signinEmail.value == "") {
        $('#signinEmail').parent('div').addClass('has-error');
        $('#signinEmail_err').show();
    }
    if (document.forms['signin-modal-form'].signinPassword.value == "") {
        $('#signinPassword').parent('div').addClass('has-error');
        $('#signinPassword_err').show();
    }
    return true;
}