function Submit() {
    var c = $("#CustomerCode").val();
    var d = $("#drpModeLCodeList").val();
    
    // var a = JSON.stringify({ customerId: $(this).val() };
    $.ajax({
        url: "/ManageMasterData/ManageMasterData/UpdateTransmit",
        type: 'POST',
        datatype: 'json',
        data: { CustomerID: c, ModeID: d },

        cache: false,
        success: function (data) {
            $('#tblDisplayData').show();
            $('#tblModeSelection').hide();

            //var items = '<option>Select a User</option>';
            //$.each(data, function (i, ModeCodeList) {
            //    items += "<option value='" + ModeCodeList.Value + "'>" + ModeCodeList.Text + "</option>";
            //});

            $("#drpRRISList").val(data.RRISList);
            $("#AccountCode").val(data.AccountCode);
            if ((data.ChangeUserTrans) != null) {
                var CU = ((data.ChangeUserTrans).split("|"));
                $("#txtChangeUserName").val(CU[0]);
                $("#txtChangeUserName1").val(CU[1]);
            }
            else {
                $("#txtChangeUserName").val("");
                $("#txtChangeUserName1").val("");
            }
            var d = new Date();
            d.setTime(parseInt(data.ChangeTimeTrans.substring(6)));
            $("#ChangeTimeTransmit").val(FormatDate(d));


        },
        error: function (data) {

        }
    });
}

function ClientValidationMode() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    $("#ErrorMsgContainer1").html("");
    HighlightInputsForError($("#drpCustomerCodeList"), false)
    if (document.getElementById('drpCustomerCodeList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Customer";
        HighlightInputsForError($("#drpCustomerCodeList"), isError)
    }
    if (isError) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}
function ClientValidationTransmit() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    $("#ErrorMsgContainer1").html("");
    HighlightInputsForError($("#drpModeLCodeList"), false)
    if (document.getElementById('drpModeLCodeList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Mode";
        HighlightInputsForError($("#drpModeLCodeList"), isError)
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
    $("#ErrorMsgContainer1").html("");
    HighlightInputsForError($("#drpCustomerCodeList"), false)
    if (document.getElementById('drpCustomerCodeList').selectedIndex == 0) {
        isError = true;
        errMsg = "Please Select a Customer";
        HighlightInputsForError($("#drpCustomerCodeList"), isError)
    }
    HighlightInputsForError($("#drpModeLCodeList"), false)
    if (document.getElementById('drpModeLCodeList').selectedIndex == 0) {
        isError = true;
        if (errMsg.trim() == "")
            errMsg = "Please Select a Mode";
        else
            errMsg += ", a Mode";
        HighlightInputsForError($("#drpModeLCodeList"), isError)
    }
    HighlightInputsForError($("#drpRRISList"), false)
    if (document.getElementById('drpRRISList').selectedIndex == 0) {
        isError = true;
        if (errMsg.trim() == "")
            errMsg = "Please Set your RRIS Transmit Switch";
        else
            errMsg += ", RRIS Transmit Switch.";
        HighlightInputsForError($("#drpRRISList"), isError)
    }
    HighlightInputsForError($("#txtAccountCode"), false)
    var val7 = document.getElementById("txtAccountCode").value;
    if (val7.length < 1) {
        isError = true;
        errMsg += "Please enter an RRIS Expense Account Code ";
        HighlightInputsForError($("#txtAccountCode"), isError)
    }
    var AccCode = ($("#txtAccountCode").val()).substring(0, 4);
    if ((document.getElementById("txtAccountCode").value).length == 7) {
        if (AccCode != "2603" && AccCode != null) {
            isError = true;
            errMsg += "Seven Digit Expense Codes must start with 2603 ";
            HighlightInputsForError($("#txtAccountCode"), isError)
        }
    }
    if (isError) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}
function ClientValidationUpdate() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    $("#ErrorMsgContainer1").html("");
    var AccCode = ($("#txtAccountCode").val()).substring(0, 4);
    if ((document.getElementById("txtAccountCode").value).length == 7) {
        if (AccCode != "2603" && AccCode != null) {
            isError = true;
            errMsg += "Seven Digit Expense Codes must start with 2603 ";
            HighlightInputsForError($("#txtAccountCode"), isError)
        }
    }
    if (isError) {
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

function ValidateNumber(e) {
    var evt = (e) ? e : window.event;
    var charCode = (evt.keyCode) ? evt.keyCode : evt.which;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}