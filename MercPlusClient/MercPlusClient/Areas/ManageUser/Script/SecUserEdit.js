/// <reference path="../../../Scripts/mainjs.js" />
$(function () {
    $("#ddlLogin").change(function () {
        $("#ErrorMsgContainer").html("");
        $("#UserId").val($(this).val());
        HighlightInputsForError($("#ddlLogin"), false)
    });
});

$(function () {
    $("#btnDelete").click(function () {
        $("#ErrorMsgContainer").html("");       
        HighlightInputsForError($("#ddlLogin"), false)
        var errMsg = "";
        var isError = false;

        if (document.getElementById("ddlLogin").selectedIndex == 0) {
            isError = true;
            errMsg = "Please Select User ID ";
            HighlightInputsForError($("#ddlLogin"), isError)
        }
        if (isError == true) {
            errMsg += ".";
            ShowRemoveValidationMessage(errMsg, "Warning")
            return false;
        }

    });
});
//document.getElementById('isCountrySelection').value
$(function () {
    $("#CountryCode").change(function () {
        $("#ErrorMsgContainer").html("");
        var CountryCode = $(this).val();
        $.ajax({
            url: "/ManageUser/ManageUser/GetUserListOfACountry",
            type: 'POST',
            data: { CountryCode: CountryCode },
            //data: JSON.stringify({ CountryCode: c, locale: locale }),
            cache: false,
            success: function (data) {
                var items = '<option>Select a User</option>';
                var itemCount = 0;
                if (data.length == 0) {
                    items = '<option>No User Exist For</option>';
                    $('#tblEditDeleteOperationButton').hide();
                }
                else {
                    items = '<option>Select a User</option>';
                    itemCount = 1;
                }
                $.each(data, function (i, UserList) {
                    items += "<option value='" + UserList.Value + "'>" + UserList.Text + "</option>";
                });
                $('#ddlLogin').html(items);
                if (itemCount == 1) {
                    $('#tblEditDeleteOperationButton').show();
                }
                var errCont = $("#ErrorMsgContainer");
                $(errCont).html("");
                addErrorClass(errCont);
            },
            error: function (data) {
            }
        });
    });
});
$(function () {
    $('#btnEdit').click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblEditView"), false)
        HighlightInputsForError($("#ddlLogin"), false)
        var UserId = $("#ddlLogin").val();
        
        var errMsg = "";
        var isError = false;         
       
        if (document.getElementById("ddlLogin").selectedIndex == 0) {
            isError = true;
            errMsg = "Please Select User ID ";
            HighlightInputsForError($("#ddlLogin"), isError)
        }
        if (isError == true) {
            errMsg += ".";
            ShowRemoveValidationMessage(errMsg, "Warning")
            return false;
        }

        $.ajax({
            url: "/ManageUser/ManageUser/GetUserDetailsOfaUserId",
            type: 'POST',
            data: { UserId: UserId },
            //data: JSON.stringify({ CountryCode: c, locale: locale }),
            cache: false,
            success: function (data) {
                $('#tblEditView').show();
                $('#tblView').hide();
                $('#tblEditDeleteOperationButton').hide();
                $('#tblSelectUserToEdit').hide();
                $("#Login").val(data.Login);
                $("#FirstName").val(data.FirstName);
                $("#LastName").val(data.LastName);
                $("#Company").val(data.Company);
                $("#Loccd").val(data.Loccd.trim());
                $('#ApproveAmount').val(data.ApproveAmount);
                $('#ActiveStatus').val(data.ActiveStatus);                
                $('#EmailId').val(data.EmailId);
                $('#Expired').val(data.Expired);               
            },
            error: function (data) {
            }
        });
    });
});

$(function () {
    $('#btnView').click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblView"), false)
        HighlightInputsForError($("#ddlLogin"), false)
        var UserId = $("#ddlLogin").val();
        
        var errMsg = "";
        var isError = false;

        if (document.getElementById("ddlLogin").selectedIndex == 0) {
            isError = true;
            errMsg = "Please Select User ID ";
            HighlightInputsForError($("#ddlLogin"), isError)
        }
        if (isError == true) {
            errMsg += ".";
            ShowRemoveValidationMessage(errMsg, "Warning")
            return false;
        }
        
        $.ajax({
            url: "/ManageUser/ManageUser/GetUserDetailsOfaUserId",
            type: 'POST',
            data: { UserId: UserId },            
            cache: false,
            success: function (data) {                
                $('#tblView').show();
                $('#tblEditView').hide();
                $('#tblEditDeleteOperationButton').hide();
                $('#tblSelectUserToEdit').hide();                
                $("#txtLoginView").val(data.Login);
                $("#txtFirstNameView").val(data.FirstName);
                $("#txtLastNameView").val(data.LastName);
                $("#txtCompanyView").val(data.Company);
                $("#txtLoccdView").val(data.Loccd.trim());
                $('#ApproveAmountView').val(data.ApproveAmount);
                $('#ActiveStatusView').val(data.ActiveStatus);
                $('#EmailIdView').val(data.EmailId);
                $('#ExpiredView').val(data.Expired);
            },
            error: function (data) {
            }
        });
    });
});

$(function () {
    $('#btnBack').click(function () {
        HighlightInputsForError($("#ActiveStatus"), false);
        $("#ErrorMsgContainer").html("");
        $('#tblEditView').hide();
        $('#tblSelectUserToEdit').show();
        $('#tblEditDeleteOperationButton').show();
    });

});



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

    //var isLocExist = false;
    //var LocCode = $("#Loccd").val();
    //$.ajax({
    //    url: "/ManageUser/ManageUser/IsLocationCodeExist",
    //    type: 'POST',
    //    data: { LocCode: LocCode },
    //    cache: false,
    //    success: function (data) {
    //        if (data.isLocationExist == false) {
    //            isLocExist = false;
    //            HighlightInputsForError($("#Loccd"), isError)
    //            errMsg += "Location " + $("#Loccd").val() + " Is invalid. Please enter correct location";
    //            ShowRemoveValidationMessage(errMsg, "Warning")
    //            return false;
    //        }
    //    },
    //    error: function (data) {
    //    }

    //});

    //if (isLocExist == false) {
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

