/// <reference path="../../../Scripts/mainjs.js" />
$wo_global_var = {};
var callAgain = true;
var SPLITBYPIPE = "|*|";
var REPAIRSPARE = { REPAIR: 1, SPARE: 2 };
var DLGTYPE = { DAMAGE: 1, REPAIR: 2, REPAIRLOCCODE: 3, TPI: 4 };
var WOMSG = {
    MODE_MISSING: 'Please select Mode.',
    REPCODERP_MISSING: 'Please select some Repair Code to fetch the hour(s) from Get Hours.',
    EQP_MISSING: 'Please insert valid Equipment Number.',
    VENDOR_MISSING: 'Please insert valid Vendor Reference Number.',
    RKEM_NOTCALLED: 'Container type not received from RKEM or RKEM call not performed.',
    WAITINFO: 'Validating Work Order details.Please wait...',
    SHOP_MISSING: 'Please select Shop.',
    CUSTOMER_MISSING: 'Please select Customer.',
    CAUSE_MISSING: 'Please select Cause.',
    EQP_NOTUNIQUE: 'Please insert unique Equipment Number.',
    VENDOR_NOTUNIQUE: 'Please insert unique Vendor Reference Number.',
    REPCODERP_NOTUNIQUE: 'Please select unique combination of Repair Code and Repair Location Code.',
    REPCODESP_NOTUNIQUE: 'Please select unique Repair Code for Spare part.',
    OWNERSPNSP_NOTUNIQUE: 'Please select unique combination of Owner Supplied Parts Number and Repair Code for Spare part.',
    REPAIRPART_INCOMPLETEENTRY: 'At least one Repair(s) entry must be completely filled for this WO.',
    REPAIRPART_COMPLETEHIGHLIGHTEDENTRY: 'Please fill in all fields for each Repair(s) entry.',
    REPAIRPART_PIECEMISSING: 'Repair piece count is required for repair code.',
    SPAREPART_EXTRAREPCODE: 'All Repair Code in Spare part(s) list must have reference in Repair Code of Repair List.',
    REPAIRPART_EXCEEDMAXQUANT: 'Maximum pieces allowed for selected Repair Code is ',
    THIRDPARTY_MISSING: 'Please enter Third Party Port number ',
    CURRENCY_MISSING: 'Currency not found ',
    COMPLETIONDATE_MISSING: 'Please select Completion date for this Work Order ',
    REMARKS_MISSING: 'Please enter remarks for this Work Order ',
    SALESTAXPC_MISSING: 'Please provide value for Sales tax Part(s) cost for this Work Order ',
    SALESTAXLC_MISSING: 'Please provide value for Sales tax Labour cost for this Work Order ',
    IMPORTTAXCOST_MISSING: 'Please provide value of Import tax cost for this Work Order ',
    NETWORK_PROBLEM: 'Unable to perform operation due to some error, please try in a while ',
    HOURNOLESS: 'Ordinary hours field can not be less than 0 '
}

//REPAIRPART_COMPLETEHIGHLIGHTEDENTRY: 'Please fill in the highlighted fields for Repair(s) entry.',

