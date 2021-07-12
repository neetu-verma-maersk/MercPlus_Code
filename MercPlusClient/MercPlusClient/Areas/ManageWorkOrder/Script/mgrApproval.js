/// <reference path="../../../Scripts/mainjs.js" />
$ma_global_var = {};
var SPLITBYPIPE = "|*|";
var REPAIRSPARE = { REPAIR: 1, SPARE: 2 };
var DLGTYPE = { DAMAGE: 1, REPAIR: 2, REPAIRLOCCODE: 3, TPI: 4 };
var MAMSG = {
    MODE_MISSING: 'Please select Mode.',
    REPCODERP_MISSING: 'Please select some Repair Code to fetch the hour(s) from Get Hours.',
    EQP_MISSING: 'Please insert valid Equipment Number.',
    VENDOR_MISSING: 'Please insert valid Vendor Reference Number.',
    RKEM_NOTCALLED: 'Container type not received from RKEM or RKEM call not performed.',
    WAITINFO: 'Processing request.Please wait...',
    SHOP_MISSING: 'Please select Shop.',
    CUSTOMER_MISSING: 'Please select Customer.',
    CAUSE_MISSING: 'Please select Cause.',
    EQP_NOTUNIQUE: 'Please insert unique Equipment Number.',
    VENDOR_NOTUNIQUE: 'Please insert unique Vendor Reference Number.',
    REPCODERP_NOTUNIQUE: 'Please select unique Repair Code for Repair part.',
    REPCODESP_NOTUNIQUE: 'Please select unique Repair Code for Spare part.',
    OWNERSPNSP_NOTUNIQUE: 'Please select unique Owner Supplied Parts Number for Spare part.',
    REPAIRPART_INCOMPLETEENTRY: 'At least one Repair(s) entry must be completely filled for this WO.',
    REPAIRPART_COMPLETEHIGHLIGHTEDENTRY: 'Please fill in the highlighted fields for Repair(s) entry.',
    REPAIRPART_PIECEMISSING: 'Repair piece count is required for repair code.',
    SPAREPART_EXTRAREPCODE: 'All Repair Code in Spare part(s) list must have reference in Repair Code of Repair List.',
    REPAIRPART_EXCEEDMAXQUANT: 'Maximum pieces allowed for selected Repair Code is ',
    THIRDPARTY_MISSING: 'Please enter Third Party Port number ',
    CURRENCY_MISSING: 'Currency not found ',
    COMPLETIONDATE_MISSING: 'Please select Completion date for this Work Order ',
    REMARKS_MISSING: 'Please enter remarks for this Work Order ',
    SALESTAXPC_MISSING: 'Please provide value for Sales tax Part(s) cost for this Work Order ',
    SALESTAXLC_MISSING: 'Please provide value for Sales tax Labour cost for this Work Order ',
    IMPORTTAXCOST_MISSING: 'Please provide value for Import tax cost for this Work Order ',
    NETWORK_PROBLEM: 'Unable to perform Delete due to network error, please try in a while ',
    REJECT_E_REMARKS: 'Explicit external remarks are required when Rejecting estimates:\n External remarks Length must be at least 2 characters.',
    REJECT_I_REMARKS: 'Internal remarks are required when Rejecting estimates. ',
    FORWARD_I_REMARKS: 'Internal remarks are required when Forwarding estimates. ',
    SUSPEND_I_REMARKS: 'Internal remarks are required when Suspending estimates ',
    CORENO_MISSING: 'Core Part Serial Number is required for ',
    REMARKS_SIZEEXCEED: 'Maximum 255 characters are allowed. ',
    UI_RELOAD: 'Request successfully processed. Fetching next record... ',
    REQUEST_SUCCESS: 'Request successfully processed. '
}

