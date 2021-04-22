
    $(function () {
        $("#ddlEqType").change(function () {
            var EqType = $(this).val();
            // var a = JSON.stringify({ customerId: $(this).val() };
            $.ajax({
                url: "/ManageMasterData/ManageMasterData/SetSubTypeDetails",
                type: 'POST',
                data: { EqType: EqType },
                cache: false,
                success: function (data) {

                    var items = [];
                    items.push("<option value= >" + "" + "</option>"); //first item
                    for (var i = 0; i < data.length; i++) {
                        items.push("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
                    }                                         //all data from the team table push into array
                    $("#ddlSubType").html(items.join(' '));

                },
                error: function (data) {

                }
            });
        });
    });


    function ClientValidation() {

        var errMsg = "";
        var isError = false;
        $("#ErrorMsgContainer").html("");

        HighlightInputsForError($("#drpCountryList"), false)
        if (document.getElementById('drpCountryList').selectedIndex == 0) {
            isError = true;
            errMsg += "Please Select The country. </br>";
            HighlightInputsForError($("#drpCountryList"), isError)
        }
        HighlightInputsForError($("#drpEquipmentType"), false)
        if (document.getElementById('drpEquipmentType').selectedIndex == 0) {
            isError = true;
            errMsg += "Please Select The Equipment Type.";
            HighlightInputsForError($("#drpEquipmentType"), isError)
        }
        if (isError == true) {
            ShowRemoveValidationMessage(errMsg, "Warning")
            return false;
        }
        else {
            return true;
        }
    }


    function ClientAddValidation() {

        var errMsg = "";
        var isError = false;
        $("#ErrorMsgContainer").html("");

        HighlightInputsForError($("#ddlSubType"), false)
        if (document.getElementById('ddlSubType').selectedIndex == 0) {
            isError = true;
            errMsg += "Please select an Equipment Type to Query. </br>";
            HighlightInputsForError($("#ddlSubType"), isError)
        }

        if (isError == true) {
            ShowRemoveValidationMessage(errMsg, "Warning")
            return false;
        }
        else {
            return true;
        }
    }

    function ClientViewValidation() {

        var errMsg = "";
        var isError = false;
        $("#ErrorMsgContainer").html("");

        HighlightInputsForError($("#ddlEqType"), false)
        if (document.getElementById('ddlEqType').selectedIndex == 0) {
            isError = true;
            errMsg += "Please select an Equipment Type to Query. </br>";
            HighlightInputsForError($("#ddlEqType"), isError)
        }

        if (isError == true) {
            ShowRemoveValidationMessage(errMsg, "Warning")
            return false;
        }
        else {
            return true;
        }
    }

    function ClientEditValidation() {

        var errMsg = "";
        var isError = false;
        $("#ErrorMsgContainer").html("");

        HighlightInputsForError($("#drpCountryList"), false)
        if (document.getElementById('drpCountryList').selectedIndex == 0) {
            isError = true;
            errMsg += "Please Select The country. ";
            HighlightInputsForError($("#drpCountryList"), isError)
        }
        HighlightInputsForError($("#drpEquipmentType"), false)
        if (document.getElementById('drpEquipmentType').selectedIndex == 0) {
            isError = true;
            if (errMsg.trim() == "") {
                errMsg = "Please Select The Equipment Type.";
            }
            else {
                errMsg += ", the Equipment Type.";
            }
            HighlightInputsForError($("#drpEquipmentType"), isError)
        }
        if (isError == true) {
            ShowRemoveValidationMessage(errMsg, "Warning")
            return false;
        }
        else {
            return true;
        }
    }