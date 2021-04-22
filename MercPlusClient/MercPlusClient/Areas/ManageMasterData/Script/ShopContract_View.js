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
    $(".CLSDTTO").datepicker({
        inline: true,
        numberOfMonths: 1,
        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() - 1);

        }
    });
    $("#btnContractSubmit").click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblAddUpdateShopContract"), false);
        if (checkShopContractAddValidation()) {


            var url = 'ShopContractAdd';

            $.post(url, { ShopCode: $("#drpShop").val(), ManualCode: $("#drpManual").val(), ModeCode: $("#drpMode").val(), RepairCode: $("#txtRepairCode").val(), ContAmt: $("#txtAmount").val(), Currency: $("#txtCurrencyAmount").val(), EffectiveDate: $("#txtEffecDate").val(), ExpireDate: $("#txtExpDate").val() }, function (data) {    //ajax call
                if (data == "Success") {
                    ShowRemoveValidationMessage("Data has been saved successfully", "Success");
                }
                else {
                    ShowRemoveValidationMessage(data, "Warning");

                }
            });
        }

    });
    $("#btnUpdateContract").click(function () {
        $("#ErrorMsgContainer").html("");
        if (checkShopContractAddValidation()) {


            var url = 'ShopContractUpdate';

            $.post(url, { ShopContId: $("#txtShopContractID").val(), ShopCode: $("#drpShop").val(), ManualCode: $("#drpManual").val(), ModeCode: $("#drpMode").val(), RepairCode: $("#txtRepairCode").val(), ContAmt: $("#txtAmount").val(), Currency: $("#txtCurrencyAmount").val(), EffectiveDate: $("#txtEffectiveDate").val(), ExpireDate: $("#txtExpDate").val() }, function (data) {    //ajax call
                if (data == "Success") {
                    ShowRemoveValidationMessage("Data has been updated successfully", "Success");
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
                            if (result[0].ChangeTime != null) { $('#txtChangeTime').val(formatJSONDate(result[0].ChangeTime)); }
                            else { $("#txtChangeTime").val(''); }
                        }
                        else {
                            ShowRemoveValidationMessage("There is no data to update payagent vendor", "Warning");
                        }
                    });
                }
                else {
                    ShowRemoveValidationMessage(data, "Warning");

                }
            });
        }

    });



    $("#drpShop").change(function () {

        var url = 'GetShopContCurrencyCode';

        $.post(url, { ShopCode: $("#drpShop").val() }, function (data) {    //ajax call
            //all data from the team table push into array
            $("#txtCurrencyAmount").val(data);
        })

    });
    $("#drpManual").change(function (e) {
        e.preventDefault();
        var url = 'GetRSAllManualModes';

        $.post(url, { ManualCode: $("#drpManual").val() }, function (data) {    //ajax call
            var items = [];

            for (var i = 0; i < data.length; i++) {
                items.push("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
            }
            $("#drpMode").html(items.join(' '));
        })

    });

    $('#btnSearch').click(function (e) {
        e.preventDefault();
        var url = 'GetRSAllContracts_View';
        $.post(url, {
            ShopCode: $('#ddlShop').val(),
            RepairCode: $('#txtRepairCode').val(),
            Mode: $('#ddlMode').val()
        }, function (result) {

            $('#gridContent').html(result);
            //document.getElementById('tblModification').style.display = "block";
            
        });
    });
    $("#btnSelectAll").click(function (e) {
        e.preventDefault();
        var ischecked = true;
        $('#grdShopContract').find("input:checkbox").each(function () {
            this.checked = ischecked;
        });
    });
    $("#btnClear").click(function (e) {

        e.preventDefault();
        var ischecked = false;
        $('#grdShopContract').find("input:checkbox").each(function () {
            this.checked = ischecked;
        });
    });
    $("#btnDelete").click(function (e) {
       
        e.preventDefault();
        $("#ErrorMsgContainer").html("");
        var _griddata = gridTojson();
        if (_griddata != "") {
            if (CheckDelete()) {
                var url = 'DeleteShopContract';
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: { gridData: _griddata }
                }).done(function (data) {
                    if (data != "") {
                        if (data == "Success") {
                            ShowRemoveValidationMessage("Data deleted successfully", "Success");
                        }
                        else {
                            ShowRemoveValidationMessage(data, "Warning");
                        }

                    }
                });
            }
        }
        else {
            ShowRemoveValidationMessage("Please select at least one row in a grid...", "Warning");
        }


    });
    $("#btnExpDateUpdate").click(function (e) {
        e.preventDefault();
        $("#ErrorMsgContainer").html("");
        var _griddata = gridTojson();
        if (_griddata != "") {
            var url = 'UpdateExpDateForShopContract';
            $.ajax({
                url: url,
                type: 'POST',
                data: { gridData: _griddata, expDate: $('#txtExpDateSubmit').val() }
            }).done(function (data) {
                if (data != "") {
                    if (data == "Success") {
                        ShowRemoveValidationMessage("Data has been updated successfully", "Success");
                    }
                    else {
                        ShowRemoveValidationMessage(data, "Warning");
                    }
                }
            });
        }
        else {
            ShowRemoveValidationMessage("Please select at least one row in a grid...", "Warning");
        }
    });
});