$(function () {
    $("#UICause").change(function () {
        //if Cause selected item not in these two then enable the 
        //"1 - Wear and Tear, 2 - 1Handling Damage"
        if (parseInt($(this).val()) > 2)
            $("#ThirdPartyPort").removeAttr("readonly");
        else {
            $("#ThirdPartyPort").removeAttr("readonly");
            $("#ThirdPartyPort").attr("readonly", "readonly");
        }
        $("#ThirdPartyPort").val("");
    });

    $("input[name=btnSubmitMA]").unbind("click").click(function () {
        event.preventDefault();
        $ma_global_var = {};

        if (GetUpdateUIData()) {
            $(this).prop("disabled", true);
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/SubmitMA",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                error: function () {
                },
                success: function (data) {
                    OnSuccess(data);
                }
            });
        }
    });

    $("input[name=btnCompleteMA]").unbind("click").click(function () {
        event.preventDefault();

        if (!checkSN())
            return false;

        if (GetUpdateUIData()) {
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/CompleteMA",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                error: function () {
                },
                success: function (data) {
                    OnSuccess(data);
                }
            });
        }
    });

    $("input[name=btnWorkingMA]").unbind("click").click(function () {
        event.preventDefault();
        if (GetUpdateUIData()) {
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/WorkingMA",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                dataType: 'json',
                error: function () {
                },
                success: function (data) {
                    OnSuccess(data);
                }
            });
        }
    });

    $("input[name=btnAdviseMSLMA]").unbind("click").click(function () {
        event.preventDefault();

        if (!checkIRemarks(MAMSG.REJECT_I_REMARKS))
            return false;

        if (GetUpdateUIData()) {
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/AdviseMSLMA",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                dataType: 'json',
                error: function () {
                },
                success: function (data) {
                    OnSuccess(data);
                }
            });
        }
    });

    $("input[name=btnTOTALLOSSMA]").unbind("click").click(function () {
        event.preventDefault();
        if (GetUpdateUIData()) {
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/TOTALLOSSMA",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                dataType: 'json',
                error: function () {
                },
                success: function (data) {
                    OnSuccessTTL(data);
                }
            });
        }
    });

    $("input[name=btnRevertTL]").unbind("click").click(function () {
        event.preventDefault();
        if (GetUpdateUIData()) {
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/RevertTotalLoss",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                dataType: 'json',
                error: function () {
                },
                success: function (data) {
                    OnSuccess(data);
                }
            });
        }
    });

    $("input[name=btnAdviseEMRMGRMA]").unbind("click").click(function () {
        event.preventDefault();

        if (!checkIRemarks(MAMSG.REJECT_I_REMARKS))
            return false;
        if (GetUpdateUIData()) {
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/AdviseEMRMGRMA",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                dataType: 'json',
                error: function () {
                },
                success: function (data) {
                    OnSuccess(data);
                }
            });
        }
    });

    $("input[name=btnReject2ApproverMA]").unbind("click").click(function () {
        event.preventDefault();

        if (!checkIRemarks(MAMSG.REJECT_I_REMARKS))
            return false;
        if (GetUpdateUIData()) {
            var errMsgR = "";
            if ($("#IRemarks").val() == "" || $("#IRemarks").val().length < 1)
                errMsgR = MAMSG.REJECT_I_REMARKS + "</br>";

            if (errMsgR != "") {
                var sChar = "</br>";
                errMsgR = errMsgR.slice(0, -sChar.length); //slice off last <br> characters here
                appendMsg(errMsgR, MESSAGETYPE.ERROR);
            }
            else {
                $.ajax({
                    url: "/ManageWorkOrder/ManagerApproval/Reject2MSL",
                    traditional: true,
                    data: $ma_global_var,
                    type: 'POST',
                    dataType: 'json',
                    error: function () {
                    },
                    success: function (data) {
                        OnSuccess(data);
                    }
                });
            }
        }
    });

    $("input[name=btnRejectMA]").unbind("click").click(function () {
        event.preventDefault();

        if (!checkERemarks(MAMSG.REJECT_E_REMARKS))
            return false;

        if (GetUpdateUIData()) {
            var errMsgR = "";
            if ($("#ERemarks").val() == "" || $("#ERemarks").val().length < 2)
                errMsgR = MAMSG.REJECT_E_REMARKS + "</br>";

            if (errMsgR != "") {
                var sChar = "</br>";
                errMsgR = errMsgR.slice(0, -sChar.length); //slice off last <br> characters here
                appendMsg(errMsgR, MESSAGETYPE.ERROR);
            }
            else {
                $.ajax({
                    url: "/ManageWorkOrder/ManagerApproval/RejectTo100",
                    traditional: true,
                    data: $ma_global_var,
                    type: 'POST',
                    dataType: 'json',
                    error: function () {
                    },
                    success: function (data) {
                        OnSuccess(data);
                    }
                });
            }
        }
    });

    $("input[name=btnFwd2EMRMA]").unbind("click").click(function () {
        event.preventDefault();
        if (!checkIRemarks(MAMSG.FORWARD_I_REMARKS))
            return false;
        if (GetUpdateUIData()) {
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/Fwd2EMRMA",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                dataType: 'json',
                error: function () {
                },
                success: function (data) {
                    OnSuccess(data);
                }
            });
        }
    });

    $("input[name=btnFwd2CENEQULOSMA]").unbind("click").click(function () {
        event.preventDefault();

        if (!checkIRemarks(MAMSG.FORWARD_I_REMARKS))
            return false;
        if (GetUpdateUIData()) {
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/Fwd2CENEQULOSMA",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                dataType: 'json',
                error: function () {
                },
                success: function (data) {
                    OnSuccess(data);
                }
            });
        }
    });

    $("input[name=btnSuspendMA]").unbind("click").click(function () {
        event.preventDefault();

        if (!checkIRemarks(MAMSG.SUSPEND_I_REMARKS))
            return false;


        if (GetUpdateUIData()) {
            var errMsgR = "";
            if ($("#IRemarks").val() == "")
                errMsgR = MAMSG.SUSPEND_I_REMARKS + "</br>";

            if (errMsgR != "") {
                var sChar = "</br>";
                errMsgR = errMsgR.slice(0, -sChar.length); //slice off last <br> characters here
                appendMsg(errMsgR, MESSAGETYPE.ERROR);
            }
            else {
                $.ajax({
                    url: "/ManageWorkOrder/ManagerApproval/SuspendMA",
                    traditional: true,
                    data: $ma_global_var,
                    type: 'POST',
                    dataType: 'json',
                    error: function () {
                    },
                    success: function (data) {
                        OnSuccess(data);
                    }
                });
            }
        }
    });


    $("input[name=btnApproveMA]").unbind("click").click(function () {
        event.preventDefault();

        if (!chkTPP()) {
            alert(MAMSG.THIRDPARTY_MISSING);
            return false;
        }
        if (GetUpdateUIData()) {
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/ApproveMA",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                dataType: 'json',
                error: function () {
                },
                success: function (data) {
                    OnSuccess(data);
                }
            });
        }
    });

    $("input[name=btnPrint]").unbind("click").click(function () {
        event.preventDefault();
        PrintDoc();
    });

    $("input[name=btnDeleteMA]").unbind("click").click(function () {
        event.preventDefault();

        if (GetUpdateUIData()) {
            $(this).prop("disabled", true);
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/DeleteMA",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                error: function () {
                },
                success: function (data) {
                    OnSuccess(data);
                }
            });
        }
    });

    $('.traverse').unbind("click").click(function () {
        event.preventDefault();
        appendMsg(MAMSG.WAITINFO, MESSAGETYPE.INFO);
        var aType = "";
        if ($(this).attr("name") == "btnFirst")
            aType = "FIRST";
        else if ($(this).attr("name") == "btnPrev")
            aType = "PREVIOUS";
        else if ($(this).attr("name") == "btnNxt")
            aType = "NEXT";
        else if ($(this).attr("name") == "btnLast")
            aType = "LAST";

        $.ajax({
            url: "/ManageWorkOrder/ManagerApproval/GetNextWODetail",
            traditional: true,
            data: { WOID: $("#hdnWOID").val(), ACTIONTYPE: aType },
            type: 'POST',
            error: function () {
            },
            success: function (data) {
                appendMsg(data.UIMsg, data.Status);
            }
        });
    });

    $("input[name=btnUpdateMA]").unbind("click").click(function () {
        event.preventDefault();
        if (GetUpdateUIData()) {
            $.ajax({
                url: "/ManageWorkOrder/ManagerApproval/UpdateMA",
                traditional: true,
                data: $ma_global_var,
                type: 'POST',
                dataType: 'json',
                error: function () {
                },
                success: function (data) {
                    OnSuccess(data,true);
                }
            });
        }
    });
});

