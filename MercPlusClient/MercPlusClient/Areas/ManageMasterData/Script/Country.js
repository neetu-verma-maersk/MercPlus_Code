
function ClientValidation() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#drpCountry"), false)
    if (document.getElementById('drpCountry').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Country Code to Query";
        HighlightInputsForError($("#drpCountry"), isError)
    }
    if (isError) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}

function ValidateNumber(e) {
    var evt = (e) ? e : window.event;
    var charCode = (evt.keyCode) ? evt.keyCode : evt.which;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}


