var MESSAGETYPE = { SUCCESS: 'SUCCESS', ERROR: 'ERROR', CLEAR: 'CLEAR', WARNING: 'WARNING', INFO: 'INFO' };

$(function () {
    function CallAjax(obj) {
        var method = obj.method || 'GET'; // 'GET' is the default here
        // more variables...
        $.ajax({
            url: obj.url,
            cache: false,
            method: method,
            dataType: type,
            async: async,
            success: function (data) {
                return data;
            },
            error: function (data) {
                return "error " + data;
            }
        });
    }

    function MsgDialogue(obj) {
        $('#' + obj.ID).dialog({
            autoOpen: true,
            resizable: false,
            title: obj.title,
            modal: true,
            width: ((obj.width == undefined || obj.width == null) ? 300 : obj.width),
            height: ((obj.height == undefined || obj.height == null) ? 200 : obj.height),
            buttons: [
                {
                    text: "Ok",
                    click: function () {
                        $(this).dialog("close");
                    }
                }
            ]
        });
    }

    function ConfirmDialogue(obj) {
        $('#' + obj.ID).dialog({
            autoOpen: true,
            resizable: false,
            title: obj.title,
            modal: true,
            width: ((obj.width == undefined || obj.width == null) ? 300 : obj.width),
            height: ((obj.height == undefined || obj.height == null) ? 200 : obj.height),
            buttons: [
                {
                    text: "Ok",
                    click: function () {
                        $(this).dialog("close");
                        return true;
                    }
                },
                {
                    text: "Cancel",
                    click: function () {
                        $(this).dialog("close");
                        return false;
                    }
                }
            ]
        });
    }

    function ContentDialogue(obj) {
        $('#' + obj.ID).dialog({
            autoOpen: true,
            resizable: false,
            title: obj.title,
            modal: true,
            width: ((obj.width == undefined || obj.width == null) ? 600 : obj.width),
            height: ((obj.height == undefined || obj.height == null) ? 600 : obj.height)
        });
    }

    function SuccessError(hasError, msg) {
        if (hasError == MESSAGETYPE.SUCCESS) {

            if ($("#divMessage .ui-corner-all").hasClass("ui-state-error")) {
                $("#divMessage .ui-corner-all").addClass("ui-state-highlight").removeClass("ui-state-error");
            }

            if ($("#divMessage .ui-icon").hasClass("ui-icon-alert")) {
                $("#divMessage .ui-icon").addClass("ui-icon-info").removeClass("ui-icon-alert");
            }
            $("#divMessage .ui-icon").html(msg);
            $("#divMessage").show();
        }
        else if (hasError == MESSAGETYPE.ERROR) {

            if ($("#divMessage .ui-corner-all").hasClass("ui-state-highlight")) {
                $("#divMessage .ui-corner-all").addClass("ui-state-error").removeClass("ui-state-highlight");
            }
            if ($("#divMessage .ui-icon").hasClass("ui-icon-info")) {
                $("#divMessage .ui-icon").addClass("ui-icon-alert").removeClass("ui-icon-info");
            }
            $("#divMessage .ui-icon").html(msg);
            $("#divMessage").show(msg);
        }
        else {
            $("#divMessage").hide();
        }
    }

    $('.CLSNUMERIC').unbind("keydown").on('keydown', function (evt) {
        return NumKeyDown(this, evt);
    });

    $('.CLSDECIMAL').unbind("keydown").on('keydown', function (evt) {
        return DecKeyDown(this, evt);
    });

    $(".CLSNUMERIC,.CLSDECIMAL").unbind("blur").on('blur', function () {
        return NumDecBlur(this);
    });

    $(".CLSNUMERIC").unbind("focus").on('focus', function () {
        return NumFocus(this);
    });

    $(".CLSDATE").datepicker({
        autoclose: true
    });

    $(".CLSDTFROM").datepicker({
        autoclose: true
    });
    $(".CLSDTTO").datepicker({
        autoclose: true
    });

    $(document).on("keydown", "#MainContent", function (e) {
        if ((e.keyCode == 10 || e.keyCode == 13) && e.ctrlKey) {
            //implement return from btnreview if true then call submit
            $(document).find("input[name=btnReview]").click();
        }
    });

    //$('select').each(function() {
    //    $(this).prepend('<option>***Any***</option>');
    //});
});

