
function GetMode() {
    var c = $("#drpCustomerCodeList").val();
    $.ajax({
        url: "/ManageMasterData/ManageMasterData/ViewTransmitModeDetails",
        type: 'POST',
        datatype: 'json',
        data: { CustomerID: c },

        cache: false,
        success: function (data) {

            $('#tblModeSelection').show();
            $('#tblCustomerDtl').hide();
            var items = '<option>Select a User</option>';
            $.each(data, function (i, ModeCodeList) {
                items += "<option value='" + ModeCodeList.Value + "'>" + ModeCodeList.Text + "</option>";
            });
            $('#drpModeLCodeList').html(items);



        },
        error: function (data) {
            alert("bye");
        }
    });



}

function Submit() {
    var c = $("#CustomerCode").val();
    var d = $("#drpModeLCodeList").val();
    alert(c);
    // var a = JSON.stringify({ customerId: $(this).val() };
    $.ajax({
        url: "/ManageMasterData/ManageMasterData/ViewTransmitEditDetails",
        type: 'POST',
        datatype: 'json',
        data: { CustomerID: c, ModeID: d },

        cache: false,
        success: function (data) {
            $('#tblDisplayData').show();
            $('#tblModeSelection').hide();

            var items = '<option>Select a User</option>';
            $.each(data, function (i, ModeCodeList) {
                items += "<option value='" + ModeCodeList.Value + "'>" + ModeCodeList.Text + "</option>";
            });

            $("#drpRRISList").val(data.RRISList);
            $("#AccountCode").val(data.AccountCode);
            $("#ChangeUserTransmit").val(data.ChangeUserTrans);
            $("#ChangeTimeTransmit").val(data.ChangeTimeTrans);


        },
        error: function (data) {

        }
    });
}

$(function () {
    $('#btnQueryLoc').click(function () {
        $('#divLocationCode').show();

        var c = $("#txtLocCode").val();

        $.ajax({
            url: "/ManageMasterData/ManageMasterData/GetAllDetailsForLocation",
            type: 'POST',
            data: { id: c },
            cache: false,
            success: function (data) {
                $("#LocCode").val(data.LocCode)
                $("#LocDesc").val(data.LocDesc);
                $("#CountryCode").val(data.CountryCode);
                $("#drpContactEqsalSW").val(data.ContactEqsalSW);
                $("#drpRegionCode").val(data.RegionCode);
                $("#ChangeUserLoc").val(data.ChangeUserLoc);
                $("#ChangeTimeLoc").val(data.ChangeTimeLoc);


            },
            error: function (data) {

            }
        });
    });
})

$(function () {

    $("#btnQueryCon").click(function () {
        $("#divCountryCode").show();

        var c = $(drpCountryList).val();
        // var a = JSON.stringify({ customerId: $(this).val() };
        $.ajax({
            url: "/ManageMasterData/ManageMasterData/GetAllDetailsForCountry",
            type: 'POST',
            datatype: 'json',
            data: { id: c },
            cache: false,
            success: function (data) {
                $("#CountryCode").val(data.CountryCode);
                $("#CountryDescription").val(data.CountryDescription);
                $("#AreaCode").val(data.AreaCode);
                $("#RepairLimitAdjFactor").val(data.RepairLimitAdjFactor);

                $("#ChangeUserCon").val(data.ChangeUserCon);
                $("#ChangeTimeCon").val(data.ChangeTimeCon);

            },
            error: function (data) {

            }
        });
        //});
        // });


    });

})

$(function () {
    $("#drpCustomerList").bind("change", function () {
        var c = $(this).val();
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
                $("#changeUserCust").val(data.ChangeUser);
                $("#changeTimeUser").val(data.ChangeTime);


            },
            error: function (data) {

            }
        });
    });
})

$(function () {
    $("#drpEquipmentTypeList").bind("change", function () {
        var c = $(this).val();

        $.ajax({
            url: "/ManageMasterData/ManageMasterData/GetAllDetailsForEquipmentType",
            type: 'POST',
            datatype: 'json',
            data: { id: c },
            cache: false,
            success: function (data) {
                $("#EquipmentDescription").val(data.EquipmentDescription);
                $("#ChangeUser").val(data.ChangeUser);
                $("#ChTime").val(data.ChTime);


            },
            error: function (data) {

            }
        });
    });
})

$(function () {
    $('#btnQueryRepairLocCode').click(function () {
        $("#ErrorMsgContainer").html("");
        var c = $("#drpRepairLocationCode").val();
        HighlightInputsForError($("#drpRepairLocationCode"), false);
        if (c.trim() == "") {
            HighlightInputsForError($("#drpRepairLocationCode"), true);
            ShowRemoveValidationMessage("Please Select a Repair Location code to Query", "Warning")
        }
        else {
            $('#divRepairLocationCode').show();
            //var c = $("#drpRepairLocationCode").val();
            $.ajax({

                url: "/ManageMasterData/ManageMasterData/GetAllDetailsForRepairLocationCode",
                type: 'POST',
                data: { id: c },

                cache: false,
                success: function (data) {

                    $("#txtRepairLocationCode").val(data.RepairCod);
                    $("#txtDescription").val(data.RepairDescription);

                    $("#txtRepairLocationCode").attr("readonly", true);
                    $("#txtDescription").attr("readonly", true);
                },
                error: function (data) {
                }
            });
        }
    });
});



// Created By Afroz

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
    var newDate = jsDate.getFullYear() + "-" + month + "-" + day + " " + jsDate.getHours() + ":" + jsDate.getMinutes() + ":" + jsDate.getSeconds();
    return newDate;
}