function GetCause() {
    var errMsg = "";
    if (Boolean($("#IsNewWO").attr("IsSingle"))) {
        if ($("#UICause").val() == "")
            errMsg = MAMSG.CAUSE_MISSING + "</br>";
        else
            $wo_global_var["CCAUSE"] = $("#UICause").val();
    }
    else {
        $ma_global_var["CCAUSE"] = $("#UICause").attr("CauseCode");
    }
    return errMsg;
}


function checkSN() {
    //serial number field format: "SN~(0)"& "~" & repairCode(1)  & "~" & partNumber(2) ==> SN~5314~XX-1001
    var bRtn = true;
    var msg = "";
    if ($('input[id^="SN||"]').length > 0) {
        $('input[id^="SN||"]').each(function () {
            var snVal = $(this).val();
            if (snVal == "") {
                msg += " Repair Code :" + $(this).attr("id").split("||")[1] + " and ";
                msg += "Part Code :" + $(this).attr("id").split("||")[2] + "\n";
                bRtn = false;
            }
        });
        if (!bRtn)
            alert(MAMSG.CORENO_MISSING + msg);
    }
    return bRtn;
}

function chkTPP() {
    var cCode = "";
    if ($("#UICause")[0].tagName == 'SELECT') {
        cCode = $("#UICause").val();
    }
    else {
        cCode = $("#UICause").attr("CauseCode");
    }

    if (cCode == "3" && $("#ThirdPartyPort").val() != "")
        return true;
    else if (cCode == "3" && $("#ThirdPartyPort").val() == "")
        return false;
    else
        return true;
}


