

$(function () {
    $("#lstEqpTypeList").change(function () {
        $("#lstEqpSubTypeList").empty();
        var procemessage = "<option value='0'> Please wait...</option>";
        $("#lstEqpSubTypeList").html(procemessage).show();

        var url = 'FillEquipmentSubType';
        $.post(url, { EqpType: $('#lstEqpTypeList').val() }, function (data) {    //ajax call
            var items = [];
            items.push("<option value=" + "" + ">" + "ALL" + "</option>"); //first item
            for (var i = 0; i < data.length; i++) {
                items.push("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
            }                                         //all data from the team table push into array
            $("#lstEqpSubTypeList").html(items.join(' '));

        })                                            //array object bind to dropdown list
    });


    $("#lstShopList").change(function () {
        $("#lstCustList").empty();
        var procemessage = "<option value='0'> Please wait...</option>";
        $("#lstCustList").html(procemessage).show();
        var url = 'FillCustomerList';
        $.post(url, { ShopCode: $('#lstShopList').val() }, function (data) {    //ajax call
            var items = [];
            items.push("<option value=" + 0 + ">" + "ALL" + "</option>"); //first item
            for (var i = 0; i < data.length; i++) {
                items.push("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
            }                                         //all data from the team table push into array
            $("#lstCustList").html(items.join(' '));
        })                                            //array object bind to dropdown list
    });


    $('#search').click(function (e) {

        e.preventDefault();
        $('#lblErrorMesage').css("display", "none");
        $('#lblMesage').css("display", "none");
        $("#ErrorMsgContainer").html("");
        if (($('#lstQueryTypeList').val().length > 30 && $('#txtEqpNo').val() == '' && $('#txtVenRefNo').val() == '')) {

            $('#lblErrorMesage').text('When selecting a Query Type of "ALL" you must specify either the Equipment No. or Vendor Ref No.');
            $('#lblErrorMesage').css("display", "block");
            $('#gridContent').css("display", "none");

        }


        else {
            if (document.getElementById("txtFromDate").value != "" || document.getElementById("txtToDate").value != "") {
                if (checkDates()) {
                    SearchWo();
                }
            }
            else {
                SearchWo();
            }
        }

    })
})

function SearchWo() {
    $('#lblErrorMesage').css("display", "none");
    $('#lblMesage').css("display", "none");
    $('#gridContent').css("display", "block");
    $('#gridContent').html("<text style='color:Red;'>Loading...<text>");
    //if ($('#hdSession').val().toString() == "MSL" || $('#hdSession').val().toString() == "CPH") {
    //    $('#tdCocl').css("display", "block");
    //    $('#tdCountry').css("display", "block");
    //}
    var url = 'WOSearch';
    $.post(url, {
        ShopCode: $('#lstShopList').val(),
        FromDate: $('#txtFromDate').val(),
        ToDate: $('#txtToDate').val(),
        CustomerCD: $('#lstCustList').val(),
        EqpSize: $('#lstEqpSizeList').val(),
        EqpType: $('#lstEqpTypeList').val(),
        qpSubType: $('#lstEqpSubTypeList').val(),
        Mode: $('#lstModeList').val(),
        EquipmentNo: $('#txtEqpNo').val(),
        VenRefNo: $('#txtVenRefNo').val(),
        COCL: $('#txtCOCL').val(),
        Country: $('#txtCountry').val(),
        Location: $('#txtLocation').val(),
        QueryType: $('#lstQueryTypeList').val(),
        SortBy: $('#lstSortList').val()

    }, function (result) {
        if (result != "") {
            $('#hdnSearchRes').val(result);
            $('#gridContent').html(result);
        }
        else {
            $('#hdnSearchRes').val("");
            $('#lblMesage').text('**** NO RECORDS FOUND MATCHING SEARCH CRITERIA **** ');
            $('#lblMesage').css("display", "block");

        }
    });
}

function gridTojson() {

    var json = '';
    var $ccol = [];
    $("#grdWO tbody tr").each(function (i, row) {
        var $tRow = $(row);

        if ($tRow.find('input[type=checkbox]').prop('checked') == true) {


            $ccol.push($tRow.find('input[type=checkbox]').val());
        }
    });

    json += $ccol.join(",") + '';
    return json;
}
function getDateJson() {
    var json = '';
    var $ccol = [];
    $("#grdWO tbody tr").each(function (i, row) {
        var $tRow = $(row);

        if ($tRow.find('input[type=checkbox]').prop('checked') == true) {


            $ccol.push($tRow.find('input[type=textbox]').val());
        }
    });

    json += $ccol.join(",") + '';
    return json;
}


/*--This JavaScript method for Print command--*/

function checkDates() {
    //alert("check date called")
    //if((form.txtdatefrom.value=="")&&(form.txtdateto.value==""))
    //{
    //   alert("Please mention To Date & From Date");

    //   return false
    //}
    if ((document.getElementById("txtFromDate").value == "") && (document.getElementById("txtToDate").value != "")) {
        $('#lblErrorMesage').text('Please mention From date also');
        $('#lblErrorMesage').css("display", "block");
        document.getElementById("txtFromDate").focus();
        return false
    }
    else if ((document.getElementById("txtToDate").value == "") && (document.getElementById("txtFromDate").value != "")) {
        $('#lblErrorMesage').text('Please mention To Date also');
        $('#lblErrorMesage').css("display", "block");
        document.mgrQuery.txtdateto.focus();
        return false
    }



    return true
}


function PrintDoc() {
    
    var toPrint = document.getElementById('divMain');

    var popupWin = window.open('', '_blank', 'width=700,height=800,location=no,left=200px');

    popupWin.document.open();

    popupWin.document.write('<html><title>::Print Preview::</title><link rel="stylesheet" type="text/css" href="Print.css" media="screen"/></head><body">')
   
    popupWin.document.write(toPrint.innerHTML);

    popupWin.document.write('</html>');

    popupWin.document.close();

}



/*--This JavaScript method for Print Preview command--*/

function PrintPreview() {
    
    var toPrint = document.getElementById('divMain');

    var popupWin = window.open('', '_blank', 'width=600px,height=700px,location=no,left=200px');


    popupWin.document.open();

    popupWin.document.write('<html><title>::Print Preview::</title><link rel="stylesheet" type="text/css" href="Print.css" media="screen"/></head><body">')
   
    popupWin.document.write(toPrint.innerHTML);

    popupWin.document.write('</html>');

    popupWin.document.close();

}

function PrintReviewEstimate() {
    var panel = document.getElementById('divMain');
    var printWindow = window.open('', '_blank', 'scrollbars=yes,menubar=yes,toolbar=yes,resizable=yes,titlebar=yes');
    printWindow.document.write('<html><head><title>Review Estimates</title>');
    printWindow.document.write('</head><body >');
    printWindow.document.write(panel.innerHTML);
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    //setTimeout(function () {
    //    printWindow.print();
    //}, 500);
    return false;
}

function ReloadSearchData() {
    if ($.trim($("#hdnSearchRes").val()) != "") {
        $('#gridContent').html($("#hdnSearchRes").val());
        $("#hdnSearchRes").val($.trim($("#hdnSearchRes").val()));
    }
}