$(function () {

    //$(".CLSDTFROM").datepicker({
    //    inline: true,
    //    defaultDate: null,
    //    numberOfMonths: 1,
    //    onSelect: function (selected) {
    //        var dt = new Date(selected);
    //        dt.setDate(dt.getDate() + 1);
    //        $(".CLSDTTO").datepicker("option", "minDate", dt);
    //    }
    //});
    //$(".CLSDTTO").datepicker({
    //    inline: true,
    //    numberOfMonths: 1,
    //    onSelect: function (selected) {
    //        var dt = new Date(selected);
    //        dt.setDate(dt.getDate() - 1);
    //        $(".CLSDTFROM").datepicker("option", "maxDate", dt);
    //    }
    //});

    // For ShopLimit by Afroz
    $("#btnFirstUpdateNew").click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblShop"), false);
        HighlightInputsForError($("#drpShop"), false);

        if ($("#drpShop").val() == '') {
            HighlightInputsForError($("#drpShop"), true);
            ShowRemoveValidationMessage("Please Select a Shop to Update", "Warning");
        }
        else {

            $("#drpMode").empty();
            var url = 'fillModeByShop';
            $('#tblShop').css("display", "none");
            $('#tblMode').css("display", "block");
            $('#btnFirstUpdateNew').css("display", "none");
            $('#btnSecondUpdateNew').css("display", "block");
            $.post(url, { ShopCode: $("#drpShop").val() }, function (data) {    //ajax call
                var items = [];
                items.push("<option value=" + -1 + ">" + "--Select--" + "</option>"); //first item
                for (var i = 0; i < data.length; i++) {
                    items.push("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
                }                                         //all data from the team table push into array
                $("#drpMode").html(items.join(' '));
            })
        }//array object bind to dropdown list
    });

    $("#btnSecondUpdateNew").click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblMode"), false);
        HighlightInputsForError($("#drpMode"), false);
        if ($("#drpMode").val() == '-1') {
            HighlightInputsForError($("#drpMode"), true);
            ShowRemoveValidationMessage("Please Select a Mode to Update", "Warning");
        }
        else {

            var url = 'fillRSByShopMode';
            $('#tblShop').css("display", "none");
            $('#tblMode').css("display", "none");
            $('#btnFirstUpdateNew').css("display", "none");
            $('#btnSecondUpdateNew').css("display", "none");
            $('#tblUpdate').css("display", "block");
            $('#btnUpdate').css("display", "block");
            $('#btnAuditTrail').css("display", "block");
            $.post(url, { ShopCode: $("#drpShop").val(), ModeCode: $("#drpMode").val() }, function (data) {

                if (data.length != 0) {
                    if (data[0].ShopCode != null) {
                        $("#txtShopCode").val(data[0].ShopCode);
                    }
                    if (data[0].Mode != null) {
                        $("#txtModeCode").val(data[0].Mode);
                    }
                    if (data[0].ModeDesc != null) {
                        $("#txtModeDesc").val(data[0].ModeDesc);
                    }
                    if (data[0].RepairAmtLimit != null) {
                        $("#txtSuspendLimitUpdate").val(data[0].RepairAmtLimit);
                    }
                    if (data[0].ShopMaterialLimit != null) {
                        $("#txtShopMaterialLimitUpdate").val(data[0].ShopMaterialLimit);
                    }
                    if (data[0].AutoApproveLimit != null) {
                        $("#txtAutoApprovalLimitUpdate").val(data[0].AutoApproveLimit);
                    }
                    if (data[0].ChangeUser != null) {
                        $("#txtFirstName").val(data[0].FName);//.split("|")[0]);
                    }
                    if (data[0].ChangeUser != null) {
                        $("#txtLastName").val(data[0].LName);//.split("|")[1]);
                        //$("#txtLastName").val(CU[1]);
                    }

                    if (data[0].ChangeTime != null) {
                        $("#txtChangeTime").val(formatJSONDate(data[0].ChangeTime));
                    }
                }

            })
        }
    });

    $("#btnAddRecord").click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblAdd"), false);
        if (checkValidationForShopLimitAdd()) {
            var url = 'InsertShopLimit';
            $.post(url, { ShopCode: $("#drpShopAdd").val(), ModeCode: $("#drpModeAdd").val(), SuspendLimit: $('#txtSusLimitAdd').val(), ShopMatLimit: $('#txtShopMatLimitAdd').val(), AutoAppLimit: $('#txtAutoAppLimitAdd').val() }, function (data) {

                if (data == 'Success') {
                    ShowRemoveValidationMessage("Successfully Inserted...", "Success");
                }

                else {
                    ShowRemoveValidationMessage(data, "Warning");
                }

            });
        }

    });

    $("#btnUpdate").click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblUpdate"), false);
        if (checkValidationShopLimitEdit()) {
            var url = 'UpdateShopLimit';
            $.post(url, { ShopCode: $("#txtShopCode").val(), ModeCode: $("#txtModeCode").val(), SuspendLimit: $('#txtSuspendLimitUpdate').val(), ShopMatLimit: $('#txtShopMaterialLimitUpdate').val(), AutoAppLimit: $('#txtAutoApprovalLimitUpdate').val() }, function (data) {

                if (data == "Success") {
                    ShowRemoveValidationMessage("Successfully Updated...", "Success");
                    var url = 'fillRSByShopMode';
                    $.post(url, { ShopCode: $("#txtShopCode").val(), ModeCode: $("#txtModeCode").val() }, function (result) {
                        $("#txtSuspendLimitUpdate").val('');
                        $("#txtShopMaterialLimitUpdate").val('');
                        $("#txtAutoApprovalLimitUpdate").val('');
                        if (result.length != 0) {
                            if (result[0].ShopCode != null) {
                                $("#txtShopCode").val(result[0].ShopCode);
                            }
                            if (result[0].Mode != null) {
                                $("#txtModeCode").val(result[0].Mode);
                            }
                            if (result[0].ModeDesc != null) {
                                $("#txtModeDesc").val(result[0].ModeDesc);
                            }

                            if (result[0].RepairAmtLimit != null) {
                                $("#txtSuspendLimitUpdate").val(result[0].RepairAmtLimit);
                            }

                            if (result[0].ShopMaterialLimit != null) {
                                $("#txtShopMaterialLimitUpdate").val(result[0].ShopMaterialLimit);
                            }

                            if (result[0].AutoApproveLimit != null) {
                                $("#txtAutoApprovalLimitUpdate").val(result[0].AutoApproveLimit);
                            }
                            if (result[0].ChangeUser != null) {
                                $("#txtFirstName").val(result[0].FName); //.val((result[0].ChangeUser).split('|')[0]);
                            }
                            if (result[0].ChangeUser != null) {
                                $("#txtLastName").val(result[0].LName); //.val((result[0].ChangeUser).split('|')[1]);
                            }

                            if (result[0].ChangeTime != null) {
                                $("#txtChangeTime").val(formatJSONDate(result[0].ChangeTime));
                            }
                        }

                    })
                }

                else {
                    ShowRemoveValidationMessage(data, "Warning");

                }

            });
        }
    });

    $("#btnAddNew").click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblAdd"), false);

        $('#tblShop').css("display", "none");
        $('#tblMode').css("display", "none");
        $('#btnFirstUpdateNew').css("display", "none");
        $('#btnSecondUpdateNew').css("display", "none");
        $('#btnUpdate').css("display", "none");
        $('#btnUpdate').css("display", "none");
        $('#btnAuditTrail').css("display", "none");
        $('#btnAddNew').css("display", "none");
        $('#btnAddRecord').css("display", "block");
        $('#tblAdd').css("display", "block");
        $('#tblUpdate').css("display", "none");
        $('#btnReturn').css("display", "block");
        $('#txtSusLimitAdd').val('');
        $('#txtShopMatLimitAdd').val('');
        $('#txtAutoAppLimitAdd').val('');
        var url = 'fillShopAdd';
        $.post(url, function (data) {
            var items = [];
            items.push("<option value=" + -1 + ">" + "--Select--" + "</option>"); //first item
            for (var i = 0; i < data.length; i++) {
                items.push("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
            }                                         //all data from the team table push into array
            $("#drpShopAdd").html(items.join(' '));
        });

        var url = 'fillModeAdd';
        $.post(url, function (data) {
            var items = [];
            items.push("<option value=" + -1 + ">" + "--Select--" + "</option>"); //first item
            for (var i = 0; i < data.length; i++) {
                items.push("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
            }                                         //all data from the team table push into array
            $("#drpModeAdd").html(items.join(' '));
        });

    });

    $("#btnReturn").click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblShop"), false);
        HighlightInputsForError($("#drpShop"), false);
        $('#drpShop').val('');
        $('#tblAdd').css("display", "none");
        $('#btnReturn').css("display", "none");
        $('#btnAddRecord').css("display", "none");
        $('#tblShop').css("display", "block");
        $('#btnFirstUpdateNew').css("display", "block");
        $('#btnAddNew').css("display", "block");


    });

});

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
function checkValidationForShopLimitAdd() {
    var isError = false;

    HighlightInputsForError($("#drpShopAdd"), false);
    if (document.getElementById('drpShopAdd').value == "-1" || document.getElementById('drpShopAdd').value == "") {
        isError = true;
        HighlightInputsForError($("#drpShopAdd"), isError);

    }
    HighlightInputsForError($("#drpModeAdd"), false);
    if (document.getElementById('drpModeAdd').value == "" || document.getElementById('drpModeAdd').value == "-1") {
        isError = true;
        HighlightInputsForError($("#drpModeAdd"), isError);

    }
    HighlightInputsForError($("#txtSusLimitAdd"), false);
    if (document.getElementById('txtSusLimitAdd').value == "") {
        isError = true;
        HighlightInputsForError($("#txtSusLimitAdd"), isError);

    }

    HighlightInputsForError($("#txtShopMatLimitAdd"), false);
    if (document.getElementById('txtShopMatLimitAdd').value == "") {
        isError = true;
        HighlightInputsForError($("#txtShopMatLimitAdd"), isError);


    }

    HighlightInputsForError($("#txtAutoAppLimitAdd"), false);
    if (document.getElementById('txtAutoAppLimitAdd').value == "") {
        isError = true;
        HighlightInputsForError($("#txtAutoAppLimitAdd"), isError);

    }

    if (isError == true) {
        var errMsg = "";
        errMsg += "Please fill the mandatory fields(*).";
        ShowRemoveValidationMessage(errMsg, "Warning");
        return false;
    }
    else {
        HighlightInputsForError($("#txtSusLimitAdd"), false);
        if (!is_pay(document.getElementById('txtSusLimitAdd').value)) {
            HighlightInputsForError($("#txtSusLimitAdd"), true);
            ShowRemoveValidationMessage("Please Enter Numbers Only  - 2 Decimal Places", "Warning");
            return false;
        }
        HighlightInputsForError($("#txtShopMatLimitAdd"), false);
        if (!is_pay(document.getElementById('txtShopMatLimitAdd').value)) {
            HighlightInputsForError($("#txtShopMatLimitAdd"), true);
            ShowRemoveValidationMessage("Please Enter Numbers Only  - 2 Decimal Places ", "Warning");
            return false;
        }
        HighlightInputsForError($("#txtAutoAppLimitAdd"), false);
        if (!is_pay(document.getElementById('txtAutoAppLimitAdd').value)) {
            HighlightInputsForError($("#txtAutoAppLimitAdd"), true);
            ShowRemoveValidationMessage("Please Enter Numbers Only  - 2 Decimal Places ", "Warning");
            return false;
        }
        else {
            return true;
        }
    }







}
function checkValidationShopLimitEdit() {
    var isError = false;
    HighlightInputsForError($("#txtSuspendLimitUpdate"), false)
    if (document.getElementById('txtSuspendLimitUpdate').value == "") {
        isError = true;
        HighlightInputsForError($("#txtSuspendLimitUpdate"), isError)

    }

    HighlightInputsForError($("#txtShopMaterialLimitUpdate"), false)
    if (document.getElementById('txtShopMaterialLimitUpdate').value == "") {
        isError = true;
        HighlightInputsForError($("#txtShopMaterialLimitUpdate"), isError)


    }

    HighlightInputsForError($("#txtAutoApprovalLimitUpdate"), false)
    if (document.getElementById('txtAutoApprovalLimitUpdate').value == "") {
        isError = true;
        HighlightInputsForError($("#txtAutoApprovalLimitUpdate"), isError)

    }
    if (isError == true) {
        var errMsg = "";
        errMsg += "Please fill the mandatory fields(*).";
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        HighlightInputsForError($("#txtSuspendLimitUpdate"), false)
        if (!is_pay(document.getElementById('txtSuspendLimitUpdate').value)) {
            HighlightInputsForError($("#txtSuspendLimitUpdate"), true);
            ShowRemoveValidationMessage("Please Enter Numbers Only  - 2 Decimal Places ", "Warning");
            return false;
        }
        HighlightInputsForError($("#txtShopMaterialLimitUpdate"), false)
        if (!is_pay(document.getElementById('txtShopMaterialLimitUpdate').value)) {
            HighlightInputsForError($("#txtShopMaterialLimitUpdate"), true);
            ShowRemoveValidationMessage("Please Enter Numbers Only  - 2 Decimal Places ", "Warning");
            return false;
        }
        HighlightInputsForError($("#txtAutoApprovalLimitUpdate"), false)
        if (!is_pay(document.getElementById('txtAutoApprovalLimitUpdate').value)) {
            HighlightInputsForError($("#txtAutoApprovalLimitUpdate"), true);
            ShowRemoveValidationMessage("Please Enter Numbers Only  - 2 Decimal Places ", "Warning");
            return false;
        }
        else {
            return true;
        }

    }


}


