
function ClientValidationManual() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#drpManCodeList"), false)
    if (document.getElementById('drpManCodeList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Manual Code ";
        HighlightInputsForError($("#drpManCodeList"), isError)
    }
    if (isError) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}
function ClientValidationMode() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#drpManCodeList"), false)
    if (document.getElementById('drpManCodeList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Mode";
        HighlightInputsForError($("#drpManCodeList"), isError)
    }
    if (isError) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}
function ClientValidationRepair() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#drpRepCodeList"), false)
    if (document.getElementById('drpRepCodeList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Repair Code";
        HighlightInputsForError($("#drpRepCodeList"), isError)
    }
    if (isError) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}
function ClientValidationAdd() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#txtExcluRepairCode"), false)
    var val7 = document.getElementById("txtExcluRepairCode").value;
    if (val7.length < 1) {
        isError = true;
        errMsg += "Please Enter an Exclusive Repair Code";
        HighlightInputsForError($("#txtExcluRepairCode"), isError)
    }

    if (isError) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}
function ClientValidationDelete() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#txtExcluRepairCode"), false)
    var val7 = document.getElementById("txtExcluRepairCode").value;
    if (val7.length < 1) {
        isError = true;
        errMsg += "Please Enter an Exclusive Repair Code";
        HighlightInputsForError($("#txtExcluRepairCode"), isError)
    }
    HighlightInputsForError($("#txtExcluRepairCode"), false)
    var val7 = document.getElementById("txtExcluRepairCode").value;
    var val8 = document.getElementById("txtRepairCode").value;
    if (val7.trim()==val8.trim()) {
        isError = true;
        errMsg += "Please Create Exclusionary Codes with Different Repair Codes";
        HighlightInputsForError($("#txtExcluRepairCode"), isError)
    }


    if (isError) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}

function DeleteConfirmMsg() {
    if (ClientValidationDelete()) {
        return confirm("Are you sure you want to delete this record?")

    }
    else {
        return false;
    }
}