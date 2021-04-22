$(function () {

    $(".CLSDTFROM").datepicker({
        inline: true,
        defaultDate: null,
        numberOfMonths: 1,
        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() + 1);

        }
    });

    $("#btnVendorAgentCode").click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblPayAgentCode"), false);
        HighlightInputsForError($("#ddlPayAgent_CD"), false);
        if ($("#ddlPayAgent_CD").val() == "-1") {
            HighlightInputsForError($("#ddlPayAgent_CD"), true);
            ShowRemoveValidationMessage("Please Select a Corporate PayAgent Code", "Warning");

        }

        else {
            $('#tblPayAgentCode').css("display", "none");
            $('#btnVendorAgentCode').css("display", "none");
            $('#btnUpdateDelete').css("display", "block");
            $('#tblPayAgentVendor').css("display", "block");
            document.getElementById('hdAgentCode').value = $('#ddlPayAgent_CD').val();

            var url = 'RSVendorsByPayAgent';

            $.post(url, { AgentCode: $("#ddlPayAgent_CD").val() }, function (data) {    //ajax call
                var items = [];
                items.push("<option value=" + "-1" + ">" + "--Select--" + "</option>"); //first item
                for (var i = 0; i < data.length; i++) {
                    items.push("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
                }                                         //all data from the team table push into array
                $("#ddlSMerc_Vendor_CD").html(items.join(' '));
            })

        }//array object bind to dropdown list
    });

    $("#btnUpdateDelete").click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblPayAgentVendor"), false);
        HighlightInputsForError($("#ddlSMerc_Vendor_CD"), false);
        if ($("#ddlSMerc_Vendor_CD").val() == "-1") {
            HighlightInputsForError($("#ddlSMerc_Vendor_CD"), true);
            ShowRemoveValidationMessage("Please Select a MERC Vendor Code", "Warning");

        }
        else {

            var url = 'RSByPayAgentVendor';
            $('#btnUpdate').css("display", "block");
            $('#btnDelete').css("display", "block");
            $('#btnVendorAgentCode').css("display", "none");
            $('#btnUpdateDelete').css("display", "none");
            $('#tblPayAgentCode').css("display", "none");
            $('#tblPayAgentVendor').css("display", "none");
            $('#tblModifyPayAgentVendor').css("display", "block");
            $.post(url, { AgentCode: document.getElementById("hdAgentCode").value, VendorCode: $('#ddlSMerc_Vendor_CD').val() }, function (data) {    //ajax call

                if (data.length != 0) {

                    if (data[0].PayAgentCode != null) { $('#txtPayAgentCode').val(data[0].PayAgentCode); }
                    else { $('#txtPayAgentCode').val(''); }

                    if (data[0].VendorCode != null) { $('#txtPayAgentVendor').val(data[0].VendorCode); }
                    else { $('#txtPayAgentVendor').val('') }

                    if (data[0].LocalAccountCode != null) { $('#txtAccountCode').val(data[0].LocalAccountCode); }
                    else { $('#txtAccountCode').val(''); }

                    if (data[0].SupplierCode != null) { $('#txtSupplierCode').val(data[0].SupplierCode); }
                    else { $('#txtSupplierCode').val(''); }

                    if (data[0].ChangeUser.split(" ")[0] != null) {
                        $('#txtFirstName').val(data[0].ChangeUser.split(" ")[0]);
                    }
                    else {
                        $('#txtFirstName').val('');
                    }
                    if (data[0].ChangeUser.split(" ")[1] != null) {
                        $('#txtLastName').val(data[0].ChangeUser.split(" ")[1]);
                    }
                    else {
                        $('#txtLastName').val('');
                    }
                    if (data[0].ChangeTime != null) { $('#txtChangeTime').val(formatJSONDate(data[0].ChangeTime)); }
                    else { $("#txtChangeTime").val(''); }
                }
                else {
                    ShowRemoveValidationMessage("There is no data to update payagent vendor", "Warning");
                }
            })

        }//array object bind to dropdown list
    });
    $("#btnDelete").click(function () {

        if (checkDelete()) {
            var url = 'DeletePayAgentVendor';

            $.post(url, { AgentCode: $('#txtPayAgentCode').val(), VendorCode: $('#txtPayAgentVendor').val() }, function (data) {    //ajax call

                if (data == 'Success') {

                    //$('#txtPayAgentCode').val('');
                    //$('#txtPayAgentVendor').val('');
                    //$('#txtAccountCode').val('');
                    //$('#txtSupplierCode').val('');
                    //$('#txtFirstName').val('');
                    //$('#txtLastName').val('');
                    //$('#txtChangeTime').val('');
                    ShowRemoveValidationMessage("PayAgent Vendor " + $('#txtPayAgentCode').val() + " / " + $('#txtPayAgentVendor').val() + " removed successfully.", "Success")

                    $('#btnUpdate').css("display", "none");
                    $('#btnDelete').css("display", "none");
                    $('#btnVendorAgentCode').css("display", "none");
                    $('#btnUpdateDelete').css("display", "none");
                    $('#btnAddNew').css("display", "none");
                    $('#btnAddRecord').css("display", "none");
                    $('#btnReturn').css("display", "block");



                    $('#ddlPayAgent_CD').val(-1);
                }
                else {
                    ShowRemoveValidationMessage("Error in deleting the data. Please contact the System Administrator.", "Warning");
                }

            })
        }

    });
    $("#btnUpdate").click(function () {
        if (PayAgenVendorUpdateValidation()) {
            var url = 'UpdatePayAgentVendor';

            $.post(url, { AgentCode: $('#txtPayAgentCode').val(), VendorCode: $('#txtPayAgentVendor').val(), AcctCode: $('#txtAccountCode').val(), SupplierCode: $('#txtSupplierCode').val() }, function (data) {    //ajax call

                if (data == 'Success') {
                    ShowRemoveValidationMessage("PayAgent Vendor " + $('#txtPayAgentCode').val() + " / " + $('#txtPayAgentVendor').val() + " updated successfully.", "Success")
                    //$('#txtPayAgentCode').val('');
                    //$('#txtPayAgentVendor').val('') 
                    $('#txtAccountCode').val('');
                    $('#txtSupplierCode').val('');
                    $('#txtFirstName').val('');
                    $('#txtLastName').val('');
                    $("#txtChangeTime").val(''); 
                    var url = 'RSByPayAgentVendor';
                    
                    $.post(url, { AgentCode: document.getElementById("txtPayAgentCode").value, VendorCode: $('#txtPayAgentVendor').val() }, function (result) {    //ajax call

                        if (result.length != 0) {

                            if (result[0].PayAgentCode != null) { $('#txtPayAgentCode').val(result[0].PayAgentCode); }
                            else { $('#txtPayAgentCode').val(''); }

                            if (result[0].VendorCode != null) { $('#txtPayAgentVendor').val(result[0].VendorCode); }
                            else { $('#txtPayAgentVendor').val('') }

                            if (result[0].LocalAccountCode != null) { $('#txtAccountCode').val(result[0].LocalAccountCode); }
                            else { $('#txtAccountCode').val(''); }

                            if (result[0].SupplierCode != null) { $('#txtSupplierCode').val(result[0].SupplierCode); }
                            else { $('#txtSupplierCode').val(''); }

                            if (result[0].ChangeUser.split(" ")[0] != null) {
                                $('#txtFirstName').val(result[0].ChangeUser.split(" ")[0]);
                            }
                            else {
                                $('#txtFirstName').val('');
                            }
                            if (result[0].ChangeUser.split(" ")[1] != null) {
                                $('#txtLastName').val(result[0].ChangeUser.split(" ")[1]);
                            }
                            else {
                                $('#txtLastName').val('');
                            }
                            if (result[0].ChangeTime != null)
                            {
                                $('#txtChangeTime').val(formatJSONDate(result[0].ChangeTime));
                            }
                            else { $("#txtChangeTime").val(''); }
                        }
                        else {
                            ShowRemoveValidationMessage("There is no data to update payagent vendor", "Warning");
                        }
                    });
                }
                else {
                    ShowRemoveValidationMessage("Error in updating the data. Please contact the System Administrator.", "Warning");
                }

            });
        }

    });
    $("#btnAddNew").click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblPayAgentCode"), false);
        HighlightInputsForError($("#ddlSMerc_Vendor_CD"), false);
        HighlightInputsForError($("#ddlPayAgent_CD"), false);

        HighlightInputsForError($("#ddlNPayAgent_CD"), false);
        HighlightInputsForError($("#ddlNMerc_Vendor_CD"), false);
        HighlightInputsForError($("#txtNAcctCode"), false);
        HighlightInputsForError($("#txtNSupplierCode"), false);
        $('#ddlNPayAgent_CD').val(-1);
        $('#ddlNMerc_Vendor_CD').val(-1);
        $('#txtNAcctCode').val("");
        $('#txtNSupplierCode').val("");

        $('#btnUpdate').css("display", "none");
        $('#btnDelete').css("display", "none");
        $('#btnVendorAgentCode').css("display", "none");
        $('#btnUpdateDelete').css("display", "none");
        $('#btnAddNew').css("display", "none");
        $('#btnAddRecord').css("display", "block");
        $('#btnReturn').css("display", "block");

        $('#tblPayAgentCode').css("display", "none");
        $('#tblPayAgentVendor').css("display", "none");
        $('#tblModifyPayAgentVendor').css("display", "none");
        $('#tblAddNew').css("display", "block");

        var url = 'RSByPayAgentCodeList';
        $.post(url, function (data) {    //ajax call
            var items = [];
            items.push("<option value=" + "-1" + ">" + "--Select--" + "</option>"); //first item
            for (var i = 0; i < data.length; i++) {
                items.push("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
            }                                         //all data from the team table push into array
            $("#ddlNPayAgent_CD").html(items.join(' '));
        });

        var url = 'RSAllVendors';
        $.post(url, function (data) {    //ajax call
            var items = [];
            items.push("<option value=" + "-1" + ">" + "--Select--" + "</option>"); //first item
            for (var i = 0; i < data.length; i++) {
                items.push("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
            }                                         //all data from the team table push into array
            $("#ddlNMerc_Vendor_CD").html(items.join(' '));
        });

    });
    $("#btnReturn").click(function () {
        $("#ErrorMsgContainer").html("");
        $('#btnUpdate').css("display", "none");
        $('#btnDelete').css("display", "none");
        $('#btnVendorAgentCode').css("display", "block");
        $('#btnUpdateDelete').css("display", "none");
        $('#btnAddNew').css("display", "block");
        $('#btnAddRecord').css("display", "none");
        $('#btnReturn').css("display", "none");

        $('#tblPayAgentCode').css("display", "block");
        $('#tblPayAgentVendor').css("display", "none");
        $('#tblModifyPayAgentVendor').css("display", "none");
        $('#tblAddNew').css("display", "none");

        $('#ddlPayAgent_CD').val(-1);

    });
    $("#btnAddRecord").click(function () {

        if (PayAgenVendorAddValidation()) {

            var url = 'CreatePayAgentVendor';

            $.post(url, { AgentCode: $('#ddlNPayAgent_CD').val(), VendorCode: $('#ddlNMerc_Vendor_CD').val(), AcctCode: $('#txtNAcctCode').val(), SupplierCode: $('#txtNSupplierCode').val() }, function (data) {    //ajax call

                if (data == 'Success') {
                    ShowRemoveValidationMessage("PayAgent Vendor " + $('#ddlNPayAgent_CD').val() + " / " + $('#ddlNMerc_Vendor_CD').val() + " added successfully.", "Success")
                }
                else if (data == 'Exist') {
                    ShowRemoveValidationMessage("The combination of PayAgent Code : " + $('#ddlNPayAgent_CD').val() + " and Vendor Code : " + $('#ddlNMerc_Vendor_CD').val() + " already exists. Not added.", "Warning")
                }
                else {
                    ShowRemoveValidationMessage("Error in inserting the data. Please contact the System Administrator.", "Warning");

                }
                //$('#ddlNPayAgent_CD').val(-1);
                //$('#ddlNMerc_Vendor_CD').val(-1);
                //$('#txtNAcctCode').val("");
                //$('#txtNSupplierCode').val("");
            });
        }

    });
});

