$(function () {
    $('#btnQueryRepairLocCode').click(function () {
        var c = $("#drpRepairLocationCode").val();
        if (c.trim() == "") {
            alert("Please Select a Repair Location code to Query");
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