function ClientValidation() {
    
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    ClearHighlightErrorForInputs($("#tblEditView"), isError)

    if ($("#Login").val() == "") {
        isError = true;
        errMsg = "Please Enter User Id ";
        HighlightInputsForError($("#Login"), isError)
    }
    if ($("#FirstName").val() == "") {
        isError = true;
        errMsg = errMsg.trim() == "" ? "Please Enter First name " : errMsg += ", First name";
        HighlightInputsForError($("#FirstName"), isError)
    }
    if ($("#LastName").val() == "") {
        isError = true;
        errMsg = errMsg.trim() == "" ? "Please Enter Last Name " : errMsg += ", Last Name";
        HighlightInputsForError($("#LastName"), isError)
    }
    if ($("#Company").val() == "") {
        isError = true;
        errMsg = errMsg.trim() == "" ? "Please Enter Company " : errMsg += ", Company";
        HighlightInputsForError($("#Company"), isError)
    }
    if ($("#Loccd").val() == "") {
        isError = true;
        errMsg = errMsg.trim() == "" ? "Please Enter Location Code " : errMsg += ", Location Code";
        HighlightInputsForError($("#Loccd"), isError)
    }
    if ($("#ApproveAmount").val() == "") {
        isError = true;
        errMsg = errMsg.trim() == "" ? "Please Enter Approval Amount " : errMsg += ", Approval Amount";
        HighlightInputsForError($("#ApproveAmount"), isError)
    }

    if ($("#ApproveAmount").val() != "" && isNumber($("#ApproveAmount").val()) == false) {
        isError = true;
        errMsg = errMsg.trim() == "" ? "Please Enter valid Approval Amount " : errMsg += " & valid Approval Amount";
        HighlightInputsForError($("#ApproveAmount"), isError)
    }
    if ($("#EmailId").val() == "") {
        isError = true;
        errMsg = errMsg.trim() == "" ? "Please Enter Email Id " : errMsg += ", Email Id";
        HighlightInputsForError($("#EmailId"), isError)
    }

    if (!ValidateEmail($("#EmailId").val())) {
        isError = true;
        errMsg = errMsg.trim() == "" ? "Please Enter valid Email Id " : errMsg += ", Email Id";
        HighlightInputsForError($("#EmailId"), isError)
    }
    var isLocExist = false;
    //var LocCode = $("#Loccd").val();
    //$.ajax({
    //    url: "/ManageUser/ManageUser/IsLocationCodeExist",
    //    type: 'POST',
    //    data: { LocCode: LocCode },
    //    cache: false,
    //    success: function (data) {
    //        if (data.isLocationExist == false) {
    //            isLocExist = true;
    //            HighlightInputsForError($("#Loccd"), isError)
    //            errMsg += "Location " + $("#Loccd").val() + " Is invalid. Please enter correct location";
    //            ShowRemoveValidationMessage(errMsg, "Warning")
    //            return false;
    //        }
    //    },
    //    error: function (data) {
    //    }

    //});

    //if (isLocExist == true) {
    //    return false;
    //}

    if (isError == true) {        
        errMsg += ".";
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        var answer = confirm("Are you sure, you want to save the record?")
        if (!answer) {
            return false;
        }
        return true;
    }
}
function ValidateNumber(e) {
    var evt = (e) ? e : window.event;    
    var charCode = (evt.keyCode) ? evt.keyCode : evt.which;
    
    if (charCode > 31 && (charCode < 48 || (charCode > 57 && charCode != 190))) {
        return false;
    }
    return true;
}

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function ValidateEmail(emailid) {
    var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;    
    if (reg.test(emailid) == false) {
        return false;
    }

    return true;
}

