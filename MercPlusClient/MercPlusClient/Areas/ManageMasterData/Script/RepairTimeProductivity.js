

function View(val) {
    if (val == "C") {
        $("#ErrorMsgContainer").html("");
        $('#divCountry').show();
        $('#divLocation').hide();
        $('#divShop').hide();
        var Location = document.getElementById("drpCOCL").value;
        if (Location.length > 0) {
            $("#drpCOCL option:selected").remove();
        }
        var Shop = document.getElementById("drpShop");
        if (Shop.length > 0) {
            $("#drpShop option:selected").remove();
        }
        
    }
    else if (val == "L") {
        $("#ErrorMsgContainer").html("");
        $('#divLocation').show();
        $('#divCountry').hide();
        $('#divShop').hide();
        var Country = document.getElementById("drpCountry").value;
        if (Country.length > 0) {
            $("#drpCountry option:selected").remove();
        }
        var Shop = document.getElementById("drpShop").value;
        if (Shop.length > 0) {
            $("#drpShop option:selected").remove();
        }
        GetLocation();
    }
    else if (val == "S") {
        $("#ErrorMsgContainer").html("");
        $('#divShop').show();
        $('#divCountry').hide();
        $('#divLocation').hide();
        var Location = document.getElementById("drpCOCL").value;
        if (Location.length > 0) {
            $("#drpCOCL option:selected").remove();
        }
        var x = document.getElementById("drpCountry").value;
        if (x.length > 0) {
            $("#drpCountry option:selected").remove();
        }
        
    }
}

function ClientValidationCountry() {

    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");

    HighlightInputsForError($("#txtDateFrom"), false)
    var val7 = document.getElementById("txtDateFrom").value;
    if (val7.length < 1) {
        isError = true;
        errMsg = "Please mention From Date ";
        HighlightInputsForError($("#txtDateFrom"), isError)
    }

    HighlightInputsForError($("#txtDateTo"), false)
    var val7 = document.getElementById("txtDateTo").value;
    if (val7.length < 1) {
        isError = true;
        if (errMsg.trim() == "")
            errMsg = "Please mention To Date ";
        else
            errMsg += ",To Date. ";
        HighlightInputsForError($("#txtDateTo"), isError)
    }

    HighlightInputsForError($("#drpCountry"), false)
    if (document.getElementById('drpCountry').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Country.";
        HighlightInputsForError($("#drpCountry"), isError)
    }
  
    if (isError == true) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}

function ClientValidationLocation() {

    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");

    HighlightInputsForError($("#txtDateFrom"), false)
    var val7 = document.getElementById("txtDateFrom").value;
    if (val7.length < 1) {
        isError = true;
        errMsg = "Please mention From Date ";
        HighlightInputsForError($("#txtDateFrom"), isError)
    }

    HighlightInputsForError($("#txtDateTo"), false)
    var val7 = document.getElementById("txtDateTo").value;
    if (val7.length < 1) {
        isError = true;
        if (errMsg.trim() == "")
            errMsg = "Please mention To Date ";
        else
            errMsg += ",To Date. ";
        HighlightInputsForError($("#txtDateTo"), isError)
    }
    HighlightInputsForError($("#drpCOCL"), false)
    if (document.getElementById('drpCOCL').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Location.";
        HighlightInputsForError($("#drpCOCL"), isError)
    }

    if (isError == true) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}

function ClientValidationShop() {

    var errMsg = "";
    var isError = false;
    $("#ErrorMsgContainer").html("");

    HighlightInputsForError($("#txtDateFrom"), false)
    var val7 = document.getElementById("txtDateFrom").value;
    if (val7.length < 1) {
        isError = true;
        errMsg = "Please mention From Date ";
        HighlightInputsForError($("#txtDateFrom"), isError)
    }

    HighlightInputsForError($("#txtDateTo"), false)
    var val7 = document.getElementById("txtDateTo").value;
    if (val7.length < 1) {
        isError = true;
        if (errMsg.trim() == "")
            errMsg = "Please mention To Date ";
        else
            errMsg += ",To Date. ";
        HighlightInputsForError($("#txtDateTo"), isError)
    }
    HighlightInputsForError($("#drpShop"), false)
    if (document.getElementById('drpShop').selectedIndex == 0) {
        isError = true;
        errMsg += "Please Select a Shop.";
        HighlightInputsForError($("#drpShop"), isError)
    }

    if (isError == true) {
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    else {
        return true;
    }
}

function GetLocation() {
    window.location = "/ManageMasterData/ManageMasterData/ViewAllDetailsRepairTimeProductivity";
    //$("#radLocation").bind("change", function () {
    // var c = $("#radLocation").val();
    // var a = JSON.stringify({ customerId: $(this).val() };

    ////$('#drpCOCL').append($('<option>', {
    ////    value: "-1",
    ////    text: "Loading Locations...",
    ////}));
        //$.ajax({
        //    url: "/ManageMasterData/ManageMasterData/GetAllDetailsAccToLocation",
        //    type: 'POST',
        //    datatype: 'json',
        //    data: { id: c },
        //    cache: false,
        //    success: function (data) {
            //    $('#drpCOCL').empty();
            //    if (data != null) {
            //        $.each(data, function (i, item) {
            //            $('#drpCOCL').append($('<option>', {
            //                value: item,
            //                text: item
            //            }));
            //        });
            //    }
            //    else {
            //        $('#drpCOCL').append($('<option>', {
            //            value: "-1",
            //            text: "[No locations found]",
            //        }));
             //   },
                //while (data.length > 0) {
                //    var ddl = document.getElementById('drpCOCL')
                //    for (var i = 0; i < data.length - 1; i = i + 2) {
                //        var opt = document.createElement('data');
                //        opt.text = data[i];
                //        opt.value = data[i + 1];
                //        ddl.appendChild(opt);

                //    }
                //}
        //    },
            //error: function (data) {
        //        $('#drpCOCL').empty();
        //        $('#drpCOCL').append($('<option>', {
        //            value: "-1",
        //            text: "[Too many records found...]",
        //        }));
           // }

        //});
}

function GetShop() {
    var c = $("#radLocation").val();
    $.ajax({
        url: "/ManageMasterData/ManageMasterData/GetAllDetailsAccToShop",
        type: 'POST',
        datatype: 'json',
        data: { id: c },
        cache: false,
        success: function (data) {
            $('#drpShop').empty();
            if (data != null) {
                $.each(data, function (i, item) {
                    $('#drpShop').append($('<option>', {
                        value: item,
                        text: item
                    }));
                });
            }
            else {
                $('#drpShop').append($('<option>', {
                    value: "-1",
                    text: "[No shops found]",
                }));
            }
        while (data.length > 0) {
            var ddl = document.getElementById('drpShop')
            for (var i = 0; i < data.length - 1; i = i + 2) {
                var opt = document.createElement('data');
                opt.text = data[i];
                opt.value = data[i + 1];
                ddl.appendChild(opt);

            }
        }
            },
            error: function (data) {
                $('#drpShop').empty();
                $('#drpShop').append($('<option>', {
                    value: "-1",
                    text: "[Too many records found...]",
                }));
            }

});
}