function PayAgenVendorAddValidation() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    ClearHighlightErrorForInputs($("#tblAddNew"), isError);

    HighlightInputsForError($("#ddlNPayAgent_CD"), false);
    if ($("#ddlNPayAgent_CD").val() == "-1") {
        isError = true;
        HighlightInputsForError($("#ddlNPayAgent_CD"), isError);
    }
    HighlightInputsForError($("#ddlNMerc_Vendor_CD"), false);
    if ($("#ddlNMerc_Vendor_CD").val() == "-1") {
        isError = true;
        HighlightInputsForError($("#ddlNMerc_Vendor_CD"), isError);
    }
    HighlightInputsForError($("#txtNAcctCode"), false);
    if ($("#txtNAcctCode").val() == "") {
        isError = true;
        HighlightInputsForError($("#txtNAcctCode"), isError);
    }
    HighlightInputsForError($("#txtNSupplierCode"), false);
    if ($("#txtNSupplierCode").val() == "") {
        isError = true;
        HighlightInputsForError($("#txtNSupplierCode"), isError);
    }


    if (isError == true) {
        errMsg += "Please fill the mandatory fields(*).";
        ShowRemoveValidationMessage(errMsg, "Warning");
        return false;
    }
    else {
        return true;
    }
}
function PayAgenVendorUpdateValidation() {
    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");
    ClearHighlightErrorForInputs($("#tblModifyPayAgentVendor"), isError);

    HighlightInputsForError($("#txtPayAgentCode"), false);
    if ($("#txtPayAgentCode").val() == "") {
        isError = true;
        HighlightInputsForError($("#txtPayAgentCode"), isError);
    }
    HighlightInputsForError($("#txtPayAgentVendor"), false);
    if ($("#txtPayAgentVendor").val() == "") {
        isError = true;
        HighlightInputsForError($("#txtPayAgentVendor"), isError);
    }
    HighlightInputsForError($("#txtAccountCode"), false);
    if ($("#txtAccountCode").val() == "") {
        isError = true;
        HighlightInputsForError($("#txtAccountCode"), isError);
    }
    HighlightInputsForError($("#txtSupplierCode"), false);
    if ($("#txtSupplierCode").val() == "") {
        isError = true;
        HighlightInputsForError($("#txtSupplierCode"), isError);
    }


    if (isError == true) {
        errMsg += "Please fill the mandatory fields(*).";
        ShowRemoveValidationMessage(errMsg, "Warning");
        return false;
    }
    else {
        return true;
    }
}
function checkDelete() {
    if (confirm("Are you sure you want to delete this record ?"))
        //document.ChangeTable.stopFlag.value = "true";
        return true;
    else
        //document.ChangeTable.stopFlag.value = "false";
        return false;
}
function trimString(str) {
    str = this != window ? this : str;
    return str.replace(/^\s+/g, '').replace(/\s+$/g, '');
}
function formatJSONDate(jsonDate) {
    var dateAsFromServerSide = jsonDate ///Date(1291374337981)/

    ////Now let's convert it to js format
    ////Example: Fri Dec 03 2010 16:37:32 GMT+0530 (India Standard Time)
    var parsedDate = new Date(parseInt(dateAsFromServerSide.substr(6)));

    var jsDate = new Date(parsedDate); //Date object
    var month = jsDate.getMonth();
    var day = jsDate.getDate();
    month = month + 1;

    month = month + "";

    if (month.length == 1) {
        month = "0" + month;
    }

    day = day + "";

    if (day.length == 1) {
        day = "0" + day;
    }
    
    var h = jsDate.getHours();
    var m = jsDate.getMinutes();
    var s = jsDate.getSeconds();
    var ampm = h >= 12 ? 'PM' : 'AM';
    h = h % 12;
    h = h ? h : 12; // the hour '0' should be '12'
    
    h = h < 10 ? '0' + h : h;
    m = m < 10 ? '0' + m : m;
    s = s < 10 ? '0' + s : s;
    
    var newDate = jsDate.getFullYear() + "-" + month + "-" + day + " " + h + ":" + m + ":" + s + " " + ampm ;
    return newDate;
}


