$(function () {
   
    if (ClientValidation) {
        $("#drpVendorList").bind("change", function () {
            $("#ErrorMsgContainer").html("");
            var c = $(this).val();
            // var a = JSON.stringify({ customerId: $(this).val() };
            $.ajax({
                url: "/ManageMasterData/ManageMasterData/GetAllDetailsForVendor",
                type: 'POST',
                datatype: 'json',
                data: { id: c },
                cache: false,
                success: function (data) {
                    $("#VendorDesc").val(data.VendorDesc);
                    $("#drpVenCountryList").val(data.VenCountryCode);
                    $("#drpVendorSwitchList").val(data.VendorActiveSw);
                    if ((data.ChangeUserVendor) != null) {
                        var CU = ((data.ChangeUserVendor).split("|"));
                        $("#ChangeUserVendor").val(CU[0]);
                        $("#ChangeUserVendor1").val(CU[1]);
                    }
                    else {
                        $("#ChangeUserVendor").val("");
                        $("#ChangeUserVendor1").val("");
                    }
                    var d = new Date();
                    d.setTime(parseInt(data.ChangeTimeVendor.substring(6)));
                    $("#ChangeTimeVendor").val(FormatDate(d));


                },
                error: function (data) {

                }
            });
        });
    }
});
function ClientValidation() {

    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#drpVendorList"), false)
    if (document.getElementById('drpVendorList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a MERC Vendor Code";
        HighlightInputsForError($("#drpVendorList"), isError)
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
    //$("#ErrorMsgContainer1").html("");
    HighlightInputsForError($("#txtVendorCode"), false)
    var val7 = document.getElementById("txtVendorCode").value;
    if (val7.length < 1) {
        isError = true;
        errMsg += "Please Enter a New MERC Vendor Code ";
        HighlightInputsForError($("#txtVendorCode"), isError)
    }
    HighlightInputsForError($("#VendorDesc"), false)
    var val7 = document.getElementById("VendorDesc").value;
    if (val7.length < 1) {
        isError = true;
        if(errMsg.trim()=="")
            errMsg = "Please Enter a Vendor Description  ";
        else
            errMsg+=", Vendor Description."
        HighlightInputsForError($("#VendorDesc"), isError)
    }
    HighlightInputsForError($("#drpVenCountryList"), false)
    if (document.getElementById('drpVenCountryList').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Country";
        HighlightInputsForError($("#drpVenCountryList"), isError)
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