function GetUpdateUIData() {
    $ma_global_var = {};
    appendMsg(MAMSG.WAITINFO, MESSAGETYPE.INFO);
    //Debadrita_TPI_Indicator-19-07-19--start
    $ma_global_var["NEWTPICODE"] = "";
    $ma_global_var["REPRCODE"] = "";
    var errMsg = "", cCode = "", newTPI = "";
    $(document).ready(function () {
        $('#tblRPAdditionalchildRows tr').each(function () {

            var idname = this.id.toString();
            if (idname == "trbody") {
                var tpval1 = ($(this).find("#drpTPIList")).val();
                if (tpval1 != "" || typeof tpval1.trim() != 'undefined') {

                    $ma_global_var["NEWTPICODE"] += tpval1 + ",";

                    var rpval = ($(this).find("#tdrepair")).text();
                    $ma_global_var["REPRCODE"] += rpval + ",";


                }

            }

        })
    });//Debadrita_TPI_Indicator-19-07-19--end

   
    $ma_global_var["WOID"] = $("#hdnWOID").val();

    if ($("#UICause").length > 0 && $("#UICause")[0].tagName == 'SELECT') {
        cCode = $("#UICause").val();
    }
    else {
        cCode = $("#UICause").attr("CauseCode");
    }

    //if (cCode > 2 && $("#ThirdPartyPort").val() == "") {
    //    errMsg = MAMSG.THIRDPARTY_MISSING + "</br>";
    //}
    //else {
    $ma_global_var["TPP"] = $("#ThirdPartyPort").val();
    //}

    $ma_global_var["CAUSE"] = cCode;
    $ma_global_var["TPP"] = $("#ThirdPartyPort").val();
    $ma_global_var["EREMARKS"] = ($("#ERemarks").length > 0 ? $("#ERemarks").val() : "");
    $ma_global_var["IREMARKS"] = ($("#IRemarks").length > 0 ? $("#IRemarks").val() : "");

    if ($('input[id^="SN||"]').length > 0) {
        $('input[id^="SN||"]').each(function () {
            var snVal = $(this).val();
            if (snVal != "") {
                if ($ma_global_var["SERIALNO"] != "") {
                    $ma_global_var["SERIALNO"] += "," + snVal;
                    $ma_global_var["REPCODE"] += "," + $(this).attr("id").split("||")[1];
                    $ma_global_var["PARTNO"] += "," + $(this).attr("id").split("||")[2];
                }
                else {
                    $ma_global_var["SERIALNO"] = snVal;
                    $ma_global_var["REPCODE"] = $(this).attr("id").split("||")[1];
                    $ma_global_var["PARTNO"] = $(this).attr("id").split("||")[2];
                }
            }
        });
    }
    else {
        $ma_global_var["SERIALNO"] = "";
        $ma_global_var["REPCODE"] = "";
        $ma_global_var["PARTNO"] = "";
    }

    $ma_global_var["REPDATE"] = $("#txtCompletionDate").val();
    $ma_global_var["VENREF"] = $("#VenRef").attr("refCode");

    event.preventDefault();
    if (errMsg != "") {
        var sChar = "</br>";
        errMsg = errMsg.slice(0, -sChar.length); //slice off last <br> characters here
        appendMsg(errMsg, MESSAGETYPE.ERROR);
        return false;
    }
    else
        return true;
}


