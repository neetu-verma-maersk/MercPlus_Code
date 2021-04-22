function GetValues() {
    $("#ErrorMsgContainer").html("");
    //$("#ErrorMsgContainer1").html("");
    if (ClientValidation) {
        var c = $("#drpSuspendCatList").val();
            $.ajax({
            url: "/ManageMasterData/ManageMasterData/GetAllDetailsForSuspendCategory",
            type: 'POST',
            datatype: 'json',
            data: { id: c },
            cache: false,
            success: function (data) {
                //alert(data.SuspcatDesc);
                $("#SuspcatDesc").val(data.SuspcatDesc);
                if ((data.ChangeUserSus) != null) {
                    var CU = ((data.ChangeUserSus).split("|"));
                    $("#ChangeUserSus").val(CU[0]);
                    $("#ChangeUserSus1").val(CU[1]);
                }
                else if (((data.ChangeUserSus) == 0)) {
                    $("#ChangeUserSus").val("");
                    $("#ChangeUserSus1").val("");
                }
                else {
                    $("#ChangeUserSus").val("");
                    $("#ChangeUserSus1").val("");
                }
               
                var d = new Date();
                d.setTime(parseInt(data.ChangeTimeSus.substring(6)));
                $("#ChangeTimeSus").val(FormatDate(d));


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
    //$("#ErrorMsgContainer1").html("");
    HighlightInputsForError($("#drpSuspendCatList"), false)
    if (document.getElementById('drpSuspendCatList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Suspend Category ID";
        HighlightInputsForError($("#drpSuspendCatList"), isError)
    }
    if (isError == true) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}


function DeleteConfirmMsg() {
    if (ClientValidation()) {
        return confirm("Are you sure you want to delete this record?")

    }
    else {
        return false;
    }
}

function ClientValidationAdd() {

    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    //$("#ErrorMsgContainer1").html("");
    HighlightInputsForError($("#SuspcatDesc"), false)
    var val7 = document.getElementById("SuspcatDesc").value;
    if (val7.length < 1) {
        isError = true;
        errMsg += "Please Enter a New Suspend Category Description ";
        HighlightInputsForError($("#SuspcatDesc"), isError)
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