$(function () {
    
    if (ClientValidation) {
        $("#drpCustomerList").bind("change", function () {

            var c = $(this).val();
            $("#ErrorMsgContainer").html("");
            // var a = JSON.stringify({ customerId: $(this).val() };
            $.ajax({
                url: "/ManageMasterData/ManageMasterData/GetAllDetailsForCustomer",
                type: 'POST',
                datatype: 'json',
                data: { id: c },
                cache: false,
                success: function (data) {
                    $("#CustomerName").val(data.CustomerDesc);
                    $("#drpManualList").val(data.ManualCode);
                    $("#drpCustActvSwtchList").val(data.CustomerActiveSw);
                    if ((data.ChangeUser) != null) {
                        var CU = ((data.ChangeUser).split("|"));
                        $("#txtChangeUserName").val(CU[0]);
                        $("#txtChangeUserName1").val(CU[1]);
                    }
                    else {
                        $("#txtChangeUserName").val("");
                        $("#txtChangeUserName1").val("");
                    }
                        var d = new Date();
                    d.setTime(parseInt(data.ChangeTime.substring(6)));
                    $("#changeTimeUser").val(FormatDate(d));



                },
                error: function (data) {

                }

            });

        });
    }
})
function ClientValidation() {

    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#drpCustomerList"), false)
    if (document.getElementById('drpCustomerList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Customer to Update";
        HighlightInputsForError($("#drpCustomerList"), isError)
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
    HighlightInputsForError($("#txtCustomer"), false)
    var val7 = document.getElementById("txtCustomer").value;
    if (val7.length < 1) {
        isError = true;
        errMsg = "Please Enter a Customer Code";
        HighlightInputsForError($("#txtCustomer"), isError)
    }
    
    HighlightInputsForError($("#CustomerName"), false)
    var val7 = document.getElementById("CustomerName").value;
    if (val7.trim()=="") {
        isError = true;
        if (errMsg.trim() == "")
            errMsg = "Please Enter a Customer Name ";
        else
            errMsg += ", Customer Name. ";
        HighlightInputsForError($("#CustomerName"), isError)
    }
    HighlightInputsForError($("#drpManualList"), false)
    if (document.getElementById('drpManualList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Manual Code ";
        HighlightInputsForError($("#drpManualList"), isError)
    }
    HighlightInputsForError($("#drpCustActvSwtchList"), false)
    if (document.getElementById('drpCustActvSwtchList').selectedIndex == 0) {
        isError = true;
        if (errMsg.trim() == "")
            errMsg = "Please Select a Customer Active Switch";
        else
            errMsg += ", Customer Active Switch.";
        HighlightInputsForError($("#drpCustActvSwtchList"), isError)
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