function GetDate(jDate) {
    var minDate = new Date(0001, 01, 01);
    if (jDate == undefined || jDate == "")
        return "";
    var value = new Date(parseInt(jDate.replace(/(^.*\()|([+-].*$)/g, '')));
    if (isNaN(value.getDate()))
        return "";
    else {
        var retDate = (value.getFullYear() + "-" + (value.getMonth() + 1) + "-" + value.getDate());
        if (value <= minDate)
            return "";
        else
            return retDate;
    }
}



//displayMessageOnUI
function appendMsg(varMsg, actionType) {
    var contId = $("#ErrorMsgContainer");
    $(contId).empty();
    $(contId).html(varMsg);
    addErrorClass(contId, actionType);
    UpScroll();
}

function clearError() {
    var contId = $("#ErrorMsgContainer");
    $(contId).empty();
    addErrorClass(contId, MESSAGETYPE.CLEAR);
}
function addErrorClass(contId, actionType) {
    if ($.trim($(contId).html()) != "") {
        if (actionType == MESSAGETYPE.WARNING) {
            if (!$(contId).hasClass("alert alert-warning")) {
                $(contId).addClass("alert alert-warning");
            }
        }
        else if (actionType == MESSAGETYPE.ERROR) {
            if (!$(contId).hasClass("alert alert-danger")) {
                $(contId).addClass("alert alert-danger");
            }
        }
        else if (actionType == MESSAGETYPE.SUCCESS) {
            if (!$(contId).hasClass("alert alert-success")) {
                $(contId).addClass("alert alert-success");
            }
        }
        else if (actionType == MESSAGETYPE.INFO) {
            if (!$(contId).hasClass("alert alert-info")) {
                $(contId).addClass("alert alert-info");
            }
        }
    }
    else {
        $(contId).removeClass("alert alert-danger alert-success alert-warning alert-info", 200);

    }
}

function HighlightError(objThis, iserror) {
    //if (iserror)
    //    $(objThis).css("background-color", "yellow");
    //else
    //    $(objThis).css("background-color", "#fff");
}

function ShowRemoveValidationMessage(varMsg, actionType) {
    var errdiv = $("#ErrorMsgContainer");
    $(errdiv).html("");
    var Htmlitems = '';
    if (varMsg != "") {
        if (actionType == "Success") {
            Htmlitems = "</br><div class='alert alert-success' style='width: 750px; vertical-align: text-top;'> <strong>Success!</strong> " + varMsg + "</div>";
        }
        else if (actionType == "Warning") {
            Htmlitems = "</br><div class='alert alert-warning' style='width: 750px; vertical-align: text-top;'> <strong>Warning!</strong> " + varMsg + "</div>";
        }
        else {
            Htmlitems = "</br><div class='alert alert-info' style='width: 750px; vertical-align: text-top;'> <strong> " + varMsg + "</strong></div>";
        }
    }
    $(errdiv).append(Htmlitems);
}
function HighlightInputsForError(objThis, isError) {
    if (isError)
        $(objThis).css("background-color", "#FFFF33");
    else
        $(objThis).css("background-color", "#fff");
}
function ClearHighlightErrorForInputs(objContainer, isError) {
    $(objContainer).find('input, select, textarea').each(function () {
        if (this.type == "text") { HighlightInputsForError(this, isError); }
        else if (this.type == "textarea") { HighlightInputsForError(this, isError); }
        else if (this.type == "select") { HighlightInputsForError(this, isError); }
    });
}
$(function () {
    $('INPUT[type="text"]').blur(function () { HighlightInputsForError(this, false) });
    $('INPUT[type="textarea"]').blur(function () { HighlightInputsForError(this, false) });
    //$('INPUT[type="select"]').selected(function () { HighlightInputsForError(this, false) });

});

function are_arrs_equal(arr1, arr2) {
    return arr1.sort().toString() === arr2.sort().toString()
}

function compareArrays(arr1, arr2) {
    var xtra = true;
    $.each(arr2, function (k, v) {
        if ($.inArray(v, arr1) === -1) {
            xtra = false;
            return false;
        }
    });
    return xtra;
}

/*--This JavaScript method for Print command--*/
function PrintDoc() {
    var toPrint = $("#divViewRoot");
    var popupWin = window.open('', '_blank', 'width=900px,height=1300px,location=no,left=auto,crolling=true,');
    popupWin.document.open();
    popupWin.document.write('<html><title>::Preview::</title><link rel="stylesheet" type="text/css" href="/Content/css/bootstrap.css" /></head><body onload="window.print()">');
    popupWin.document.write(toPrint.html());
    popupWin.document.write('</html>');
    popupWin.document.close();
}

/*--This JavaScript method for Print Preview command--*/
function PrintPreview() {
    var toPrint = $("#divViewRoot");
    var popupWin = window.open('', '_blank', 'width=600px,height=700px,location=no,left=200px');
    popupWin.document.open();
    popupWin.document.write('<html><title>::Print Preview::</title><link rel="stylesheet" type="text/css" href="Print.css" media="screen"/></head><body">');
    popupWin.document.write(toPrint.innerHTML);
    popupWin.document.write('</html>');
    popupWin.document.close();
}

function HighlightInputsForError(objThis, isError) {
    if (isError)
        $(objThis).css("background-color", "#FFFF33");
    else
        $(objThis).css("background-color", "#fff");
}
function ClearHighlightErrorForInputs(objContainer, isError) {
    $(objContainer).find('input, select, textarea').each(function () {
        if (this.type == "text") { HighlightInputsForError(this, isError); }
        else if (this.type == "textarea") { HighlightInputsForError(this, isError); }
        else if (this.type == "select") { HighlightInputsForError(this, isError); }
    });
}
$(function () {
    $('INPUT[type="text"]').blur(function () { HighlightInputsForError(this, false) });
    $('INPUT[type="textarea"]').blur(function () { HighlightInputsForError(this, false) });
    //$('INPUT[type="select"]').selected(function () { HighlightInputsForError(this, false) });

});

// Amlan added -- End --

function getBool(val) {
    var num = +val;
    return !isNaN(num) ? !!num : !!String(val).toLowerCase().replace(!!0, '');
}
// Amlan added -- End --

function ShowAuditTrail(mypage, pagename, TableName, RecordID, VenRefNo, EqpNo, w, h, scroll) {
    var winl = (screen.width - w) / 2;
    var wint = (screen.height - h) / 2;
    if (TableName == 'WorkOrder') {
        var val = RecordID + ',' + VenRefNo + ',' + EqpNo;

    }
        //else if (TableName == 'Country')
        //    {
        //    var val = RecordID;
        //}
    else {
        var val = RecordID;
        val = encodeURIComponent(val);

    }
    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + ',scrollbars=yes,resizable=yes'
    win = window.open(mypage + '?PageName=' + pagename + '&TableName=' + TableName + '&val=' + val, pagename, winprops)
    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
}


function getModelBody(noOkButton, alertKya) {
    var mBody = "<div class='modal fade' id='mercModal' role='dialog'>";
    mBody += "<div class='modal-dialog'>";
    mBody += "<div class='modal-content'>";
    if (!alertKya) {
        mBody += "<div class='modal-header'>";
        mBody += "<button type='button' class='close' data-dismiss='modal'>&times;</button>";
        mBody += "<h4 class='modal-title'>Modal Header</h4>";
        mBody += "</div>";
        mBody += "<div class='modal-body'>";
        mBody += "<div id='dialog'><span>Loading...</span></div>";
        mBody += "</div>";
        mBody += "<div class='modal-footer'>";
        if (noOkButton) {
            mBody += "<input type='button' data-dismiss='modal' value='Ok' id='btnModelOk' />";
            mBody += "<input type='button' data-dismiss='modal' value='Close' />";
        }
        else
            mBody += "<input type='button' data-dismiss='modal' value='Close' />";
        mBody += "</div>";
    }
    else {
        mBody += "<div class='modal-header'>";
        mBody += "<button type='button' class='close' data-dismiss='modal'>&times;</button>";
        mBody += "<h4 class='modal-title'>Alert</h4>";
        mBody += "</div>";
        mBody += "<div class='modal-body'>";
        mBody += "<div id='dialogAlert'><span>Loading...</span></div>";
        mBody += "</div>";
    }
    mBody += "</div>";
    mBody += "</div>";
    mBody += "</div>";

    return mBody;
}

function dispAlert(obj, msg) {
    $(obj).attr("data-toggle", "popover");
    $(obj).attr("data-content", msg);
    //$("#ModelContainerAlert").html(getModelBody(false, true));
    //setTimeout(function () {
    //    ($("#dialogAlert").html(msg)).fadeOut(3000);
    //}, 4000);
}

function AuditTrailShow(mypage, pagename, TableName, RecordID, w, h, scroll) {
    //var winl = (screen.width - w) / 2;
    //var wint = (screen.height - h) / 2;

    var val = RecordID;

    winprops = 'height=' + h + ',width=' + w + ',scrollbars=yes,resizable=yes';
    win = window.open(mypage + '?PageName=' + pagename + '&TableName=' + TableName + '&val=' + val, pagename, winprops);
    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
}

function UpScroll() {
    $('html, body').animate({
        scrollTop: (0)
    }, 500);
}

function showPopupWindow(mypage, pagename, RecordID, w, h) {

    var winl = (screen.width - w) / 2;
    var wint = (screen.height - h) / 2;
    var val = RecordID;
    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + ',scrollbars=no,resizable';
    win = window.open(mypage + '?PageName=' + pagename + '&RecordID=' + val, pagename, winprops);
    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
}

function NumFocus(objThis) {
    if ($(objThis).hasClass("cPieces")) {
        if ($.trim($(objThis).val()) == "" || $.trim($(objThis).val()) == "0") {
            $(objThis).val("1");
            $(objThis).select();
        }
    }
}

function NumDecBlur(objThis) {
    if ($.trim($(objThis).val()) == "" || $.trim($(objThis).val()) == null) {
        if (!$(objThis).hasClass("cPieces")) {
            if ($(objThis).hasClass('CLSNUMERIC')) {
                $(objThis).val("0");
            }
            else {
                $(objThis).val("0.0");
            }
        }
    }
}

function DecKeyDown(objThis, evt) {
    var ret = true;
    var keycode = (evt.which) ? evt.which : event.keyCode;
if (evt.altKey || evt.ctrlKey || evt.shiftKey)
        return false;
    if (keycode == 8 || keycode == 9 || keycode == 46)
        return true;
    if (((keycode >= 48 && keycode <= 57) || (keycode >= 96 && keycode <= 105)) || (keycode == 8 || keycode == 46 || keycode == 37 || keycode == 39 || keycode == 110 ||  keycode == 190)) {
        var len = $(objThis).val().length;
        var index = $(objThis).val().indexOf('.');

        if (index > 0 && (keycode == 190 || keycode == 110)) {
            return false;
        }
        if (index > 0) {
            var keyPindex = objThis.selectionStart;
            if (index >= keyPindex) {
                return true;
            }
            else {
                var CharAfterdot = (len) - (index + 1);
                if ($(objThis).hasClass("cMHours")) {
                    if (CharAfterdot > 0) {
                        return false;
                    }
                }
                else {
                    if (CharAfterdot > 1) {
                        return false;
                    }
                }
            }
        }
    }
    else {
        return false;
    }
 
    //if (keycode > 31 && keycode != 46 && (keycode < 48 || keycode > 57) && (keycode != 190)) {
    //    if (keycode == 8 || keycode == 46 || keycode == 37 || keycode == 39)
    //        return true;
    //    else
    //        return false;
    //}
    //else {
    //    var len = $(objThis).val().length;
    //    var index = $(objThis).val().indexOf('.');

    //    if (index > 0 && keycode == 190) {
    //        return false;
    //    }
    //    if (index > 0) {
    //        var keyPindex = objThis.selectionStart;
    //        if (index >= keyPindex) {
    //            return true;
    //        }
    //        else {
    //            var CharAfterdot = (len) - (index + 1);
    //            if ($(objThis).hasClass("cMHours")) {
    //                if (CharAfterdot > 0) {
    //                    return false;
    //                }
    //            }
    //            else {
    //                if (CharAfterdot > 1) {
    //                    return false;
    //                }
    //            }
    //        }
    //    }
    //}
    return ret;
}

function NumKeyDown(objThis, evt) {
    var ret = true;
    var keycode = (evt.which) ? evt.which : event.keyCode;
    if (evt.altKey || evt.ctrlKey || evt.shiftKey)
        return false;
    if (((keycode >= 48 && keycode <= 57) || (keycode >= 96 && keycode <= 105)) || (keycode == 8 || keycode == 9 || keycode == 46 || keycode == 37 || keycode == 39)) {
        return true;
    }
    else{
        return false;
    }
   
    //if (keycode > 31 && ((keycode < 48 || keycode > 57))) {
    //    if (keycode == 8 || keycode == 46 || keycode == 37 || keycode == 39)
    //        return true;
    //    else
    //        return false;
    //}
    //else {
    //    return true;
    //}
    return ret;
}