function isInt(elm) {
    var elmstr = elm.toString();
    for (var i = 0; i < elmstr.length; i++) {
        if (elmstr.charAt(i) < "0" || elmstr.charAt(i) > "9") {
            if (elmstr.charAt(i) == ".") continue;
            else
                return false;
        }
    }
    return true;
}
function trimString(str) {
    str = this != window ? this : str;
    return str.replace(/^\s+/g, '').replace(/\s+$/g, '');
}
function is_pay(string) {
    var check_it = (string);

    if (check_it == "") return true;

    if (check_it.search(/^\d*$|^\d*\.\d{2}$/) != -1) {
        return true;
    }
    else {
        return false;
    }
}

// End ShopLimit by Afroz

// Shop Profile by Afroz

$(function () {

    $("#btnAddNewShop").click(function () {
        $("#ErrorMsgContainer").html("");
        $('#lblByPass').text('Bypass Leased Container Validations');
        ClearHighlightErrorForInputs($("#tblShop"), false);
        ClearHighlightErrorForInputs($("#tbAddUpdate"), false);
        HighlightInputsForError($("#drpShopType"), false);
        HighlightInputsForError($("#drpVendor"), false);
        HighlightInputsForError($("#drpCurrency"), false);
        $("#ErrorMsgContainer").html("");
        $('#tbAddUpdate').css("display", "block");
        $('#btnSubmitSP').css("display", "block");
        $('#btnUpdateSP').css("display", "none");
        $('#btnAuditTrailSP').css("display", "none");
        $('#lblMessage').text("Shop Add");
        $("#txtShopCode").val('');
        document.getElementById("txtShopCode").disabled = false;
        $("#drpShopActive").val('N');
        $("#txtShopDesc").val('');
        $("#drpShopType").val(-1);
        $("#drpVendor").val(-1);
        $("#txtGeoLoc").val('');
        $("#txtRKRPLoc").val('');
        $("#txtPart1").val('');
        $("#txtLabor1").val('');
        $("#drpCurrency").val(-1);
        $("#txtPart2").val('');
        $("#txtLabor2").val('');
        $("#txtEmailAddress").val('');
        $("#txtImportTax").val('');
        $("#txtPhone").val('');
        $("#txtDiscount").val(100);
        $("#drpLinkAccount").val('N');
        $("#drpAcep").val(-1);
        $("#txtRRIS70SuffixCode").val('');
        $("#drpOTSuspend").val(-1);
        $("#txtChangeUserName").val('N/A');
        $("#drpPrepTime").val(-1);
        $("#txtChangeTime").val('N/A');
        $("#txtDecentralized").val('N');
        $("#drpBypassLeasedContainerValidations").val(-1);
        $("#drpAutoComplete").val(-1);
    });

    $("#btnQuery").click(function () {
        $("#ErrorMsgContainer").html("");
        $('#lblByPass').text('Bypass All Validations Except CPH Limits');
        ClearHighlightErrorForInputs($("#tblShop"), false);
        ClearHighlightErrorForInputs($("#tbAddUpdate"), false);
        var url = 'fillRSByShop';
        $('#tbAddUpdate').css("display", "block");
        $('#lblMessage').text("Shop Edit");
        $.post(url, { ShopCode: $("#drpShop").val() }, function (data) {
            if (data[0] != null || data[0].count() > 0) {
                if (data[0].ShopCode != null) {
                    $("#txtShopCode").val(data[0].ShopCode);
                    document.getElementById("txtShopCode").disabled = true;
                }

                if (data[0].ShopActiveSW != null) {
                    $("#drpShopActive").val(data[0].ShopActiveSW);
                    if (data[0].ShopActiveSW == 'N') {
                        document.getElementById("MyCheck1").value = "checked1";
                    }
                    else {
                        document.getElementById("MyCheck1").value = "";
                    }
                    //alert(document.getElementById("MyCheck1").value);
                }
                if (data[0].ShopDescription != null) {
                    $("#txtShopDesc").val(data[0].ShopDescription);
                }

                if (data[0].ShopTypeCode != null) {
                    $("#drpShopType").val(data[0].ShopTypeCode);
                    if (data[0].ShopTypeCode == 3) {
                        document.getElementById('lblDiscount1').style.display = "none"
                        document.getElementById('txtDiscount').style.display = "none"
                        document.getElementById('lblDiscount3').style.display = "none"
                        document.getElementById('btnDiscount').style.display = "block"
                    }
                    else {
                        document.getElementById('lblDiscount1').style.display = "block"
                        document.getElementById('txtDiscount').style.display = "block"
                        document.getElementById('lblDiscount3').style.display = "block"
                        document.getElementById('btnDiscount').style.display = "none"
                    }
                }


                if (data[0].LocationCode != null) {
                    $("#txtGeoLoc").val(data[0].LocationCode);
                }
                if (data[0].RKRPloc != null) {
                    $("#txtRKRPLoc").val(data[0].RKRPloc);
                }
                if (data[0].SalesTaxPartCont != null) {
                    $("#txtPart1").val(data[0].SalesTaxPartCont);
                }
                if (data[0].SalesTaxLaborCon != null) {
                    $("#txtLabor1").val(data[0].SalesTaxLaborCon);
                }
                if (data[0].CUCDN != null) {
                    $("#drpCurrency").val(data[0].CUCDN);
                }

                if (data[0].VendorCode != null) {
                    $("#drpVendor").val(data[0].VendorCode);
                }
                if (data[0].SalesTaxPartGen != null) {
                    $("#txtPart2").val(data[0].SalesTaxPartGen);
                }
                if (data[0].SalesTaxLaborGen != null) {
                    $("#txtLabor2").val(data[0].SalesTaxLaborGen);
                }
                if (data[0].EmailAdress != null) {
                    $("#txtEmailAddress").val(data[0].EmailAdress);
                }
                if (data[0].ImportTax != null) {
                    $("#txtImportTax").val(data[0].ImportTax);
                }
                if (data[0].Phone != null) {
                    $("#txtPhone").val(data[0].Phone);
                }
                if (data[0].PCTMaterialFactor != null) {
                    $("#txtDiscount").val(data[0].PCTMaterialFactor);
                }
                if (data[0].RRISXmitSW != null) {
                    $("#drpLinkAccount").val(data[0].RRISXmitSW);
                }
                if (data[0].AcepSW != null) {
                    $("#drpAcep").val(data[0].AcepSW);
                }
                if (data[0].RRIS70SuffixCode != null) {
                    $("#txtRRIS70SuffixCode").val(data[0].RRIS70SuffixCode);
                }
                if (data[0].OvertimeSuspSW != null) {
                    $("#drpOTSuspend").val(data[0].OvertimeSuspSW);
                }
                if (data[0].ChangeUser != null) {
                    $("#txtChangeUserName").val(data[0].ChangeUser);
                }
                if (data[0].PreptimeSW != null) {
                    $("#drpPrepTime").val(data[0].PreptimeSW);
                }
                if (data[0].ChangeTime != null) {
                    $("#txtChangeTime").val(formatJSONDate(data[0].ChangeTime));
                }
                if (data[0].Decentralized != null) {
                    $("#txtDecentralized").val(data[0].Decentralized);
                }
                if (data[0].BypassLeaseRules != null || data[0].BypassLeaseRules != '') {

                    $("#drpBypassLeasedContainerValidations").val(data[0].BypassLeaseRules);


                }


                else {

                    $("#drpBypassLeasedContainerValidations").text('N');


                }

                if (data[0].AutoCompleteSW != null || data[0].AutoCompleteSW != '') {

                    $("#drpAutoComplete").val(data[0].AutoCompleteSW);
                }

                else {

                    $("#drpAutoComplete").text('N');

                }
                $('#btnSubmitSP').css("display", "none");
                $('#btnUpdateSP').css("display", "block");
                $('#btnAuditTrailSP').css("display", "block");
            }
            else {
                HighlightInputsForError($("#drpShop"), true);
                ShowRemoveValidationMessage("Please Select a Mode to UpdatePlease Confirm Shop Code to Edit matches Selected Shop in List : " + $("#drpShop").val(), "Warning");
            }


        });
    });

    $("#btnDisSubmit").click(function () {
        $("#ErrorMsgContainer").html("");
        ClearHighlightErrorForInputs($("#tblShop"), false);
        if (getManufacturer() && getDiscount() && checkDupManufactur() && getPCT()) {
            var url = 'InsertUpdateShopDiscount';
            $.post(url,
                {
                    ShopCode: $("#txtShopCode").val(),
                    DiscountAll: $("#txtPCTMaterialFactor").val(),
                    ManufactCode: document.forms[0].ManfString.value,
                    ManufactDis: document.forms[0].DiscString.value

                },
                function (data) {
                    if (data == "Success") {
                        ShowRemoveValidationMessage("Shop " + $("#txtShopCode").val() + " Updated.", "Success");
                        //window.location.href = "ShopProfiles"
                    }
                    else {
                        ShowRemoveValidationMessage(data, "Warning");
                    }

                });
        }
    });

});
function getManufacturer() {
    var strManf1;
    var strDisc1;
    strManf1 = "";
    strDisc1 = "";
    var errMsg = "";
    var isError = false;
    for (i = 0; i < document.forms[0].elements.length; i++) {
        // alert(document.ChangeTable.elements[i].type);
        if (document.forms[0].elements[i].type == "select-one") {
            // alert("inside 1st if");
            if (document.forms[0].elements[i].selectedIndex > 0)
                //  if(document.ChangeTable.elements[i].selected)
            {

                if (document.forms[0].elements[i + 1].value.length > 0) {
                    strManf1 = strManf1 + document.forms[0].elements[i].options[document.forms[0].elements[i].selectedIndex].text + ",";
                }
                else {
                    HighlightInputsForError(document.forms[0].elements[i + 1], true);
                    ShowRemoveValidationMessage("Discount field can not be empty", "Warning");
                    return false;
                }
            }

        }

    }
    document.forms[0].ManfString.value = strManf1;
    return true;
}
function getDiscount() {
    //	var strManf1;
    var strDisc1;
    //	strManf1="";
    strDisc1 = "";

    for (i = 3; i < document.forms[0].elements.length - 2; i = i + 1) {

        //   alert(document.ChangeTable.elements[i].type);
        if (document.forms[0].elements[i].type == "text") {
            // alert("inside 1st if");
            if (document.forms[0].elements[i].value.length > 0) {
                if (document.forms[0].elements[i - 1].selectedIndex > 0) {
                    if (IsNumeric(document.forms[0].elements[i].value))
                        //  if(document.ChangeTable.elements[i].selected)
                    {
                        //   alert("**");
                        strDisc1 = strDisc1 + document.forms[0].elements[i].value + ",";
                    }
                    else {
                        HighlightInputsForError(document.forms[0].elements[i], true);
                        ShowRemoveValidationMessage("Discount field must be numeric", "Warning");
                        return false;
                    }
                }
                else {
                    HighlightInputsForError(document.forms[0].elements[i - 1], true);
                    ShowRemoveValidationMessage("Select the corresponding manufacturer", "Warning");
                    return false;

                }
            }



        }


    }

    document.forms[0].DiscString.value = strDisc1;

    return true;
}
function checkDupManufactur() {

    var strManf1;
    var strManf2;
    strManf1 = "";
    strManf2 = "";
    for (i = 2; i < document.forms[0].elements.length - 2; i = i + 2) {
        if (document.forms[0].elements[i].selectedIndex > 0) {
            for (j = i + 2; j < document.forms[0].elements.length - 2; j = j + 2) {
                if (document.forms[0].elements[j].selectedIndex > 0) {
                    strManf1 = document.forms[0].elements[i].options[document.forms[0].elements[i].selectedIndex].text;
                    strManf2 = document.forms[0].elements[j].options[document.forms[0].elements[j].selectedIndex].text;
                    //alert(strManf1);
                    //	alert(strManf2);
                    if (strManf1 == strManf2) {
                        HighlightInputsForError(document.forms[0].elements[j], true);
                        ShowRemoveValidationMessage("duplicate record of manufacturer", "Warning");
                        return false;
                    }
                }
            }
        }
    }
    return true;
}
function getPCT() {
    var strPCT;
    strPCT = document.forms[0].elements[1].value;
    if (trimString(strPCT) == "") {
        HighlightInputsForError(document.forms[0].elements[1], true);
        ShowRemoveValidationMessage("duplicate record of manufacturerPlease Enter Discount/Markup -  100 is Default to indicate No Discount or Markup on Parts", "Warning");
        return false;
    }
    if (isInt(strPCT)) {
        return true;
    }
    else {
        HighlightInputsForError(document.forms[0].elements[1], true);
        ShowRemoveValidationMessage("Please Enter Whole Numbers Only", "Warning");
        return false;
    }
}
// Isit a nemeric ?
function IsNumeric(String)
    //  check for valid numeric strings	
{
    var strValidChars = ".0123456789";
    var strChar;
    var blnResult = true;
    var count;
    count = 0;
    //	alert("1");
    var strString = String.toString();
    if (strString.length == 0) return false;
    //  alert(strString.length);
    //	alert("2");
    //  test strString consists of valid characters listed above
    for (j = 0; j < strString.length && blnResult == true; j++) {
        //	alert("3");
        strChar = strString.charAt(j);
        if (strValidChars.indexOf(strChar) == -1) {
            blnResult = false;
            //alert ("Invalid discount");
            //	alert("false 1");
        }
        if (strChar == '.') {
            //	alert("Inside dot");
            count = count + 1;
            if (count == 2) {
                blnResult = false;
                //	alert("false 2");
                //	alert ("Invalid discount");
            }
        }
        //	if(strChar=='3')
        //		alert("It is 3");

        //	alert(blnResult);

    }
    //	alert(blnResult);
    return blnResult;

}
// Is it a float?
function isFloat(elm) {
    var elmstr = elm.toString();
    for (var i = 0; i < elmstr.length; i++) {
        if (elmstr.charAt(i) < "0" || elmstr.charAt(i) > "9") {
            if (elmstr.charAt(i) == ".") continue;
            else
                return false;
        }
    }
    return true;
}
// check decimal places to be none, 1, 2, 3,
function is_dec(string) {
    var check_it = trimString(string);

    if (check_it == "") return true;

    if (check_it.search(/^\d*$|^\d*\.\d{0}$/) != -1) {
        return true;
    }
    else if (check_it.search(/^\d*$|^\d*\.\d{1}$/) != -1) {
        return true;
    }
    else if (check_it.search(/^\d*$|^\d*\.\d{2}$/) != -1) {
        return true;
    }
    else if (check_it.search(/^\d*$|^\d*\.\d{3}$/) != -1) {
        return true;
    }
    else {
        return false;
    }
}
function doSubmit(strType) {

    $("#ErrorMsgContainer").html("");
    var v1 = "";
    var RKRP = "";
    ClearHighlightErrorForInputs($("#tbAddUpdate"), false);
    if (strType == 'Insert') {

        if (InsertValidationShopProfile()) {

            v1 = $("#txtRKRPLoc").val();
            if (v1.trim() == "") {
                RKRP = ($("#txtGeoLoc").val().substr($("#txtGeoLoc").val().length - 3))
            }
            else {
                RKRP = $("#txtRKRPLoc").val();
            }
            var url = 'InsertShopProfile';
            $.post(url,
                {
                    ShopCode: $("#txtShopCode").val(),
                    ShopActive: $("#drpShopActive").val(),
                    ShopDesc: $("#txtShopDesc").val(),
                    ShopType: $("#drpShopType").val(),
                    VendorCode: $("#drpVendor").val(),
                    GeoLoc: $("#txtGeoLoc").val(),
                    RKRPLoc: RKRP,
                    Part1: $("#txtPart1").val(),
                    Labor1: $("#txtLabor1").val(),
                    CurrencyCode: $("#drpCurrency").val(),
                    Part2: $("#txtPart2").val(),
                    Labor2: $("#txtLabor2").val(),
                    EmailAddress: $("#txtEmailAddress").val(),
                    ImportTax: $("#txtImportTax").val(),
                    Phone: $("#txtPhone").val(),
                    Discount: $("#txtDiscount").val(),
                    LinkAccount: $("#drpLinkAccount").val(),
                    Acep: $("#drpAcep").val(),
                    RRIS70SuffixCode: $("#txtRRIS70SuffixCode").val(),
                    OTSuspend: $("#drpOTSuspend").val(),
                    ChangeUserName: $("#txtChangeUserName").val(),
                    PrepTime: $("#drpPrepTime").val(),
                    ChangeTime: $("#txtChangeTime").val(),
                    Decentralized: $("#txtDecentralized").val(),
                    BypassLeasedContainerValidations: $("#drpBypassLeasedContainerValidations").val(),
                    AutoComplete: $("#drpAutoComplete").val()
                },
                function (data) {
                    if (data == 'Success') {
                        ShowRemoveValidationMessage("Record added successfully.", "Success");

                    }
                    else {
                        ShowRemoveValidationMessage(data, "Warning");
                    }
                });

        }

    }
    if (strType == 'Update') {

        if (UpdateValidationShopProfile()) {


            var url = 'UpdateShopProfile';
            $.post(url,
                {

                    ShopCode: $("#txtShopCode").val(),
                    ShopActive: $("#drpShopActive").val(),
                    ShopDesc: $("#txtShopDesc").val(),
                    ShopType: $("#drpShopType").val(),
                    VendorCode: $("#drpVendor").val(),
                    GeoLoc: $("#txtGeoLoc").val(),
                    RKRPLoc: $("#txtRKRPLoc").val(),
                    Part1: $("#txtPart1").val(),
                    Labor1: $("#txtLabor1").val(),
                    CurrencyCode: $("#drpCurrency").val(),
                    Part2: $("#txtPart2").val(),
                    Labor2: $("#txtLabor2").val(),
                    EmailAddress: $("#txtEmailAddress").val(),
                    ImportTax: $("#txtImportTax").val(),
                    Phone: $("#txtPhone").val(),
                    Discount: $("#txtDiscount").val(),
                    LinkAccount: $("#drpLinkAccount").val(),
                    Acep: $("#drpAcep").val(),
                    RRIS70SuffixCode: $("#txtRRIS70SuffixCode").val(),
                    OTSuspend: $("#drpOTSuspend").val(),
                    PrepTime: $("#drpPrepTime").val(),
                    Decentralized: $("#txtDecentralized").val(),
                    BypassLeasedContainerValidations: $("#drpBypassLeasedContainerValidations").val(),
                    AutoComplete: $("#drpAutoComplete").val()
                },
                function (data) {
                    if (data == "Success") {
                        ShowRemoveValidationMessage("Record updated successfully.", "Success");
                        var url = 'fillRSByShop';
                        $.post(url, { ShopCode: $("#drpShop").val() }, function (data) {

                            if (data[0] != null || data[0].count() > 0) {
                                if (data[0].ShopCode != null) {
                                    $("#txtShopCode").val(data[0].ShopCode);
                                    document.getElementById("txtShopCode").disabled = true;
                                }

                                if (data[0].ShopActiveSW != null) {
                                    $("#drpShopActive").val(data[0].ShopActiveSW);
                                    if (data[0].ShopActiveSW == 'N') {
                                        document.getElementById("MyCheck1").value = "checked1";
                                    }
                                    else {
                                        document.getElementById("MyCheck1").value = "";
                                    }
                                    //alert(document.getElementById("MyCheck1").value);
                                }
                                if (data[0].ShopDescription != null) {
                                    $("#txtShopDesc").val(data[0].ShopDescription);
                                }

                                if (data[0].ShopTypeCode != null) {
                                    $("#drpShopType").val(data[0].ShopTypeCode);
                                    if (data[0].ShopTypeCode == 3) {
                                        document.getElementById('lblDiscount1').style.display = "none"
                                        document.getElementById('txtDiscount').style.display = "none"
                                        document.getElementById('lblDiscount3').style.display = "none"
                                        document.getElementById('btnDiscount').style.display = "block"
                                    }
                                    else {
                                        document.getElementById('lblDiscount1').style.display = "block"
                                        document.getElementById('txtDiscount').style.display = "block"
                                        document.getElementById('lblDiscount3').style.display = "block"
                                        document.getElementById('btnDiscount').style.display = "none"
                                    }
                                }


                                if (data[0].LocationCode != null) {
                                    $("#txtGeoLoc").val(data[0].LocationCode);
                                }
                                if (data[0].RKRPloc != null) {
                                    $("#txtRKRPLoc").val(data[0].RKRPloc);
                                }
                                if (data[0].SalesTaxPartCont != null) {
                                    $("#txtPart1").val(data[0].SalesTaxPartCont);
                                }
                                if (data[0].SalesTaxLaborCon != null) {
                                    $("#txtLabor1").val(data[0].SalesTaxLaborCon);
                                }
                                if (data[0].CUCDN != null) {
                                    $("#drpCurrency").val(data[0].CUCDN);
                                }

                                if (data[0].VendorCode != null) {
                                    $("#drpVendor").val(data[0].VendorCode);
                                }
                                if (data[0].SalesTaxPartGen != null) {
                                    $("#txtPart2").val(data[0].SalesTaxPartGen);
                                }
                                if (data[0].SalesTaxLaborGen != null) {
                                    $("#txtLabor2").val(data[0].SalesTaxLaborGen);
                                }
                                if (data[0].EmailAdress != null) {
                                    $("#txtEmailAddress").val(data[0].EmailAdress);
                                }
                                if (data[0].ImportTax != null) {
                                    $("#txtImportTax").val(data[0].ImportTax);
                                }
                                if (data[0].Phone != null) {
                                    $("#txtPhone").val(data[0].Phone);
                                }
                                if (data[0].PCTMaterialFactor != null) {
                                    $("#txtDiscount").val(data[0].PCTMaterialFactor);
                                }
                                if (data[0].RRISXmitSW != null) {
                                    $("#drpLinkAccount").val(data[0].RRISXmitSW);
                                }
                                if (data[0].AcepSW != null) {
                                    $("#drpAcep").val(data[0].AcepSW);
                                }
                                if (data[0].RRIS70SuffixCode != null) {
                                    $("#txtRRIS70SuffixCode").val(data[0].RRIS70SuffixCode);
                                }
                                if (data[0].OvertimeSuspSW != null) {
                                    $("#drpOTSuspend").val(data[0].OvertimeSuspSW);
                                }
                                if (data[0].ChangeUser != null) {
                                    $("#txtChangeUserName").val(data[0].ChangeUser);
                                }
                                if (data[0].PreptimeSW != null) {
                                    $("#drpPrepTime").val(data[0].PreptimeSW);
                                }
                                if (data[0].ChangeTime != null) {
                                    $("#txtChangeTime").val(formatJSONDate(data[0].ChangeTime));
                                }
                                if (data[0].Decentralized != null) {
                                    $("#txtDecentralized").val(data[0].Decentralized);
                                }
                                if (data[0].BypassLeaseRules != null || data[0].BypassLeaseRules != '') {
                                    $("#drpBypassLeasedContainerValidations").val(data[0].BypassLeaseRules);

                                }

                                else {
                                    $("#drpBypassLeasedContainerValidations").text('N');

                                }
                                if (data[0].AutoCompleteSW != null || data[0].AutoCompleteSW != '') {

                                }
                                else {
                                    $("#drpAutoComplete").text('N');
                                }
                            }
                        });
                    }

                    else {
                        ShowRemoveValidationMessage(data, "Warning");
                    }
                });

        }
    }
    else if (strType == 'Redirect') {
        if (document.getElementById("MyCheck").value == "checked") {
            if (document.getElementById("MyCheck1").value == "checked1") {
                ShowRemoveValidationMessage("Submit changes before going to the next page", "Warning");
                return false;
            }
        }

        var ShopCode = $("#txtShopCode").val();
        window.location.href = 'Discount?ShopCD=' + ShopCode;


    }
    else if (strType == 'Redirect1') {
        if (document.getElementById("MyCheck").value == "checked") {
            if (document.getElementById("MyCheck1").value == "checked1") {
                ShowRemoveValidationMessage("Submit changes before going to the next page", "Warning");
                return false;
            }
        }

        var ShopCode = $("#txtShopCode").val();
        window.location.href = 'Discount_View?ShopCD=' + ShopCode;


    }


}
function InsertValidationShopProfile() {
    var IsError = false;
    HighlightInputsForError($("#txtShopCode"), false);
    if (document.getElementById('txtShopCode').value == '') {
        IsError = true;
        HighlightInputsForError($("#txtShopCode"), IsError);

    }
    HighlightInputsForError($("#drpShopActive"), false);
    if (document.getElementById('drpShopActive').value == "") {
        IsError = true;
        HighlightInputsForError($("#drpShopActive"), IsError);

    }
    HighlightInputsForError($("#txtShopDesc"), false);
    if (document.getElementById('txtShopDesc').value == "") {
        IsError = true;
        HighlightInputsForError($("#txtShopDesc"), IsError);

    }
    HighlightInputsForError($("#drpShopType"), false);
    if (document.getElementById('drpShopType').value == '-1') {
        IsError = true;
        HighlightInputsForError($("#drpShopType"), IsError);
    }

    HighlightInputsForError($("#drpVendor"), false);
    if (document.getElementById('drpVendor').value == '') {
        IsError = true;
        HighlightInputsForError($("#drpVendor"), IsError);

    }
    HighlightInputsForError($("#txtGeoLoc"), false);
    if (document.getElementById('txtGeoLoc').value == '') {
        IsError = true;
        HighlightInputsForError($("#txtGeoLoc"), IsError);

    }

    //HighlightInputsForError($("#txtRKRPLoc"), false);
    //if (document.getElementById('txtRKRPLoc').value == '') {
    //    IsError = true;
    //    HighlightInputsForError($("#txtRKRPLoc"), IsError);
    //}

    HighlightInputsForError($("#drpCurrency"), false);
    if (document.getElementById('drpCurrency').value == '') {
        IsError = true
        HighlightInputsForError($("#drpCurrency"), IsError);
    }
    if (IsError == true) {
        var errMsg = "";
        errMsg += "Please fill all highlighted mandatory fields...";
        ShowRemoveValidationMessage(errMsg, "Warning");
        return false;

    }
    else {
        HighlightInputsForError($("#txtShopDesc"), false);
        if (document.getElementById('txtShopDesc').value != '') {
            var strDesc = document.getElementById('txtShopDesc').value;
            if (strDesc.indexOf('"') > -1) {
                HighlightInputsForError($("#txtShopDesc"), true);
                ShowRemoveValidationMessage("Please Remove Double Quote Character in Shop Name", "Warning");
                return false;
            }
        }
        var numString;
        HighlightInputsForError($("#txtPart1"), false);
        if (document.getElementById('txtPart1').value != '') {
            numString = document.getElementById('txtPart1').value;
            if (!isFloat(numString)) {
                HighlightInputsForError($("#txtPart1"), true);
                ShowRemoveValidationMessage("Please Enter Numbers Only - Decimal OK ", "Warning");
                return false;
            }
            else if (parseInt(numString) > parseInt("32767")) {
                HighlightInputsForError($("#txtPart1"), true);
                ShowRemoveValidationMessage("Number cannot be larger than 32,767 ", "Warning");
                return false;
            }
            if (!is_dec(numString)) {
                HighlightInputsForError($("#txtPart1"), true);
                ShowRemoveValidationMessage("Please Enter Max 3 decimal positions ", "Warning");
                return false;
            }

        }
        HighlightInputsForError($("#txtPart2"), false);
        if (document.getElementById('txtPart2').value != '') {
            numString = document.getElementById('txtPart2').value;
            if (!isFloat(numString)) {
                HighlightInputsForError($("#txtPart2"), true);
                ShowRemoveValidationMessage("Please Enter Numbers Only - Decimal OK ", "Warning");
                return false;
            }
            else if (parseInt(numString) > parseInt("32767")) {
                HighlightInputsForError($("#txtPart2"), true);
                ShowRemoveValidationMessage("Number cannot be larger than 32,767  ", "Warning");
                return false;
            }
            if (!is_dec(numString)) {
                HighlightInputsForError($("#txtPart2"), true);
                ShowRemoveValidationMessage("Please Enter Max 3 decimal positions", "Warning");
                return false;
            }

        }
        HighlightInputsForError($("#txtLabor1"), false);
        if (document.getElementById('txtLabor1').value != '') {
            numString = document.getElementById('txtLabor1').value;
            if (!isFloat(numString)) {
                HighlightInputsForError($("#txtLabor1"), true);
                ShowRemoveValidationMessage("Please Enter Numbers Only - Decimal OK ", "Warning");
                return false;
            }
            else if (parseInt(numString) > parseInt("32767")) {
                HighlightInputsForError($("#txtLabor1"), true);
                ShowRemoveValidationMessage("Number cannot be larger than 32,767  ", "Warning");
                return false;
            }
            if (!is_dec(numString)) {
                HighlightInputsForError($("#txtLabor1"), true);
                ShowRemoveValidationMessage("Please Enter Max 3 decimal positions ", "Warning");
                return false;
            }

        }
        HighlightInputsForError($("#txtLabor2"), false);
        if (document.getElementById('txtLabor2').value != '') {
            numString = document.getElementById('txtLabor2').value;
            if (!isFloat(numString)) {
                HighlightInputsForError($("#txtLabor2"), true);
                ShowRemoveValidationMessage("Please Enter Numbers Only - Decimal OK ", "Warning");
                return false;
            }
            else if (parseInt(numString) > parseInt("32767")) {
                HighlightInputsForError($("#txtLabor2"), true);
                ShowRemoveValidationMessage("Number cannot be larger than 32,767  ", "Warning");
                return false;
            }
            if (!is_dec(numString)) {
                HighlightInputsForError($("#txtLabor2"), true);
                ShowRemoveValidationMessage("Please Enter Max 3 decimal positions ", "Warning");
                return false;
            }

        }
        HighlightInputsForError($("#txtImportTax"), false);
        if (document.getElementById('txtImportTax').value != '') {
            numString = document.getElementById('txtImportTax').value;
            if (!isFloat(numString)) {
                HighlightInputsForError($("#txtImportTax"), true);
                ShowRemoveValidationMessage("Please Enter Numbers Only - Decimal OK ", "Warning");
                return false;
            }
            else if (parseInt(numString) > parseInt("32767")) {
                HighlightInputsForError($("#txtImportTax"), true);
                ShowRemoveValidationMessage("Number cannot be larger than 32,767  ", "Warning");
                return false;
            }
            if (!is_dec(numString)) {
                HighlightInputsForError($("#txtImportTax"), true);
                ShowRemoveValidationMessage("Please Enter Max 3 decimal positions ", "Warning");
                return false;
            }


        }
        HighlightInputsForError($("#txtDiscount"), false);
        if (document.getElementById('txtDiscount').value != '') {
            numString = document.getElementById('txtDiscount').value;
            if (!isInt(numString)) {
                HighlightInputsForError($("#txtDiscount"), true);
                ShowRemoveValidationMessage("Please Enter Whole Numbers Only", "Warning");
                return false;
            }


        }

    }
    return true;
}
function UpdateValidationShopProfile() {

    var IsError = false;
    HighlightInputsForError($("#txtShopDesc"), false);
    if (document.getElementById('txtShopDesc').value == '') {
        IsError = true;
        HighlightInputsForError($("#txtShopDesc"), IsError);
    }
    HighlightInputsForError($("#drpShopType"), false);
    if (document.getElementById('drpShopType').value == '') {
        IsError = true;
        HighlightInputsForError($("#drpShopType"), IsError);
    }
    HighlightInputsForError($("#drpVendor"), false);
    if (document.getElementById('drpVendor').value == '') {
        IsError = true;
        HighlightInputsForError($("#drpVendor"), IsError);
    }
    HighlightInputsForError($("#txtGeoLoc"), false);
    if (document.getElementById('txtGeoLoc').value == '') {
        IsError = true;
        HighlightInputsForError($("#txtGeoLoc"), IsError);

    }
    HighlightInputsForError($("#txtRKRPLoc"), false);
    if (document.getElementById('txtRKRPLoc').value == '') {
        IsError = true;
        HighlightInputsForError($("#txtRKRPLoc"), IsError);
    }
    HighlightInputsForError($("#drpCurrency"), false);
    if (document.getElementById('drpCurrency').value == '') {
        IsError = true
        HighlightInputsForError($("#drpCurrency"), IsError);
    }
    HighlightInputsForError($("#drpLinkAccount"), false);
    if (document.getElementById('drpLinkAccount').value == '') {
        IsError = true;
        HighlightInputsForError($("#drpLinkAccount"), IsError);
    }

    HighlightInputsForError($("#txtDiscount"), false);
    if (document.getElementById('txtDiscount').value == '') {
        IsError = true;
        HighlightInputsForError($("#txtDiscount"), IsError);
    }
    HighlightInputsForError($("#drpAcep"), false);
    if (document.getElementById('drpAcep').value == '') {
        IsError = true;
        HighlightInputsForError($("#drpAcep"), IsError);
    }
    HighlightInputsForError($("#drpOTSuspend"), false);
    if (document.getElementById('drpOTSuspend').value == '') {
        IsError = true;
        HighlightInputsForError($("#drpOTSuspend"), IsError);
    }
    HighlightInputsForError($("#drpPrepTime"), false);
    if (document.getElementById('drpPrepTime').value == '') {
        IsError = true;
        HighlightInputsForError($("#drpPrepTime"), IsError);
    }
    if (IsError == true) {
        var errMsg = "";
        errMsg += "Please fill all highlighted mandatory fields.";
        ShowRemoveValidationMessage(errMsg, "Warning");
        return false;
    }
    else {
        HighlightInputsForError($("#txtShopDesc"), false);
        if (document.getElementById('txtShopDesc').value != '') {
            var strDesc = document.getElementById('txtShopDesc').value;
            if (strDesc.indexOf('"') > -1) {
                HighlightInputsForError($("#txtShopDesc"), true);
                ShowRemoveValidationMessage("Please Remove Double Quote Character in Shop Name", "Warning");
                return false;
            }
        }
        var numString;
        HighlightInputsForError($("#txtPart1"), false);
        if (document.getElementById('txtPart1').value != '') {
            numString = document.getElementById('txtPart1').value;
            if (!isFloat(numString)) {
                HighlightInputsForError($("#txtPart1"), true);
                ShowRemoveValidationMessage("Please Enter Numbers Only - Decimal OK ", "Warning");
                return false;
            }
            else if (parseInt(numString) > parseInt("32767")) {
                HighlightInputsForError($("#txtPart1"), true);
                ShowRemoveValidationMessage("Number cannot be larger than 32,767 ", "Warning");
                return false;
            }
            if (!is_dec(numString)) {
                HighlightInputsForError($("#txtPart1"), true);
                ShowRemoveValidationMessage("Please Enter Max 3 decimal positions ", "Warning");
                return false;
            }

        }
        HighlightInputsForError($("#txtPart2"), false);
        if (document.getElementById('txtPart2').value != '') {
            numString = document.getElementById('txtPart2').value;
            if (!isFloat(numString)) {
                HighlightInputsForError($("#txtPart2"), true);
                ShowRemoveValidationMessage("Please Enter Numbers Only - Decimal OK ", "Warning");
                return false;
            }
            else if (parseInt(numString) > parseInt("32767")) {
                HighlightInputsForError($("#txtPart2"), true);
                ShowRemoveValidationMessage("Number cannot be larger than 32,767  ", "Warning");
                return false;
            }
            if (!is_dec(numString)) {
                HighlightInputsForError($("#txtPart2"), true);
                ShowRemoveValidationMessage("Please Enter Max 3 decimal positions", "Warning");
                return false;
            }

        }
        HighlightInputsForError($("#txtLabor1"), false);
        if (document.getElementById('txtLabor1').value != '') {
            numString = document.getElementById('txtLabor1').value;
            if (!isFloat(numString)) {
                HighlightInputsForError($("#txtLabor1"), true);
                ShowRemoveValidationMessage("Please Enter Numbers Only - Decimal OK ", "Warning");
                return false;
            }
            else if (parseInt(numString) > parseInt("32767")) {
                HighlightInputsForError($("#txtLabor1"), true);
                ShowRemoveValidationMessage("Number cannot be larger than 32,767  ", "Warning");
                return false;
            }
            if (!is_dec(numString)) {
                HighlightInputsForError($("#txtLabor1"), true);
                ShowRemoveValidationMessage("Please Enter Max 3 decimal positions ", "Warning");
                return false;
            }

        }
        HighlightInputsForError($("#txtLabor2"), false);
        if (document.getElementById('txtLabor2').value != '') {
            numString = document.getElementById('txtLabor2').value;
            if (!isFloat(numString)) {
                HighlightInputsForError($("#txtLabor2"), true);
                ShowRemoveValidationMessage("Please Enter Numbers Only - Decimal OK ", "Warning");
                return false;
            }
            else if (parseInt(numString) > parseInt("32767")) {
                HighlightInputsForError($("#txtLabor2"), true);
                ShowRemoveValidationMessage("Number cannot be larger than 32,767  ", "Warning");
                return false;
            }
            if (!is_dec(numString)) {
                HighlightInputsForError($("#txtLabor2"), true);
                ShowRemoveValidationMessage("Please Enter Max 3 decimal positions ", "Warning");
                return false;
            }

        }
        HighlightInputsForError($("#txtImportTax"), false);
        if (document.getElementById('txtImportTax').value != '') {
            numString = document.getElementById('txtImportTax').value;
            if (!isFloat(numString)) {
                HighlightInputsForError($("#txtImportTax"), true);
                ShowRemoveValidationMessage("Please Enter Numbers Only - Decimal OK ", "Warning");
                return false;
            }
            else if (parseInt(numString) > parseInt("32767")) {
                HighlightInputsForError($("#txtImportTax"), true);
                ShowRemoveValidationMessage("Number cannot be larger than 32,767  ", "Warning");
                return false;
            }
            if (!is_dec(numString)) {
                HighlightInputsForError($("#txtImportTax"), true);
                ShowRemoveValidationMessage("Please Enter Max 3 decimal positions ", "Warning");
                return false;
            }


        }
        HighlightInputsForError($("#txtDiscount"), false);
        if (document.getElementById('txtDiscount').value != '') {
            numString = document.getElementById('txtDiscount').value;
            if (!isInt(numString)) {
                HighlightInputsForError($("#txtDiscount"), true);
                ShowRemoveValidationMessage("Please Enter Whole Numbers Only", "Warning");
                return false;
            }

        }
        HighlightInputsForError($("#drpAutoComplete"), false);
        if (document.getElementById('drpAutoComplete') && document.getElementById('drpBypassLeasedContainerValidations')) {
            if (document.getElementById('drpAutoComplete').options[document.getElementById('drpAutoComplete').selectedIndex].value == "Y"
            && document.getElementById('drpBypassLeasedContainerValidations').options[document.getElementById('drpBypassLeasedContainerValidations').selectedIndex].value != "Y") {
                HighlightInputsForError($("#drpAutoComplete"), true);
                ShowRemoveValidationMessage("Autocomplete can only be set to Y if switch 'By pass all validations, except shop and CPH limit' is set to Y. ", "Warning");
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return true;
        }
    }
    return true;
}
function doRedirect() {

    var sType;
    sType = document.getElementById("drpShopType").value;
    if (sType == 3) {

        document.getElementById("MyCheck").value = "checked";
        //alert(document.getElementById("MyCheck").value);
        document.getElementById("lblDiscount1").style.display = "none";
        document.getElementById("txtDiscount").style.display = "none";
        document.getElementById("lblDiscount3").style.display = "none";
        document.getElementById("btnDiscount").style.display = "";
    }
    else {
        document.getElementById("lblDiscount1").style.display = "block";
        document.getElementById("txtDiscount").style.display = "block";
        document.getElementById("lblDiscount3").style.display = "block";
        document.getElementById("btnDiscount").style.display = "none";
    }
}
// End Shop Profile by Afroz



// End Afroz



$("#btnAddCPH").click(function () {

    //e.preventDefault();
    $("#divCPHApproval").show();


    $('#LimitAmt1').css("display", "inline-block");
    $('#LimitAmt2').css("display", "inline-block");
    $('#LimitAmt3').css("display", "inline-block");
    $('#LimitAmt4').css("display", "inline-block");
    $('#LimitAmt5').css("display", "inline-block");
    $('#LimitAmt6').css("display", "inline-block");
    $('#LimitAmt7').css("display", "inline-block");

    $('#ChangeUserName1').css("display", "none");
    $('#ChangeUserName2').css("display", "none");
    $('#ChangeUserName3').css("display", "none");
    $('#ChangeUserName4').css("display", "none");
    $('#ChangeUserName5').css("display", "none");
    $('#ChangeUserName6').css("display", "none");
    $('#ChangeUserName7').css("display", "none");

    $('#lblChName').css("display", "none");
    $('#lblChTime').css("display", "none");

    $('#ChangeTime1').css("display", "none");
    $('#ChangeTime2').css("display", "none");
    $('#ChangeTime3').css("display", "none");
    $('#ChangeTime4').css("display", "none");
    $('#ChangeTime5').css("display", "none");
    $('#ChangeTime6').css("display", "none");
    $('#ChangeTime7').css("display", "none");

    $('#tblEdit').hide();
    $('#tblSubmit').show();

    $('#LimitAmt1').val("");
    $('#LimitAmt2').val("");
    $('#LimitAmt3').val("");
    $('#LimitAmt4').val("");
    $('#LimitAmt5').val("");
    $('#LimitAmt6').val("");
    $('#LimitAmt7').val("");


    $("#EqSize").val("");
    $("#ModeList").val("");

});

$("#btnQryCPH").click(function () {
    $("#divCPHApproval").show();

    $('#ChangeUserName1').css("display", "inline-block");
    $('#ChangeUserName2').css("display", "inline-block");
    $('#ChangeUserName3').css("display", "inline-block");
    $('#ChangeUserName4').css("display", "inline-block");
    $('#ChangeUserName5').css("display", "inline-block");
    $('#ChangeUserName6').css("display", "inline-block");
    $('#ChangeUserName7').css("display", "inline-block");

    $('#ChangeTime1').css("display", "inline-block");
    $('#ChangeTime2').css("display", "inline-block");
    $('#ChangeTime3').css("display", "inline-block");
    $('#ChangeTime4').css("display", "inline-block");
    $('#ChangeTime5').css("display", "inline-block");
    $('#ChangeTime6').css("display", "inline-block");
    $('#ChangeTime7').css("display", "inline-block");

    $('#lblChName').css("display", "inline-block");
    $('#lblChTime').css("display", "inline-block");

    var equipmentSize = $(drpEqSize).val();
    var modeList = $(drpModeList).val();


    // var a = JSON.stringify({ customerId: $(this).val() };
    $.ajax({
        url: "/ManageMasterData/ManageMasterData/GetAllDetailsForCPHApproval",
        type: 'POST',
        datatype: 'json',
        data: { Eq: equipmentSize, Mode: modeList },
        cache: false,
        success: function (data) {

            var equipmentSize = $(drpEqSize).val();
            var modeList = $(drpModeList).val();

            $("#EqSize").val(equipmentSize);
            $("#ModeList").val(modeList);

            $('#tblEdit').show();
            $('#tblSubmit').hide();

            var items = [];
            $.each(data, function (i, data) {

                if (i == 0) {

                    $("#LimitAmt1").val(data.LimitAmount);
                    $("#ChangeUserName1").val(data.ChangeUser);
                    $("#ChangeTime1").val(formatJSONDate(data.ChTime));

                }

                if (i == 1) {
                    $("#LimitAmt2").val(data.LimitAmount);
                    $("#ChangeUserName2").val(data.ChangeUser);
                    $("#ChangeTime2").val(formatJSONDate(data.ChTime));


                }

                if (i == 2) {
                    $("#LimitAmt3").val(data.LimitAmount);
                    $("#ChangeUserName3").val(data.ChangeUser);
                    $("#ChangeTime3").val(formatJSONDate(data.ChTime));
                }

                if (i == 3) {
                    $("#LimitAmt4").val(data.LimitAmount);
                    $("#ChangeUserName4").val(data.ChangeUser);
                    $("#ChangeTime4").val(formatJSONDate(data.ChTime));
                }

                if (i == 4) {
                    $("#LimitAmt5").val(data.LimitAmount);
                    $("#ChangeUserName5").val(data.ChangeUser);
                    $("#ChangeTime5").val(formatJSONDate(data.ChTime));
                }

                if (i == 5) {
                    $("#LimitAmt6").val(data.LimitAmount);
                    $("#ChangeUserName6").val(data.ChangeUser);
                    $("#ChangeTime6").val(formatJSONDate(data.ChTime));
                }

                if (i == 6) {
                    $("#LimitAmt7").val(data.LimitAmount);
                    $("#ChangeUserName7").val(data.ChangeUser);
                    $("#ChangeTime7").val(formatJSONDate(data.ChTime));
                }


            });



        },
        error: function (data) {

        }
    });
    //});
    // });


});

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

$(function () {
    $('#btnDamageQuery1').click(function () {
        HighlightInputsForError($("#txtDamageCode"), false);
        HighlightInputsForError($("#txtDamageName"), false);
        HighlightInputsForError($("#txtDamageDescription"), false);
        HighlightInputsForError($("#txtNumericalCode"), false)
        $("#ErrorMsgContainer").html("");
        $('#divDamageCode').show();
        var c = $("#drpDamageCode").val();
        $.ajax({

            url: "/ManageMasterData/ManageMasterData/GetAllDetailsForDamageCode",
            type: 'POST',
            data: { id: c },

            //data: JSON.stringify({ CountryCode: c, locale: locale }),
            cache: false,
            success: function (data) {

                $("#txtDamageCode").val(data.DamageCedexCode);
                $("#txtDamageName").val(data.DamageName);
                $("#txtDamageDescription").val(data.DamageDescription);
                $("#txtNumericalCode").val(data.DamageNumericalCode);
                $("#txtChangeUser").val(data.ChangeUser);
                var d = new Date();
                d.setTime(parseInt(data.ChangeTime.substring(6)));
                $("#txtChangeTime").val(FormatDate(d));
                $("#txtChangeUser").attr("readonly", true);
                $("#txtChangeTime").attr("readonly", true);
            },
            error: function (data) {
            }
        });
    });
});

function FormatDate(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var seconds = date.getSeconds();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    var month = date.getMonth();
    month++;
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;
    month = month < 10 ? '0' + month : month;
    var daynumbers = date.getDate();
    daynumbers = daynumbers < 10 ? '0' + daynumbers : daynumbers;
    var strTime = hours + ':' + minutes + ':' + seconds + ' ' + ampm;
    //return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear() + "  " + strTime;
    return date.getFullYear() + "-" + month + "-" + daynumbers + " " + strTime;
}