$(function () {

    $("#UICause").change(function () {
        //if Cause selected item not in these two then enable the 
        //"1 - Wear and Tear, 2 - 1Handling Damage"
        if (parseInt($(this).val()) > 2) {
            $("#ThirdPartyPort").removeAttr("readonly");
        }
        else {
            $("#ThirdPartyPort").removeAttr("readonly");
            $("#ThirdPartyPort").attr("readonly", "readonly");
        }
        $("#ThirdPartyPort").val("");
    });

    $("#ddlShop").change(function () {
        LoadShopAndUpdate($("#ddlShop").val());
    });

    //ADD ROWS
    $(".clsRPAddRows,.clsSPAddRows").click(function () {
        event.preventDefault();

        if ($(this).hasClass('clsRPAddRows')) {
            //$("#container").load("/ManageWorkOrder/ManageWorkOrder/AddUpdateRepairViewRows", { viewType: parseInt(REPAIRSPARE.REPAIR) });

            $.ajax({
                type: "POST",
                url: '/ManageWorkOrder/ManageWorkOrder/AddUpdateRepairViewRows',
                cache: false,
                data: { viewType: parseInt(REPAIRSPARE.REPAIR) },
                success: function (html) {
                    $("#tblRPAdditionalRows tr:last").after(html);
                    if ($('#tblRPAdditionalRows').children('tr').length >= 30) {
                        $('.clsRPAddRows').hide();
                    } else
                        $('.clsRPAddRows').show();
                }
            });
        }
        else {
            $.ajax({
                type: "POST",
                url: '/ManageWorkOrder/ManageWorkOrder/AddUpdateRepairViewRows',
                cache: false,
                data: { viewType: parseInt(REPAIRSPARE.SPARE) },
                success: function (html) {
                    $("#tblSPAdditionalRows tr:last").after(html);
                    if ($('#tblSPAdditionalRows').children('tr').length >= 30) {
                        $('.clsSPAddRows').hide();
                    } else
                        $('.clsSPAddRows').show();
                }
            });
            //$("#trSPAdditionalRows tr:last").load("/ManageWorkOrder/ManageWorkOrder/AddUpdateSpareViewRows", { viewType: REPAIRSPARE.SPARE });
        }
    });

    ////GET HOURS
    //$("input[name=btnGetHours]").bind("click", function () {
    //    event.stopPropagation ? event.stopPropagation() : event.cancelBubble = true;
    //    var errMsg = "";
    //    $wo_global_var = {};

    //    if (getBool($("#IsNewWO").val())) {
    //        if ($("#ddlShop").val() == "")
    //            errMsg += WOMSG.SHOP_MISSING + "</br>";

    //        if ($("#ddlCustomer").val() == "")
    //            errMsg += WOMSG.CUSTOMER_MISSING + "</br>";

    //        if ($("#ddlMode").val() == "-1")
    //            errMsg += WOMSG.MODE_MISSING + "</br>";

    //        if (errMsg == "") {
    //            $wo_global_var["CSHOP"] = $("#ddlShop").val();
    //            $wo_global_var["CCUSTOMER"] = $("#ddlCustomer").val();
    //            $wo_global_var["CMODE"] = $("#ddlMode").val();
    //        }
    //    }
    //    else {
    //        $wo_global_var["CSHOP"] = $("#ddlShop").attr("shopCode");
    //        $wo_global_var["CCUSTOMER"] = $("#ddlCustomer").attr("custCode");
    //        $wo_global_var["CMODE"] = $("#ddlMode").attr("modeCode");
    //    }

    //    var result = GetRepVList();
    //    if (result.msg != null)
    //        errMsg += result.msg;
    //    else if (result.repVList.length == 0)
    //        errMsg += WOMSG.REPAIRPART_INCOMPLETEENTRY + "</br>";
    //    else
    //        $wo_global_var["CREPVIEW"] = result.repVList;

    //    if (errMsg != "") {
    //        var sChar = "</br>";
    //        errMsg = errMsg.slice(0, -sChar.length); //slice off last <br> characters here
    //        appendMsg(errMsg, MESSAGETYPE.ERROR);
    //    }
    //    else {
    //        //appendMsg(errMsgR, MESSAGETYPE.CLEAR);

    //        $.ajax({
    //            url: "/ManageWorkOrder/ManageWorkOrder/GetHours",
    //            traditional: true,
    //            data: $wo_global_var,
    //            type: 'POST',
    //            error: function () {
    //                appendMsg(WOMSG.NETWORK_PROBLEM, MESSAGETYPE.ERROR);
    //            },
    //            success: function (data) {
    //                UpdateManHoursRepV(data.RepVList);
    //                appendMsg(data.UIMsg, data.Status);
    //            }
    //        });
    //    }
    //});

    $('.clsEqp').on('keydown', function (event) {
        var keycode = event.keyCode;
        if (keycode == 9 || keycode == 13) {
            var modeVal = $('#ddlMode').val();
            $('#ddlMode').html($('<option>', {
                value: "-1",
                text: "Loading...",
            }));
            EqpRKEMCall();
            $('.clsVenR')[0].focus();
        }
    });

    ////REVIEW
    //$("input[name=btnReview]").bind("click", function () {
    //    event.preventDefault();
    //    $wo_global_var = {};
    //    $("#SummarySpace").html("");
    //    appendMsg(WOMSG.WAITINFO, MESSAGETYPE.INFO);
    //    $(this).prop("disabled", true);
    //    var errMsgR = FnBasicValidationsAndDataCollection();
    //    if (errMsgR != "") {
    //        var sChar = "</br>";
    //        errMsgR = errMsgR.slice(0, -sChar.length); //slice off last <br> characters here
    //        //appendMsg(WOMSG.WAITINFO, MESSAGETYPE.CLEAR);
    //        appendMsg(errMsgR, MESSAGETYPE.ERROR);
    //        $(this).removeAttr('disabled');
    //    }
    //    else {
    //        //appendMsg(errMsgR, MESSAGETYPE.CLEAR);
    //        var json = JSON.stringify($wo_global_var);
    //        $.ajax({
    //            //contentType: "application/json;",
    //            url: "/ManageWorkOrder/ManageWorkOrder/ReviewWO",
    //            traditional: true,
    //            data: $wo_global_var,
    //            type: 'POST',
    //            //dataType: 'json',
    //            error: function () {
    //                $("#SummarySpace").html("");
    //                appendMsg(WOMSG.RKEM_NOTCALLED, MESSAGETYPE.ERROR);
    //            },
    //            success: function (data) {
    //                $("#SummarySpace").html(data._woSummary);
    //                appendMsg(data.UIMsg, data.Status);
    //            }
    //        });
    //        $(this).removeAttr('disabled');
    //    }
    //});

    //$("input[name=btnSubmit]").bind("click", function () {
    //    event.preventDefault();
    //    $wo_global_var = {};
    //    appendMsg(WOMSG.WAITINFO, MESSAGETYPE.INFO);
    //    $(this).prop("disabled", true);
    //    var errMsgR = FnBasicValidationsAndDataCollection();
    //    if (errMsgR != "") {
    //        var sChar = "</br>";
    //        errMsgR = errMsgR.slice(0, -sChar.length); //slice off last <br> characters here
    //        appendMsg(errMsgR, MESSAGETYPE.ERROR);
    //        $(this).removeAttr('disabled');
    //    }
    //    else {
    //        // appendMsg(errMsgR, MESSAGETYPE.CLEAR);
    //        $.ajax({
    //            url: "/ManageWorkOrder/ManageWorkOrder/SubmitWO",
    //            traditional: true,
    //            data: $wo_global_var,
    //            type: 'POST',
    //            error: function () {
    //            },
    //            success: function (data) {
    //                appendMsg(data.UIMsg, data.Status);
    //            }
    //        });
    //        $(this).removeAttr('disabled');
    //    }
    //});

    //$("input[name=btnSaveAsDraft]").bind("click", function () {
    //    event.preventDefault();
    //    $wo_global_var = {};
    //    appendMsg(WOMSG.WAITINFO, MESSAGETYPE.INFO);
    //    $(this).prop("disabled", true);
    //    var errMsgR = FnBasicValidationsAndDataCollection();
    //    if (errMsgR != "") {
    //        var sChar = "</br>";
    //        errMsgR = errMsgR.slice(0, -sChar.length); //slice off last <br> characters here
    //        appendMsg(errMsgR, MESSAGETYPE.ERROR);
    //        $(this).removeAttr('disabled');
    //    }
    //    else {
    //        //appendMsg(errMsgR, MESSAGETYPE.CLEAR);
    //        $.ajax({
    //            url: "/ManageWorkOrder/ManageWorkOrder/SaveAsDraft",
    //            traditional: true,
    //            data: $wo_global_var,
    //            type: 'POST',
    //            error: function () {
    //            },
    //            success: function (data) {
    //                appendMsg(data.UIMsg, data.Status);
    //            }
    //        });
    //        $(this).removeAttr('disabled');
    //    }
    //});

    //$("input[name=btnClear]").bind("click", function () {

    //    location.reload();
    //    return;
    //    //event.preventDefault();

    //    //$.ajax({
    //    //    url: "/ManageWorkOrder/ManageWorkOrder/ClearWO",
    //    //    traditional: true,
    //    //    data: { isSingle: $("#IsNewWO").attr("IsSingle") },
    //    //    type: 'POST',
    //    //    dataType: 'json',
    //    //    error: function () {
    //    //    },
    //    //    success: function (data) {
    //    //    }
    //    //});
    //});

    //$("input[name=btnPrint]").bind("click", function () {
    //    event.preventDefault();
    //    PrintDoc();
    //});

    //$("input[name=btnDelete]").bind("click", function () {
    //    event.preventDefault();
    //    appendMsg(WOMSG.WAITINFO, MESSAGETYPE.INFO);
    //    $(this).prop("disabled", true);
    //    //appendMsg(errMsgR, MESSAGETYPE.CLEAR);
    //    $.ajax({
    //        url: "/ManageWorkOrder/ManageWorkOrder/DeleteWO",
    //        traditional: true,
    //        data: { WOID: $("#IsNewWO").attr("WOID") },
    //        type: 'POST',
    //        error: function () {
    //        },
    //        success: function (data) {
    //            appendMsg(data.UIMsg, data.Status);
    //        }
    //    });
    //});
});