//function GetUpdateUIData() {
//    $ma_global_var = {};
//    appendMsg(MAMSG.WAITINFO, MESSAGETYPE.INFO);
//    var errMsg = "", cCode = "";
//    $ma_global_var["WOID"] = $("#hdnWOID").val();

//    if ($("#UICause").attr('type') == 'select') {
//        cCode = $("#UICause").val();
//    }
//    else {
//        cCode = $("#UICause").attr("CauseCode");
//    }

//    if (cCode > 2 && $("#ThirdPartyPort").val() == "") {
//        errMsg = MAMSG.THIRDPARTY_MISSING + "</br>";
//    }
//    else {
//        $ma_global_var["TPP"] = $("#ThirdPartyPort").val();
//    }

//    $ma_global_var["CAUSE"] = cCode;
//    $ma_global_var["TPP"] = $("#ThirdPartyPort").val();
//    $ma_global_var["EREMARKS"] = ($("#ERemarks").length > 0 ? $("#ERemarks").val() : "");
//    $ma_global_var["IREMARKS"] = ($("#IRemarks").length > 0 ? $("#IRemarks").val() : "");
//    if ($('input[id^="SN~"]').length > 0) {
//        var sNo = $('input[id^="SN~"]').val();
//        $ma_global_var["SERIALNO"] = sNo;
//        $ma_global_var["REPCODE"] = sNo.split("~")[2];
//        $ma_global_var["PARTNO"] = sNo.split("~")[3];
//    }
//    else {
//        $ma_global_var["SERIALNO"] = "";
//        $ma_global_var["REPCODE"] = "";
//        $ma_global_var["PARTNO"] = "";
//    }
//    $ma_global_var["REPDATE"] = $("#txtCompletionDate").val();

//    event.preventDefault();
//    if (errMsg != "") {
//        var sChar = "</br>";
//        errMsg = errMsg.slice(0, -sChar.length); //slice off last <br> characters here
//        appendMsg(errMsg, MESSAGETYPE.ERROR);
//    }
//    else {
//        $.ajax({
//            url: "/ManageWorkOrder/ManagerApproval/UpdateGeneralValues",
//            traditional: true,
//            data: $ma_global_var,
//            type: 'POST',
//            error: function () {
//            },
//            success: function (data) {
//                $("#tdRemarksSpace").html(data.RemarksData);
//                appendMsg(data.UIMsg, data.Status);
//            }
//        });
//    }
//}

