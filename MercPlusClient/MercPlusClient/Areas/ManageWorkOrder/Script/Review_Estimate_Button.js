$(function () {
    $("#btnSelect").click(function (e) {
        e.preventDefault();
        var ischecked = true;
        $('#grdWO').find("input:checkbox").each(function () {
            this.checked = ischecked;
        });
    });

    $("#btnClear").click(function (e) {
        e.preventDefault();
        var ischecked = false;
        $('#grdWO').find("input:checkbox").each(function () {
            this.checked = ischecked;
        });
    });

    $("#btnWorking").click(function (e) {
        e.preventDefault();
        $("#ErrorMsgContainer").html("");
        var _griddata = gridTojson();
        var sSwitch = 'Y';
        var url = 'SetWorkingSwitchByID';
        $.ajax({
            url: url,
            type: 'POST',
            data: { gridData: _griddata, sSwitch: sSwitch }
        }).done(function (data) {
            if (data != "") {
                if (data == "Success") {
                    ShowRemoveValidationMessage("Selected estimates have been set to working.", "Success");
                    SearchWo();
                }
                else {
                    ShowRemoveValidationMessage(data, "Warning");
                    //ShowRemoveValidationMessage("Failed to set estimates to working. Please contact the System Administrator.", "Warning");
                }

                // $('#message').html(data);
            }
        });
    });

    $("#btnCompleted").click(function (e) {
        e.preventDefault();
        $("#ErrorMsgContainer").html("");
        var _griddata = gridTojson();
        var _date = getDateJson();
        var url = 'CompleteApprovedWO';
        $.ajax({
            url: url,
            type: 'POST',
            data: { griddata: _griddata, New_Status_Code: 400, Old_Status_Code: 390, gridDate: _date }
        }).done(function (data) {
            if (data != "") {
                if (data == "Success") {
                    ShowRemoveValidationMessage("Selected estimates have been completed.", "Success");
                    SearchWo();
                }
                else {
                    ShowRemoveValidationMessage(data, "Warning");
                }
            }
        });
    });

    $("#btnApproved").click(function (e) {
        e.preventDefault();

        $("#ErrorMsgContainer").html("");
        var _griddata = gridTojson();
        if (_griddata != "") {

            var url = 'ApprovePending';
            $.ajax({
                url: url,
                type: 'POST',
                data: { gridData: _griddata },

                error: function () {
                },
                success: function (data) {
                    if (data != "") {
                        if (data == "Success") {
                            ShowRemoveValidationMessage("Selected estimates have been approved.", "Success");
                            SearchWo();
                        }
                        else {
                            ShowRemoveValidationMessage(data, "Warning");
                        }
                    }
                }
            });
        }
    });
});