function EqpRKEMCall() {
    if ($('.clsEqp').val() == null || $('.clsEqp').val() == "") {
        appendMsg(WOMSG.EQP_MISSING, MESSAGETYPE.ERROR);
        $('.clsEqp').focus();
        $('#ddlMode').html($('<option>', {
            value: "-1",
            text: "No Mode(s) Returned for Equipment",
        }));
        return false;
    } else {
        //appendMsg(WOMSG.EQP_MISSING, MESSAGETYPE.CLEAR);
    }

    event.preventDefault();

    $.ajax({
        url: "/ManageWorkOrder/ManageWorkOrder/TabbedCallRKEM",
        traditional: true,
        data: { EqpNo: $.trim($('.clsEqp').val()).toUpperCase(), shpCode: $("#ddlShop").val(), vendRefNo: $("#VendorRefNo").val() },
        type: 'POST',
        dataType: 'json',
        error: function () {
            $('#ddlMode').html($('<option>', {
                value: "-1",
                text: "No Mode(s) Returned for Equipment",
            }));
            clearRKEM();
            appendMsg(WOMSG.RKEM_NOTCALLED, MESSAGETYPE.ERROR);
        },
        success: function (data) {
            $('#ddlMode').empty();
            if (data == null || data.data == null || data.data.ModeList == null) {
                $('#ddlMode').html($('<option>', {
                    value: "-1",
                    text: "No Mode(s) Returned for Equipment",
                }));
                appendMsg(WOMSG.RKEM_NOTCALLED, MESSAGETYPE.ERROR);
                clearRKEM();
            }

            else {
                $.each(data.data.ModeList, function (i, item) {
                    $('#ddlMode').append($('<option>', {
                        value: item.ModeCode,
                        text: item.ModeCode + "-" + item.ModeDescription,
                    }));
                });
                clearError();
                //appendMsg(WOMSG.RKEM_NOTCALLED, MESSAGETYPE.CLEAR);
                //appendMsg(WOMSG.MODE_MISSING, MESSAGETYPE.CLEAR);

                $("#RKType").html(data.data.COType + "/" + data.data.Eqstype);
                $("#RKInService").html(GetDate(data.data.EQInDate));
                $("#RKProfile").html(data.data.EQProfile);
                if ((data.data.Size != null && data.data.Size != undefined) && (data.data.Eqouthgu != null && data.data.Eqouthgu != undefined))
                    $("#RKSize").html(data.data.Size + "/" + data.data.Eqouthgu);
                else
                    $("#RKSize").html("");
                //setting value of Indicator code in unitidentifier as asked by Abu
                $("#lblIndicator").html(data.IndCode);

                $("#RKPTIDate").html(GetDate(data.data.Deldatsh));
                $("#RKMaterial").html(data.data.Eqmatr);
                $("#RKReeferMakeModel").html("");
                if (data.data.COType == "CONT") {
                    if (data.data.Eqstype == "REEF") {
                        $("#RKReeferMakeModel").html(data.data.EQRuman + "/" + data.data.EQRutyp);
                    }
                    else {
                    }
                    $("#RKBoxMfg").html(data.data.EqMancd);
                    $("#RKGenSetMakeModel").html("");
                }
                else if (data.data.COType == "GENS") {
                    $("#RKBoxMfg").html("");
                    $("#RKGenSetMakeModel").html(data.data.EqMancd + "/" + data.data.EQRutyp);
                }

                $("#RKDamage").html(data.data.Damage);
                $("#RKLeasingCompany").html(data.data.LeasingCompany);
                $("#RKLeasingContract").html(data.data.LeasingContract);
                $("#RKGradeCode").html(data.data.GradeCode);
                $("#RKEqpNotFound").html(data.data.EqpNotFound);
            }
        }
    });
}

function clearRKEM() {
    $("#RKType").html("");
    $("#RKInService").html("");
    $("#RKProfile").html("");
    $("#RKReeferMakeModel").html("");
    $("#RKSize").html("");
    $("#RKPTIDate").html("");
    $("#RKMaterial").html("");
    $("#RKGenSetMakeModel").html("");
    $("#RKBoxMfg").html("");
    $("#RKDamage").html("");
    $("#RKLeasingCompany").html("");
    $("#RKLeasingContract").html("");
    $("#RKGradeCode").html("");
    $("#RKEqpNotFound").html("");
    $("#lblIndicator").html("");
}


function PiecesOnBlur(objThis) {
    event.preventDefault();
    var txtVal = $(objThis).val();
    var maxVal = $(objThis).attr("maxquantity");
    if (txtVal == "") {
        if (parseInt(maxVal) > 0)
            $(objThis).val("1");
        else
            $(objThis).val("0");
    }
    else {
        if (txtVal > maxVal) {
            alert(WOMSG.REPAIRPART_EXCEEDMAXQUANT + maxVal + ".");
            //dispAlert(objThis, WOMSG.REPAIRPART_EXCEEDMAXQUANT + maxVal + ".");
            $(objThis).val("0");
            HighlightError(objThis, true);
        }
        else
            HighlightError(objThis, false);
    }
}

function FnBasicValidationsAndDataCollection() {
    //RohitM : field values are stored in global array $wo_global_var as per items mentioned in array UIITEMS of controller.
    //if needed to add or remove something then do make changes in the array and ConstructWOD method as required.

    var errMsg = "";
    var strContent = "";
    GetHrsCalc();//for updating the ordinary hours textbox
    $wo_global_var["CCREATENEW"] = getBool($("#IsNewWO").val());
    $wo_global_var["CISSINGLE"] = getBool($("#IsNewWO").attr("IsSingle"));
    $wo_global_var["CWOID"] = $("#IsNewWO").attr("WOID");

    if (getBool($("#IsNewWO").val())) {
        if ($("#ddlShop").val() == "")
            errMsg += WOMSG.SHOP_MISSING + "</br>";

        if ($("#ddlCustomer").val() == "")
            errMsg += WOMSG.CUSTOMER_MISSING + "</br>";

        if ($("#ddlMode").val() == "-1")
            errMsg += WOMSG.MODE_MISSING + "</br>";

        if (errMsg == "") {
            $wo_global_var["CSHOP"] = $("#ddlShop").val();
            $wo_global_var["CCUSTOMER"] = $("#ddlCustomer").val();
            $wo_global_var["CMODE"] = $("#ddlMode").val();
        }
    }
    else {
        $wo_global_var["CSHOP"] = $("#ddlShop").attr("shopCode");
        $wo_global_var["CCUSTOMER"] = $("#ddlCustomer").attr("custCode");
        $wo_global_var["CMODE"] = $("#ddlMode").attr("modeCode");
    }

    errMsg += GetCause();
    errMsg += GetThirdPartyPort();

    $wo_global_var["CCURRENCY"] = $("#Currency").attr("CurrCode");

    //this will always have null as completion date appears when editing is done
    $wo_global_var["CCOMPLETIONDATE"] = ($("#CompletionDate").val() == null ? "" : $("#CompletionDate").val());

    errMsg += GetEqpVendList();
    //setting value of Indicator code in unitidentifier as asked by Abu
    $wo_global_var["CUNITIDENDIGIT"] = $("#lblIndicator").html();

    errMsg += GetRemarks();
    errMsg += GetTaxParts();

    //check repair part and spare part duplicacy and populate to global array
    errMsg += CheckRepairSpare();
    errMsg += GetOrdHours();
    return errMsg;
}

function GetThirdPartyPort() {
    var errMsg = "";
    //TPP is not required for Multiple WO ,so just passing empty val,in order to maintain the ENUM sequence
    if (getBool($("#IsNewWO").attr("IsSingle"))) {
        $wo_global_var["CTHIRDPARTYPORT"] = $("#ThirdPartyPort").val();
    }
    else
        $wo_global_var["CTHIRDPARTYPORT"] = "";

    return errMsg;
}

