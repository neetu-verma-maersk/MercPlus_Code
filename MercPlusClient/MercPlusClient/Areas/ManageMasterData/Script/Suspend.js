function ClientValidationShop() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    //$("#ErrorMsgContainer1").html("");
    HighlightInputsForError($("#drpShopCodeList"), false)
    if (document.getElementById('drpShopCodeList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Shop Code";
        HighlightInputsForError($("#drpShopCodeList"), isError)
    }
    if (isError) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}
function ClientValidationManual() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    //$("#ErrorMsgContainer1").html("");
    HighlightInputsForError($("#drpManualCodeList"), false)
    if (document.getElementById('drpManualCodeList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Manual Code ";
        HighlightInputsForError($("#drpManualCodeList"), isError)
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
    //$("#ErrorMsgContainer1").html("");
    HighlightInputsForError($("#drpModeSusList"), false)
    if (document.getElementById('drpModeSusList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Mode";
        HighlightInputsForError($("#drpModeSusList"), isError)
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
    //$("#ErrorMsgContainer1").html("");
    HighlightInputsForError($("#drpRepairCodeList"), false)
    if (document.getElementById('drpRepairCodeList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Repair Code";
        HighlightInputsForError($("#drpRepairCodeList"), isError)
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
    //$("#ErrorMsgContainer1").html("");
    HighlightInputsForError($("#drpRepairCodeList"), false)
    if (document.getElementById('drpRepairCodeList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Repair Code";
        HighlightInputsForError($("#drpRepairCodeList"), isError)
    }
    HighlightInputsForError($("#drpSuspendList"), false)
    if (document.getElementById('drpSuspendList').selectedIndex == 0) {
        isError = true;
        if(errMsg.trim()=="")
            errMsg = "Please Select a Suspend Category ID ";
        else
            errMsg+=", Suspend Category ID."
        HighlightInputsForError($("#drpSuspendList"), isError)
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
        return confirm("Are you sure you want to delete this record?")

    }
  