
function drpEquipmentTypeListChange(val) {
    if (ClientValidation) {
        $("#ErrorMsgContainer").html("");
        var c = val;
        $.ajax({
            url: "/ManageMasterData/ManageMasterData/GetAllDetailsForEquipmentType",
            type: 'POST',
            datatype: 'json',
            data: { id: c },
            cache: false,
            success: function (data) {
                $("#txtEqDesc").val(data.EqTypeDesc);
                if ((data.ChangeUser) != null) {
                    var CU = ((data.ChangeUser).split("|"));
                    $("#ChangeUser").val(CU[0]);
                    $("#ChangeUser1").val(CU[1]);
                }
                else {
                    $("#ChangeUser").val("");
                    $("#ChangeUser1").val("");
                }
                var d = new Date();
                d.setTime(parseInt(data.ChangeTime.substring(6)));
                $("#ChTime").val(FormatDate(d));

            },
            error: function (data) {

            }
        });
    }
}
function ClientValidation() {

    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#drpEquipmentTypeList"), false)
    if (document.getElementById("drpEquipmentTypeList").selectedIndex == 0) {
        isError = true;
        errMsg += "Please select existing Equipment Type ";
        HighlightInputsForError($("#drpEquipmentTypeList"), isError)
    }
    if (isError == true) {
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
    HighlightInputsForError($("#txtEqType"), false)
    var val7 = document.getElementById("txtEqType").value;
    if (val7.length < 1) {
        isError = true;
        errMsg += "Please Enter a New Equipment Type ";
        HighlightInputsForError($("#txtEqType"), isError)
    }
    HighlightInputsForError($("#txtEqDesc"), false)
    var val7 = document.getElementById("txtEqDesc").value;
    if (val7.length < 1) {
        isError = true;
        if (errMsg.trim() == "")
            errMsg = "Please Enter an Equipment Description ";
        else
            errMsg += ", an Equipment Description";
        HighlightInputsForError($("#txtEqDesc"), isError)
    }
    if (isError == true) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}

function FormatDate(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var seconds = date.getSeconds();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    var month = date.getMonth();
    month++;
    month = month < 10 ? '0' + month : month;
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;
    var strTime = hours + ':' + minutes + ':' + seconds + ' ' + ampm;
    //return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear() + "  " + strTime;
    return date.getFullYear() + "-" + month + "-" + +date.getDate() + " " + strTime;
}