function GetCause() {
    var errMsg = "";
    if (getBool($("#IsNewWO").attr("IsSingle"))) {
        if ($("#UICause").val() == "")
            errMsg = WOMSG.CAUSE_MISSING + "</br>";
        else
            $wo_global_var["CCAUSE"] = $("#UICause").val();
    }
    else {
        $wo_global_var["CCAUSE"] = $("#UICause").attr("CauseCode");
    }
    return errMsg;
}

function GetOrdHours() {
    var errMsg = ""; strContent = "";
    if (parseFloat($("input[name=TotalManHourReg]").val()) < 0)
        errMsg = WOMSG.HOURNOLESS + "</br>";
    else
        strContent = $("input[name=TotalManHourReg]").val() + SPLITBYPIPE + $("input[name=TotalManHourOverTime]").val() + SPLITBYPIPE + $("input[name=TotalManHourDoubleTime]").val() + SPLITBYPIPE + $("input[name=TotalManHourMisc]").val();

    $wo_global_var["CHOURS"] = strContent;
    return errMsg;
}

function GetTaxParts() {
    var errMsg = "";
    //if ($("#SalesTaxParts").length != 0) {
    //    if ($("#SalesTaxParts").val() == null)
    //        errMsg += WOMSG.SALESTAXPC_MISSING + "</br>";
    //    else {
    //        $wo_global_var["CSALESTAXPARTSCOST"] = $("#SalesTaxParts").val();
    //    }

    //    if ($("#SalesTaxLabour").val() == null)
    //        errMsg += WOMSG.SALESTAXLC_MISSING + "</br>";
    //    else {
    //        $wo_global_var["CSALESTAXLABOURCOST"] = $("#SalesTaxLabour").val();
    //    }

    //    if ($("#ImportTax").val() == null)
    //        errMsg += WOMSG.IMPORTTAXCOST_MISSING + "</br>";
    //    else {
    //        $wo_global_var["CIMPORTTAXCOST"] = $("#ImportTax").val();
    //    }
    //}
    if ($("#ImportTax").length != 0) {
        $wo_global_var["CIMPORTTAXCOST"] = $("#ImportTax").val();
    }
    return errMsg;
}

function GetEqpVendList() {
    var errMsg = strContent = "";
    var eqpVend = [];
    if (getBool($("#IsNewWO").attr("IsSingle"))) {
        if (!getBool($("#IsNewWO").val())) {
            //set code for getting equipmnt no.
            strContent = $("#EquipmentNo").html() + SPLITBYPIPE;
            //strContent += $.trim($("#VendorRefNo").val());
            strContent += $.trim($(".clsVenR").val());
        }
        else {
            if ($.trim($('.clsEqp').val()) == null || $.trim($('.clsEqp').val()) == "")
                errMsg += WOMSG.EQP_MISSING + "</br>";
            else
                strContent = $.trim($('.clsEqp').val()).toUpperCase() + SPLITBYPIPE;

            strContent += $.trim($("#VendorRefNo").val());
        }
        eqpVend.push(strContent);
        $wo_global_var["CEQPVENDLIST"] = eqpVend;
    }
    else {
        var dup = false;
        if (findDuplicates("tblEquipment", "clsENO", null, true)) {
            errMsg += WOMSG.EQP_NOTUNIQUE + "</br>";
            dup = true;
        }
        if (findDuplicates("tblEquipment", "clsVenR", null, true)) {
            errMsg += WOMSG.VENDOR_NOTUNIQUE + "</br>";
            dup = true;
        }

        if (!dup) {
            $wo_global_var["CEQPVENDLIST"] = FetchEqpVend("tblEquipment", "EquipmentNo", "VendorRefNo");
        }
    }
    return errMsg;
}

function GetRemarks() {
    var errMsg = "";
    if ($('#chkShowComments').is(':checked')) {
        if ($("#Remarks").val() == "")
            errMsg = WOMSG.REMARKS_MISSING + "</br>";
        else {
            $wo_global_var["CSHOWCOMMENTS"] = true;
            $wo_global_var["CREMARKS"] = $("#Remarks").val();
        }
    }
    else {
        $wo_global_var["CSHOWCOMMENTS"] = false;//chkShowComments is set to false
        $wo_global_var["CREMARKS"] = $("#Remarks").val();
    }
    return errMsg;
}

function FetchEqpVend(thisTable, eqpTxt, vendTxt) {
    var strContent = "";
    var eqpVend = [];
    $("#" + thisTable + " tr").each(function () {
        strContent = ($(this).find("input[name^='" + eqpTxt + "']")).val();
        if ($.trim(strContent) != "") {
            strContent = (strContent).toUpperCase() + SPLITBYPIPE;
            strContent += ($(this).find("input[name^='" + vendTxt + "']")).val();
            eqpVend.push(strContent);
        }
    });
    return eqpVend;
}

function GetRepVList() {
    if (findDuplicates("tblRPAdditionalRows", "CRep", "cRLocCode", true)) {
        return { iserror: true, repVList: null, msg: WOMSG.REPCODERP_NOTUNIQUE + "</br>" };
    }

    var mDmgCode = false, mRprCode = false, mRprLocCode = false, mPcs = false, mMatCostPP = false, mManHourPP = false, mTPI = false, forAll = false, iserror = false;
    var repView = [];
    var repCodeLst = [];
    var strContent = "";

    $("#tblRPAdditionalRows tr").each(function () {
        if ($.trim($(this).find(".CRep").val()) != "" && $.trim($(this).find(".CRep").val()) != null) {
            strContent = "";
            $(this).find("td").each(function () {
                var inpId = $(this).find("input[type=text],input[type=number]");
                strContent += $.trim($(inpId).val()).toUpperCase() + SPLITBYPIPE;
                if ($.trim($(inpId).val()) == null || $.trim($(inpId).val()) == "") {
                    forAll = iserror = true;
                }
                else {
                    iserror = false;
                    if ($(inpId).hasClass('cDCode')) {
                        if ($.trim($(inpId).val()) == "")
                            iserror = true;
                    }
                    else if ($(inpId).hasClass('CRep')) {
                        repCodeLst.push($.trim($(inpId).val()));
                        if ($.trim($(inpId).val()) == "")
                            iserror = true;
                    }
                    else if ($(inpId).hasClass('cRLocCode')) {
                        if ($.trim($(inpId).val()) == "")
                            iserror = true;
                    }
                    else if ($(inpId).hasClass('cPieces')) {
                        if ($.trim($(inpId).val()) <= 0) {
                            mPcs = iserror = true;
                        }
                    }
                    else if ($(inpId).hasClass('cMCost')) {
                        if ($.trim($(inpId).val()) < 0)
                            mMatCostPP = iserror = true;
                    }
                    else if ($(inpId).hasClass('cMHours')) {
                        if ($.trim($(inpId).val()) <= 0) {
                            mManHourPP = iserror = true;
                        }
                    } else {
                        if ($(inpId).hasClass('cTPI')) {
                            if ($.trim($(inpId).val()) == "")
                                mTPI = iserror = true;
                        }
                    }
                }
                if (iserror)
                    HighlightError(inpId, true);
                else
                    HighlightError(inpId, false);
            });

            strContent = strContent.slice(0, -SPLITBYPIPE.length);
            repView.push(strContent);
        }
    });

    return { iserror: forAll, repVList: repView, msg: null, repCodeLst: repCodeLst };
}

