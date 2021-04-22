
function ClientValidation() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#txtLocCode"), false)
    var val7 = document.getElementById("txtLocCode").value;
    if (val7.length < 1) {
        isError = true;
        errMsg += "Please Enter a Location Code to Query ";
        HighlightInputsForError($("#txtLocCode"), isError)
    }
    if (isError) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}