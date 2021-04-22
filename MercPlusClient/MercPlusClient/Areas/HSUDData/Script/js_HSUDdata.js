

$(function () {
    


    $('#search').click(function (e) {

        e.preventDefault();
        $('#lblErrorMesage').css("display", "none");
        $('#lblMesage').css("display", "none");
        $("#ErrorMsgContainer").html("");
        if (  $('#txtEqpNo').val() == ''  ) {

            $('#lblErrorMesage').text('Please Select Equipment Number');
            $('#lblErrorMesage').css("display", "block");
            $('#gridContent').css("display", "none");

        }


            else {
                SearchWo();
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
   // alert($('#txtEqpNo').val());
    var url = 'HSUDDataSearch';
    $.post(url, {
       
        EquipmentNo: $('#txtEqpNo').val()
       

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



function ReloadSearchData() {
    if ($.trim($("#hdnSearchRes").val()) != "") {
        $('#gridContent').html($("#hdnSearchRes").val());
        $("#hdnSearchRes").val($.trim($("#hdnSearchRes").val()));
    }
}

function GetHSUDDetails() {
    $('#lblErrorMesage').css("display", "none");
    $('#lblMesage').css("display", "none");
    $('#gridContent').css("display", "block");
    $('#gridContent').html("<text style='color:Red;'>Loading...<text>");
    
    var url = 'GetHSUDDetails';
    $.post(url, {

        EquipmentNo: $('#txtEqpNo').val()


    }, function (result) {
        if (result != "") {
            $('#hdnSearchRes1').val(result);
            $('#gridContent').html(result);
        }
        else {
            $('#hdnSearchRes1').val("");
            $('#lblMesage').text('**** NO RECORDS FOUND MATCHING SEARCH CRITERIA **** ');
            $('#lblMesage').css("display", "block");

        }
    });
}