function GetHrsCalc() {
    var sumManHrs = 0; var pcs = 0, mahHr = 0;

    $("#tblRPAdditionalRows tr").each(function () {
        if ($.trim($(this).find(".CRep").val()) != "" && $.trim($(this).find(".CRep").val()) != null) {
            strContent = ""; pcs = 0; mahHr = 0;
            $(this).find("td").each(function () {
                var inpId = $(this).find("input[type=text],input[type=number]");
                if ($(inpId).hasClass('cPieces')) {
                    pcs = $.trim($(inpId).val());
                    if (pcs <= 0) {
                        mPcs = iserror = true;
                        pcs = 0;
                    }
                }
                else if ($(inpId).hasClass('cMHours')) {
                    mahHr = $.trim($(inpId).val());
                    if (mahHr <= 0) {
                        mManHourPP = iserror = true;
                        mahHr = 0;
                    }
                    sumManHrs = parseFloat(sumManHrs) + ((parseFloat(pcs) * parseFloat(mahHr)));
                }
            });
        }
    });

    sumManHrs = CalcTotalHrs(sumManHrs);
    $("input[name=TotalManHourReg]").attr("totManHr", sumManHrs);
    $("input[name=TotalManHourReg]").val(parseFloat(sumManHrs).toFixed(1));
}


function CalcTotalHrs(sumManHrs) {
    var sTmp = 0.0;
    sTmp = $("input[name=TotalManHourOverTime]").val();
    if (sTmp != "" && parseFloat(sTmp) > 0) {
        sumManHrs = parseFloat(parseFloat(sumManHrs) - parseFloat(sTmp)).toFixed(1);
    }
    sTmp = $("input[name=TotalManHourDoubleTime]").val();
    if (sTmp != "" && parseFloat(sTmp) > 0) {
        sumManHrs = parseFloat(parseFloat(sumManHrs) - parseFloat(sTmp)).toFixed(1);
    }
    sTmp = $("input[name=TotalManHourMisc]").val();
    if (sTmp != "" && parseFloat(sTmp) > 0) {
        sumManHrs = parseFloat(parseFloat(sumManHrs) - parseFloat(sTmp)).toFixed(1);
    }
    return sumManHrs;
}

function CalcHrs(objThis) {
    event.preventDefault();
    var sumManHrs = $("input[name=TotalManHourReg]").attr("totManHr");
    sumManHrs = CalcTotalHrs(sumManHrs);
    $("input[name=TotalManHourReg]").val(parseFloat(sumManHrs).toFixed(1));
}

function CheckRepairSpare() {
    var emptyMsg = "";
    var result = GetRepVList();
    if (result.msg != null)
        return result.msg;

    if (result.iserror || result.repVList.length == 0) {
        emptyMsg += WOMSG.REPAIRPART_INCOMPLETEENTRY + "</br>";
        if (result.repVList.length != 0)
            emptyMsg += WOMSG.REPAIRPART_COMPLETEHIGHLIGHTEDENTRY + "</br>";
        return emptyMsg;
    }
    else
        $wo_global_var["CREPVIEW"] = result.repVList;

    //spare part is not present in case of Multiple equipment
    if (getBool($("#IsNewWO").attr("IsSingle"))) {
        emptyMsg += $.trim(ValidateSpareRepCodeEntry(result.repCodeLst));
    }
    return emptyMsg;
}

function ValidateSpareRepCodeEntry(objRRepList) {
    if (findDuplicates("tblSPAdditionalRows", "CRep", "cPartCode", true)) {
        return WOMSG.OWNERSPNSP_NOTUNIQUE + "</br>";
    }
    else {
        var objSRepList = [];
        objSRepList = GetSpareData();
        if (objRRepList != null && objSRepList != null && objSRepList.length > 0) {
            objRRepList = $.unique(objRRepList);
            objSRepList = $.unique(objSRepList);
            if (!compareArrays(objRRepList, objSRepList))
                return WOMSG.SPAREPART_EXTRAREPCODE + "</br>";
            else
                return "";
        } else
            return "";
    }
}

function GetSpareData() {
    var objSRepList = [];
    var sprView = [];
    var strContent = "";
    var repCode = "", partCode = "";
    var hasSummary = $.trim($("#SummarySpace").html());
    $("#tblSPAdditionalRows tr").each(function () {
        repCode = $.trim($(this).find(".CRep").val());
        if (repCode != "" && repCode != null) {
            partCode = $.trim($(this).find(".cPartCode").val()).toUpperCase();
            strContent = repCode;
            objSRepList.push(strContent);
            strContent += SPLITBYPIPE;
            strContent += $.trim($(this).find(".cPieces").val()) + SPLITBYPIPE;
            strContent += partCode + SPLITBYPIPE;
            if (hasSummary != "")
                strContent += GetSerialNo(repCode, partCode);
            else {
                var attr = $.trim($(this).find(".cPartCode").attr("SRNO"));
                if (typeof attr !== typeof undefined && attr != "")
                    strContent += $.trim($(this).find(".cPartCode").attr("SRNO"));
                else
                    strContent += "";
            }
            sprView.push(strContent);
        }
    });
    $wo_global_var["CSPAREVIEW"] = sprView;
    return objSRepList;
}

function GetSerialNo(repCode, partCode) {
    //// SN~+@Model.dbWOD.WorkOrderID+'~'+ @repI.RepairCode.RepairCod +'~'+ @sprI.OwnerSuppliedPartsNumber"
    //var srID = "SN||" + repCode + "||" + partCode;
    //if ($("#" + srID).length > 0)
    //    return $("#" + srID).val();
    //else
    //    return "";

    var srID = "";

    if ($('input[id^="SN||"]').length > 0) {
        $('input[id^="SN||"]').each(function () {
            if (($(this).attr("id").split("||")[1] == repCode) && ($(this).attr("id").split("||")[2] == partCode)) {
                srID = $(this).val();
                return;
            }
        });
    }

    return srID;
}

function UpdateManHoursRepV(repVList) {
    var rVal = "";
    $.each(repVList, function (i, item) {
        $("#tblRPAdditionalRows tr").each(function () {
            rVal = $.trim($(this).find(".CRep").val());
            if (rVal != "" && rVal != null) {
                if (rVal == $.trim(item.RepairCode.RepairCod)) {
                    $(this).find(".cMHours").val(item.ManHoursPerPiece == null ? "0.0" : (item.ManHoursPerPiece).toFixed(1));
                    HighlightError($(this).find(".cMHours"), false);
                }
            }
        });
    });
}