function CheckDelete() {
    if (confirm("Are you sure you want to delete this record ?"))
        return true;
    else
        return false;
}
function gridTojson() {
    var json = '';
    var $ccol = [];
    $("#grdShopContract tbody tr").each(function (i, row) {
        var $tRow = $(row);

        if ($tRow.find('input[type=checkbox]').prop('checked') == true) {


            $ccol.push($tRow.find('input[type=checkbox]').val());
        }
    });

    json += $ccol.join(",") + '';
    return json;
}

function checkShopContractAddValidation() {
    var IsError = false;
    HighlightInputsForError($("#drpShop"), false);
    if (document.getElementById('drpShop').value == '') {
        IsError = true;
        HighlightInputsForError($("#drpShop"), IsError);
    }
    HighlightInputsForError($("#drpManual"), false);
    if (document.getElementById('drpManual').value == '') {
        IsError = true;
        HighlightInputsForError($("#drpManual"), IsError);

    }
    HighlightInputsForError($("#drpMode"), false);
    if (document.getElementById('drpMode').value == '') {
        IsError = true;
        HighlightInputsForError($("#drpMode"), IsError);
    }
    HighlightInputsForError($("#txtRepairCode"), false);
    if (document.getElementById('txtRepairCode').value == '') {
        IsError = true;
        HighlightInputsForError($("#txtRepairCode"), IsError);
    }
    HighlightInputsForError($("#txtAmount"), false);
    if (document.getElementById('txtAmount').value == '') {
        IsError = true;
        HighlightInputsForError($("#txtAmount"), IsError);
    }

    HighlightInputsForError($("#txtEffecDate"), false);
    if ($('#txtEffecDate').val() == '') {
        IsError = true;
        HighlightInputsForError($("#txtEffecDate"), IsError);
    }
    if (IsError == true) {
        var errMsg = "";
        errMsg += "Please fill the mandatory fields(*).";
        ShowRemoveValidationMessage(errMsg, "Warning");
        return false;

    }
    else {
        HighlightInputsForError($("#txtAmount"), false);
        if (!(is_pay(document.getElementById('txtAmount').value))) {
            HighlightInputsForError($("#txtAmount"), true);
            ShowRemoveValidationMessage("Please Enter Amount with 4 Decimal Places ", "Warning");
            return false;
        }
        else {
            return true;
        }
    }


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

    var newDate = jsDate.getFullYear() + "-" + month + "-" + day + " " + h + ":" + m + ":" + s + " " + ampm;
    return newDate;
}

function doSubmit(strUrlModifier) {

    $("#ErrorMsgContainer").html("");

    if (CheckDelete()) {
        var url = 'DeleteShopContract';
        $.ajax({
            url: url,
            type: 'POST',
            data: { gridData: strUrlModifier }
        }).done(function (data) {
            if (data != "") {
                if (data == "Success") {
                    ShowRemoveValidationMessage("Data deleted successfully", "Success");
                }
                else {
                    ShowRemoveValidationMessage(data, "Warning");
                }

            }
        });
    }



}
function is_pay(string) {
    var check_it = (string);

    if (check_it == "") return true;

    if (check_it.search(/^\d*$|^\d*\.\d{4}$/) != -1) {
        return true;
    }
    else {
        return false;
    }


}





