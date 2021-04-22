$(function () {
    if (ClientValidation) {
        $("#drpRRISCodesList").change(function () {
            $("#ErrorMsgContainer").html("");
            var c = $("#drpRRISCodesList").val();
            // var a = JSON.stringify({ customerId: $(this).val() };
            $.ajax({
                url: "/ManageMasterData/ManageMasterData/GetAllDetails",
                type: 'POST',
                data: { id: c },
                cache: false,
                success: function (data) {
                    $('#drpRRISFormatList').val(data.RRISFormat);
                    $("#txtPayAgentProfitCenter").val(data.SubProfitCenter);
                    $("#txtCorporateProfitCenter").val(data.ProfitCenter);
                    $("#txtCorporatePayAgentCode").val(data.CorpPayAgentCode);
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
                    $("#txtChangeTime").val(FormatDate(d));


                },
                error: function (data) {

                }
            });
        });
   }
});


// function DeleteFunction(){
//     if (DeleteConfirmMsg()) {
//        $("#drpRRISCodesList").change(function () {
//            var c = $(this).val();
//            // var a = JSON.stringify({ customerId: $(this).val() };
//            $.ajax({
//                url: "/ManageMasterData/ManageMasterData/DeletePayAgent",
//                type: 'POST',
//                data: { id: c },
//                cache: false
//            });
//        });
//    }
//};
function ClientValidation() {

    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#drpRRISCodesList"), false)
    if (document.getElementById('drpRRISCodesList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Pay Agent Code  </br>";
        HighlightInputsForError($("#drpRRISCodesList"), isError)
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
    var val1 = "";
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#txtRRISCode"), false)
    val1 = $("#txtRRISCode").val();
    if (val1.trim() == "") {
        isError = true;
        if (errMsg.trim() == "")
            errMsg = "Please Enter a RRIS Pay Agent Code ";
        else
            errMsg += ", RRIS Pay Agent Code";
        HighlightInputsForError($("#txtRRISCode"), isError)
    }
   
    HighlightInputsForError($("#txtCorporateProfitCenter"), false)
    val1 = $("#txtCorporateProfitCenter").val();
    if (val1.trim() == "") {
        isError = true;
        if (errMsg.trim() == "")
            errMsg = "Please Enter a Corporate Profit Center Code";
        else
            errMsg += ", Corporate Profit Center Code";
        HighlightInputsForError($("#txtCorporateProfitCenter"), isError)
    }
    HighlightInputsForError($("#txtCorporatePayAgentCode"), false)
    val1 = $('#txtCorporatePayAgentCode').val();
    if (val1.trim() == "") {
        isError = true;
        if (errMsg.trim() == "")
            errMsg = "Please Enter a Corporate Pay Agent Code  ";
        else
            errMsg += ", Corporate Pay Agent Code.   ";
        HighlightInputsForError($("#txtCorporatePayAgentCode"), isError)
    }
    HighlightInputsForError($("#drpRRISFormatList"), false)
    var val7 = document.getElementById("drpRRISFormatList").value;
    if (val7.length < 1) {
        isError = true;
        errMsg += "Please Select a RRIS Format  ";
        HighlightInputsForError($("#drpRRISFormatList"), isError)
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
    var strTime = hours + ':' + minutes +':'+seconds+ ' ' + ampm;
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

function ClientValidationDel() {

    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#drpRRISCodesList"), false)
    if (document.getElementById('drpRRISCodesList').selectedIndex == 0) {
        isError = true;
        errMsg = "Please Select a Pay Agent Code  </br>";
        HighlightInputsForError($("#drpRRISCodesList"), isError)
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
    if (ClientValidationDel()) {
        return confirm("Are you sure you want to delete this record?")

    }
    else {
        return false;
    }
}