//pass table Id and the element name, so that element with same value for attribute name doesnt get messed up. 
function findDuplicates(thisTable, col1, col2, allChar) {
    var isDuplicate = false; Toclear = true;
    $("#" + thisTable).find("." + col1).each(function (i, el1) {
        var Toclear = true;
        var current_valCol1 = $.trim($(el1).val()).toUpperCase();
        if (col2 != null) {
            var current_valCol2 = $.trim($($(el1).closest("tr")).find("." + col2).val()).toUpperCase();
        }
        var indexI = i;
        if (current_valCol1 != "") {
            if (col2 == null || (col2 != null && current_valCol2 != "")) {
                if ($.trim(current_valCol2).length > 2 && col2 != "cPartCode" && !allChar)
                    current_valCol2 = current_valCol2.substring(0, 2);
                $("#" + thisTable).find("." + col1).each(function (i, el2) {
                    if (col2 == null) {
                        if ($.trim($(el2).val()).toUpperCase() == current_valCol1 && indexI != i) {
                            isDuplicate = true;
                            Toclear = false;
                            HighlightError(el2, true);
                            HighlightError(el1, true);
                            return false;
                        }
                    }
                    else {
                        var current_valCol2El2 = $.trim($($(el2).closest("tr")).find("." + col2).val()).toUpperCase();
                        if ($.trim(current_valCol2El2).length > 2 && col2 != "cPartCode" && !allChar)
                            current_valCol2El2 = current_valCol2El2.substring(0, 2);
                        if (($.trim($(el2).val()).toUpperCase() == current_valCol1 && current_valCol2El2 == current_valCol2) && indexI != i) {
                            isDuplicate = true;
                            Toclear = false;
                            HighlightError(el2, true);
                            HighlightError(el1, true);
                            return false;
                        }
                    }
                });

                if (Toclear) {
                    HighlightError(el1, false);
                }
            }
        }
    });
    return isDuplicate;
}

//GET HOURS
function GetHours() {
    event.stopPropagation ? event.stopPropagation() : event.cancelBubble = true;
    var errMsg = "";
    $wo_global_var = {};

    if (getBool($("#IsNewWO").val())) {
        if ($("#ddlShop").val() == "")
            errMsg += WOMSG.SHOP_MISSING + "</br>";

        if ($("#ddlCustomer").val() == "")
            errMsg += WOMSG.CUSTOMER_MISSING + "</br>";

        if ($("#ddlMode").val() == "-1")
            errMsg += WOMSG.MODE_MISSING + "</br>";

        if (errMsg == "") {
            $wo_global_var["CSHOP"] = $("#ddlShop").val();
            $wo_global_var["CCUSTOMER"] = $("#ddlCustomer").val();
            $wo_global_var["CMODE"] = $("#ddlMode").val();
        }
    }
    else {
        $wo_global_var["CSHOP"] = $("#ddlShop").attr("shopCode");
        $wo_global_var["CCUSTOMER"] = $("#ddlCustomer").attr("custCode");
        $wo_global_var["CMODE"] = $("#ddlMode").attr("modeCode");
    }

    var result = GetRepVList();
    if (result.msg != null)
        errMsg += result.msg;
    else if (result.repVList.length == 0)
        errMsg += WOMSG.REPAIRPART_INCOMPLETEENTRY + "</br>";
    else
        $wo_global_var["CREPVIEW"] = result.repVList;

    if (errMsg != "") {
        var sChar = "</br>";
        errMsg = errMsg.slice(0, -sChar.length); //slice off last <br> characters here
        appendMsg(errMsg, MESSAGETYPE.ERROR);
    }
    else {
        //appendMsg(errMsgR, MESSAGETYPE.CLEAR);

        $.ajax({
            url: "/ManageWorkOrder/ManageWorkOrder/GetHours",
            traditional: true,
            data: $wo_global_var,
            type: 'POST',
            error: function () {
                appendMsg(WOMSG.NETWORK_PROBLEM, MESSAGETYPE.ERROR);
            },
            success: function (data) {
                UpdateManHoursRepV(data.RepVList);
                GetHrsCalc();
                appendMsg(data.UIMsg, data.Status);
            }
        });
    }
}


//REVIEW
function DoReview() {
    event.preventDefault();
    if (!SetDisabled(true, $("input[name=btnReview]")))
        return;
    $wo_global_var = {};
    $("#SummarySpace").html("");
    appendMsg(WOMSG.WAITINFO, MESSAGETYPE.INFO);
    var errMsgR = FnBasicValidationsAndDataCollection();
    if (errMsgR != "") {
        var sChar = "</br>";
        errMsgR = errMsgR.slice(0, -sChar.length); //slice off last <br> characters here
        //appendMsg(WOMSG.WAITINFO, MESSAGETYPE.CLEAR);
        appendMsg(errMsgR, MESSAGETYPE.ERROR);
        SetDisabled(false);
    }
    else {
        //appendMsg(errMsgR, MESSAGETYPE.CLEAR);
        var json = JSON.stringify($wo_global_var);
        $.ajax({
            //contentType: "application/json;",
            url: "/ManageWorkOrder/ManageWorkOrder/ReviewWO",
            traditional: true,
            data: $wo_global_var,
            type: 'POST',
            dataType: 'json',
            error: function () {
                $("#SummarySpace").html("");
                appendMsg(WOMSG.RKEM_NOTCALLED, MESSAGETYPE.ERROR);
                SetDisabled(false);
            },
            success: function (data) {
                $("#SummarySpace").html(data._woSummary);
                appendMsg(data.UIMsg, data.Status);
                SetDisabled(false);
            }
        });
    }
}


function DoSubmit() {
    event.preventDefault();
    if (!SetDisabled(true, $("input[name=btnSubmit]")))
        return;
    $wo_global_var = {};
    appendMsg(WOMSG.WAITINFO, MESSAGETYPE.INFO);
    var errMsgR = FnBasicValidationsAndDataCollection();
    if (errMsgR != "") {
        var sChar = "</br>";
        errMsgR = errMsgR.slice(0, -sChar.length); //slice off last <br> characters here
        appendMsg(errMsgR, MESSAGETYPE.ERROR);
        SetDisabled(false);
    }
    else {
        // appendMsg(errMsgR, MESSAGETYPE.CLEAR);
        $.ajax({
            url: "/ManageWorkOrder/ManageWorkOrder/SubmitWO",
            traditional: true,
            data: $wo_global_var,
            type: 'POST',
            error: function () {
                SetDisabled(false);
            },
            success: function (data) {
                if (data.Status == MESSAGETYPE.SUCCESS) {
                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    }
                } else {
                    $("#SummarySpace").html(data._woSummary);
                    appendMsg(data.UIMsg, data.Status);
                }
                SetDisabled(false);
            }
        });
    }
}

function DoSaveAsDraft() {
    event.preventDefault();
    if (!SetDisabled(true, $("input[name=btnSaveAsDraft]")))
        return;
    $wo_global_var = {};
    appendMsg(WOMSG.WAITINFO, MESSAGETYPE.INFO);
    var errMsgR = FnBasicValidationsAndDataCollection();
    if (errMsgR != "") {
        var sChar = "</br>";
        errMsgR = errMsgR.slice(0, -sChar.length); //slice off last <br> characters here
        appendMsg(errMsgR, MESSAGETYPE.ERROR);
        SetDisabled(false);
    }
    else {
        //appendMsg(errMsgR, MESSAGETYPE.CLEAR);
        $.ajax({
            url: "/ManageWorkOrder/ManageWorkOrder/SaveAsDraft",
            traditional: true,
            data: $wo_global_var,
            type: 'POST',
            error: function () {
                SetDisabled(false);
            },
            success: function (data) {
                if (data.Status == MESSAGETYPE.SUCCESS) {
                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    }
                } else {
                    $("#SummarySpace").html(data._woSummary);
                    appendMsg(data.UIMsg, data.Status);
                }
                SetDisabled(false);
            }
        });
    }
}