function checkERemarks(msg) {
    if ($("#ERemarks").val() == "") {
        alert(msg);
        return false;
    }
    else if ($("#ERemarks").val().length > 255) {
        alert(MAMSG.REMARKS_SIZEEXCEED);
        return false;
    }
    else
        return true;
}

function checkIRemarks(msg) {
    if ($("#IRemarks").val().length < 2) {
        alert(msg);
        return false;
    }
    else if ($("#IRemarks").val().length > 255) {
        alert(MAMSG.REMARKS_SIZEEXCEED);
        return false;
    }
    else
        return true;
}

function navigate(target) {
    //Perform your AJAX call to your Controller Action
    $.post(target, { WOID: $('#UserName').val(), ACTIONTYPE: $('#Password').val() });
}

function showShopDetails(sCode) {
    getModelBody(true);
    var pTitle = "";
    pTitle = "Shop Details";
    $(".modal-title").html(pTitle);
    $("#dialog").load("./ManageMasterData/ManageMasterShopVendor/ShopDetailsByShopCode", { RecordID: sCode });
}

function toggleFont(obj) {
    if ($(obj).val() == "Font <") {
        $(obj).val("Font >");
        $('body').css('font-size', '10px');
    }
    else {
        $(obj).val("Font <");
        $('body').css('font-size', '12px');
    }
}
function OnSuccessTTL(data, frmUpdate) {
  
    if (data.Status == "SUCCESS") {
        if (data.isRedirect) {
            //alert(data.UIMsg);
            appendMsg(data.UIMsg, MESSAGETYPE.INFO);
            window.location.href = data.redirectUrl;
        }
        else {
            $("#tdRemarksSpace").html(data.RemarksData);
            if (data.UIMsg == "")
                appendMsg(MAMSG.REQUEST_SUCCESS, data.Status);
            else {
                appendMsg(data.UIMsg, data.Status);
               // alert("Info: " + data.UIMsg);
            }

            if (frmUpdate == undefined || !frmUpdate) {
                $('[name^="btn"]').attr("disabled", "disabled");
                $('[name=btnFont],[name=btnPrint],[name=btnNewQuery],[name=btnUpdateMA],input[name=btnAuditTrail]').removeAttr("disabled");
            }
        }
    }
     
    else {
        $("#tdRemarksSpace").html(data.RemarksData);
        appendMsg(data.UIMsg, data.Status);
        var str = data.UIMsg;
      
       {
               alert(str);
            if (data.isRedirect) {
                // alert(data.UIMsg);
                appendMsg(data.UIMsg, MESSAGETYPE.WARNING);
                window.location.href = data.redirectUrl;
            }
        }
    }
}
function OnSuccess(data, frmUpdate) {
    if (data.Status == "SUCCESS") {
        if (data.isRedirect) {
            appendMsg(MAMSG.UI_RELOAD, MESSAGETYPE.INFO);
            window.location.href = data.redirectUrl;
        }
        else {
            $("#tdRemarksSpace").html(data.RemarksData);
            if (data.UIMsg == "")
                appendMsg(MAMSG.REQUEST_SUCCESS, data.Status);
            else
                appendMsg(data.UIMsg, data.Status);

            if (frmUpdate == undefined || !frmUpdate) {
                $('[name^="btn"]').attr("disabled", "disabled");
                $('[name=btnFont],[name=btnPrint],[name=btnNewQuery],[name=btnUpdateMA],input[name=btnAuditTrail]').removeAttr("disabled");
            }	    
        }
    } else {
        $("#tdRemarksSpace").html(data.RemarksData);
        appendMsg(data.UIMsg, data.Status);
    }
}