function DoClear() {

    location.reload();
    return;
}


function DoPrint() {
    event.preventDefault();
    PrintDoc();
}


function DoDelete() {
    event.preventDefault();
    if (!SetDisabled(true, $("input[name=btnDelete]")))
        return;
    appendMsg(WOMSG.WAITINFO, MESSAGETYPE.INFO);
    $.ajax({
        url: "/ManageWorkOrder/ManageWorkOrder/DeleteWO",
        traditional: true,
        data: { WOID: $("#IsNewWO").attr("WOID") },
        type: 'POST',
        error: function () {
            SetDisabled(false);
        },
        success: function (data) {
            appendMsg(data.UIMsg, data.Status);
            SetDisabled(false);
        }
    });
}


//$(".clsDamageCode, .clsRepairCode, .clsRepairLocCode, .clsTPI").on("click", function (event) {
function RepairClick(objThis) {
    var dlgFor = $(objThis);
    var modeCode = "";
    if ($(dlgFor).hasClass('clsRepairCode')) {
        if (getBool($("#IsNewWO").val())) {
            modeCode = $("#ddlMode").val();
            if (modeCode == "-1") {
                alert(WOMSG.MODE_MISSING);
                //dispAlert(this, WOMSG.MODE_MISSING);
                event.stopPropagation ? event.stopPropagation() : event.cancelBubble = true;
                return false;
            }
        }
        else {
            modeCode = $("#ddlMode").attr("modeCode");
        }
    }
    $("#ModelContainer").html(getModelBody(true, false));

    $("#tblRPAdditionalRows").find("td[modalTD=modalTD]").removeAttr("modalTD");
    $(dlgFor).parent("td").attr("modalTD", "modalTD");
    var pTitle = "";
    if ($(dlgFor).hasClass('clsDamageCode')) {
        pTitle = "Damage Code";
        $("#dialog").load("/ManageWorkOrder/ManageWorkOrder/RepairDialogues", { dlgType: DLGTYPE.DAMAGE, modeCode: null });
    }
    else if ($(dlgFor).hasClass('clsRepairCode')) {
        pTitle = "Repair Code";
        $("#dialog").load("/ManageWorkOrder/ManageWorkOrder/RepairDialogues", { dlgType: DLGTYPE.REPAIR, modeCode: $.trim(modeCode) });
    }
    else if ($(dlgFor).hasClass('clsRepairLocCode')) {
        pTitle = "Repair Location Code";
        $("#dialog").load("/ManageWorkOrder/ManageWorkOrder/RepairDialogues", { dlgType: DLGTYPE.REPAIRLOCCODE, modeCode: null });
    }
    else {
        pTitle = "TPI Code";
        $("#dialog").load("/ManageWorkOrder/ManageWorkOrder/RepairDialogues", { dlgType: DLGTYPE.TPI, modeCode: null });
    }

    $(".modal-title").html(pTitle);
    $("#btnModelOk").click(OkModel);

    //$("#dialog").dialog({
    //    autoOpen: true,
    //    width: 600,
    //    height: 600,
    //    title: pTitle,
    //    buttons: [
    //        {
    //            text: "Ok",
    //            click: function () {
    //                var selected = $("#dialog input[type='radio']:checked");
    //                if (selected.length > 0) {
    //                    if ($(dlgFor).hasClass('clsRepairLocCode')) {
    //                        if ((selected.val().tr.trim()).length == 2) {
    //                            $(dlgFor).parent("td").find('input[type=text]').val($.trim(selected.val()) + "XX");
    //                        }
    //                        else {
    //                            $(dlgFor).parent("td").find('input[type=text]').val($.trim(selected.val()));
    //                        }
    //                    }
    //                    else {
    //                        $(dlgFor).parent("td").find('input[type=text]').val($.trim(selected.val()));
    //                    }
    //                }
    //                $("#dialog").html("<span>Loading...</span>");
    //                $(this).dialog("close");
    //            }
    //        },
    //        {
    //            text: "Cancel",
    //            click: function () {
    //                $("#dialog").html("<span>Loading...</span>");
    //                $(this).dialog("close");
    //            }
    //        }
    //    ]
    //});
}

function OkModel() {
    event.preventDefault();
    var selected = $("#dialog input[type='radio']:checked");
    if (selected.length > 0) {
        var updateTD = $("#tblRPAdditionalRows").find("td[modalTD=modalTD]");
        if (updateTD != null && $(updateTD).length > 0) {
            if ($(updateTD).find("span").hasClass('clsRepairLocCode')) {
                if ($.trim((selected.val())).length == 2) {
                    $(updateTD).find('input[type=text]').val($.trim(selected.val()) + "XX");
                }
                else {
                    $(updateTD).find('input[type=text]').val($.trim(selected.val()));
                }
            }
            else if ($(updateTD).find("span").hasClass('clsRepairCode')) {
                if (!InValidRepair($.trim(selected.val()))) {
                    $(updateTD).find('input[type=text]').val($.trim(selected.val()));
                    var pcsInput = $(updateTD).parent("tr").find(".cPieces");
                    $(pcsInput).attr("maxquantity", $(selected).attr("maxquantity"));
                    $(pcsInput).attr("manualcode", $(selected).attr("manualcode"));
                }
                else { return false; }
            }
            else {
                $(updateTD).find('input[type=text]').val($.trim(selected.val()));
            }
        }
    }
    $(updateTD).removeAttr("modalTD");
}


function LocBlur(objThis) {
    var contentIs = $.trim($(objThis).val());
    if (contentIs != "") {
        if (contentIs.length < 2) { alert("Invalid Repair Loc Code. Repair Loc Code length must be 4 characters."); $(objThis).focus(); }
        else if (contentIs.length == 2) { $(objThis).val(contentIs + "XX"); }
        else if (contentIs.length == 3) { $(objThis).val(contentIs + "X"); }
    }
}

function RepBlur(objThis) {
    var contentIs = $.trim($(objThis).val());
    if (contentIs != "") {
        if (InValidRepair(contentIs))
            $(objThis).focus();
    }
}

function InValidRepair(valRep) {
    if ($.trim(valRep).length < 4) {
        alert("Repair Code " + valRep + " is invalid.Repair Code length must be 4 characters.");
        return true;
    }
    else if (("0975 0976 0977 9997 9998 9999").indexOf($.trim(valRep)) >= 0) {
        alert("Invalid Repair Code " + valRep + ".");
        return true;
    }
    else
        return false;
}

//function addManHrs() {
//    var f = document.WorkOrder;
//    var sumManHrs = 0;
//    var intRows = f.workOrderRows.value;
//    var OTHrs = 0;
//    var NAN = isFinite("x");
//    var sTmp = '';
//    for (var i = 0; i < intRows; i++) {//changed for 7602, repair code also to be considered in the if condition
//        if (isFinite(parseFloat(f.mHrs[i].value)) && isFinite(parseFloat(f.pcs[i].value)) && isFinite(f.rCd[i].value) && trimString(f.rCd[i].value) != "") {
//            sumManHrs = parseFloat(sumManHrs) + (parseFloat(f.mHrs[i].value) * parseFloat(f.pcs[i].value));
//        }
//    }
//    sTmp = trimString(f.OT1.value);
//    if (isFinite(f.OT1.value) && sTmp != "") {
//        sumManHrs = parseFloat(sumManHrs) - parseFloat(f.OT1.value);
//    }
//    sTmp = trimString(f.OT2.value);
//    if (isFinite(f.OT2.value) && sTmp != "") {
//        sumManHrs = parseFloat(sumManHrs) - parseFloat(f.OT2.value);
//    }
//    sTmp = trimString(f.OT3.value);
//    if (isFinite(f.OT3.value) && sTmp != "") {
//        sumManHrs = parseFloat(sumManHrs) - parseFloat(f.OT3.value);
//    }
//    if (isFinite(sumManHrs)) {
//        f.ordinaryHours.value = round_decimals(sumManHrs, 1);
//        //if(sumManHrs<0)
//        //{
//        //alert('Ordinary Hours Can Not be a Negative Amount');
//        //}
//    }
//    else {
//        f.ordinaryHours.value = '';
//    }
//}


function LoadShopAndUpdate(shopCode) {
    SetDisabled(true);
    $.ajax({
        type: "POST",
        url: '/ManageWorkOrder/ManageWorkOrder/GetCustomersOnShopChange',
        cache: false,
        data: { shopCode: shopCode },
        error: function () {
            SetDisabled(false);
        },
        success: function (data) {
            // enable code for ddl shop change
            if (data.Status != MESSAGETYPE.ERROR) {
                /*
                HARDCODED : MAER :As per dicussion
                $("#ddlCustomer").empty();
                $.each(data.ItemList, function (i, item) {
                    $('<option/>', {
                        'value': item.CustomerCode,
                        'text': item.CustomerCode,
                        'selected': (item.CustomerCode == "MAER" ? "selected" : '')
                    }).appendTo("#ddlCustomer");
                });
                */

                $("#Currency").attr("CurrCode", data.CurrCode.Cucdn);
                $("#Currency").html(data.CurrCode.CurrName);
                if ($("#ImportTax").length > 0)
                    $("#ImportTax").val("0.00");
                (getBool(data.ShowTax) ? $(".clsImport").hide() : $(".clsImport").hide());//Kasturee_Import_Tax-14-08-18
            }
            else {
                appendMsg(data.UIMsg, data.Status);
            }
            SetDisabled(false);
        }
    });
}


function LoadShopDetails() {

    if (getBool($("#IsNewWO").val())) {
        SetDisabled(true);
        $('#ddlShop').html($('<option>', {
            value: "-1",
            text: "Loading...",
        }));
        $('#ddlCustomer').html($('<option>', {
            value: "-1",
            text: "Loading...",
        }));
        $("#ddlCustomer").empty();
        $('<option/>', {
            'value': "MAER",
            'text': "MAER"
        }).appendTo("#ddlCustomer");

        $.ajax({
            type: "POST",
            url: '/ManageWorkOrder/ManageWorkOrder/GetShopDetails',
            cache: false,
            data: {},
            error: function (data) {
                SetDisabled(false);
            },
            success: function (data) {
                if (data == null || data == undefined || data.Status == undefined) {
                    window.location = "/ManageSecurity/ManageSecurity/SessionExpire";
                }
                else if (data.Status != MESSAGETYPE.ERROR) {
                    $("#ddlShop").empty();
                    $.each(data.ItemList, function (i, item) {
                        $('<option/>', {
                            'text': item.ShopCode + "-" + item.ShopDescription,
                            'value': item.ShopCode
                        }).appendTo("#ddlShop");

                        /*
                        HARDCODED : MAER :As per dicussion
                        if (i == 0) {
                            $("#ddlCustomer").empty();
                            $.each(item.Customer, function (i, item) {
                                $('<option/>', {
                                    'value': item.CustomerCode,
                                    'text': item.CustomerCode,
                                    'selected': (item.CustomerCode == "MAER" ? "selected" : '')
                                }).appendTo("#ddlCustomer");
                            });
                        }
                        */
                    });
                    var codeS = $.trim($("#ErrorMsgContainer").attr("PrevShop"));
                    if (codeS == "") {
                        $("#Currency").attr("CurrCode", data.CurrCode.Cucdn);
                        $("#Currency").html(data.CurrCode.CurrName);
                        (getBool(data.ShowTax) ? $(".clsImport").hide() : $(".clsImport").hide());//Kasturee_Import_Tax-14-08-18--CSI raised to disable import tax from front end
                        if ($("#ImportTax").length > 0)
                            $("#ImportTax").val("0.00");
                    }
                    else {
                        $("#ddlShop").val(codeS);
                        LoadShopAndUpdate(codeS);
                    }
                }
                else {
                    appendMsg(data.UIMsg, data.Status);
                }
                SetDisabled(false);
            }
        });
    }
}


function SetDisabled(set, objThis) {

    if (set && (objThis != null)) {
        var attr = $(objThis).attr('disabled');
        if (set && typeof attr !== typeof undefined && attr !== false) {
            alert("Your previous request is being processed.Please wait...");
            return false;
        }
    }
    if (set) {
        //if (getBool($("#IsNewWO").val())) {
        $("#ddlShop").attr('disabled', true);
        $("#ddlCustomer").attr('disabled', true);
        $("input[name=btnReview]").attr('disabled', true);
        $("input[name=btnSubmit]").attr('disabled', true);
        $("input[name=btnSaveAsDraft]").attr('disabled', true);
        $("input[name=btnClear]").attr('disabled', true);
        $("input[name=btnPrint]").attr('disabled', true);
        $('.clsEqp').attr('disabled', true);

        if ($("input[name=btnDelete]").length > 0)
            $("input[name=btnDelete]").attr('disabled', true);
        //}
    }
    else {
        $("#ddlShop").removeAttr('disabled');
        $("#ddlCustomer").removeAttr('disabled');
        $("input[name=btnReview]").removeAttr('disabled');
        $("input[name=btnSubmit]").removeAttr('disabled');
        $("input[name=btnSaveAsDraft]").removeAttr('disabled');
        $("input[name=btnClear]").removeAttr('disabled');
        $("input[name=btnPrint]").removeAttr('disabled');
        if ($('.clsEqp').length > 0) {
            $('.clsEqp').removeAttr('disabled');
            $('.clsEqp')[0].focus();
        }

        if ($("input[name=btnDelete]").length > 0)
            $("input[name=btnDelete]").removeAttr('disabled');
    }

    return true;
}

function changeVal(objThis, type) {
    if (type == "C") {
        if ((event.keyCode >= 65 && event.keyCode <= 90) || (event.keyCode >= 97 && event.keyCode <= 122)) {
            $(objThis).val(String.fromCharCode(event.keyCode));
            return true;
        }
    }
    //else if (type == "N") {
    //    if (event.keyCode >= 48 && event.keyCode <= 57) {
    //        $(objThis).val(String.fromCharCode(event.keyCode));
    //        return true;
    //    }
    //}
}
