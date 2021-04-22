

$(function () {

    $("#btnAddCPH").click(function () {

        //e.preventDefault();
        $("#divCPHApproval").show();

        $('#ddlEqSize').css("display", "inline-block");
        $('#ddlMode_List').css("display", "inline-block");

        $('#EqSize').css("display", "none");
        $('#ModeList').css("display", "none");

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
});

   //function Query() {


   //     $('#ChangeUserName1').css("display", "inline-block");
   //     $('#ChangeUserName2').css("display", "inline-block");
   //     $('#ChangeUserName3').css("display", "inline-block");
   //     $('#ChangeUserName4').css("display", "inline-block");
   //     $('#ChangeUserName5').css("display", "inline-block");
   //     $('#ChangeUserName6').css("display", "inline-block");
   //     $('#ChangeUserName7').css("display", "inline-block");

   //     $('#ChangeTime1').css("display", "inline-block");
   //     $('#ChangeTime2').css("display", "inline-block");
   //     $('#ChangeTime3').css("display", "inline-block");
   //     $('#ChangeTime4').css("display", "inline-block");
   //     $('#ChangeTime5').css("display", "inline-block");
   //     $('#ChangeTime6').css("display", "inline-block");
   //     $('#ChangeTime7').css("display", "inline-block");

   //     $('#lblChName').css("display", "inline-block");
   //     $('#lblChTime').css("display", "inline-block");

   //     $('#ddlEqSize').css("display", "none");
   //     $('#ddlMode_List').css("display", "none");

   //     $('#EqSize').css("display", "inline-block");
   //     $('#ModeList').css("display", "inline-block");

   //     var equipmentSize = $("#drpEqSize").val();
   //     var modeList = $("#drpModeList").val();

   //     if ((equipmentSize.length > 0) && (modeList, length > 0)) {
   //         $("#divCPHApproval").show();
   //         $('#tblEdit').show();
   //         $('#tblSubmit').hide();
   //     }


   //     // var a = JSON.stringify({ customerId: $(this).val() };
   //     //$.ajax({
   //     //    url: "/ManageMasterData/ManageMasterData/GetAllDetailsForCPHApproval",
   //     //    type: 'POST',
   //     //    datatype: 'json',
   //     //    data: { Eq: equipmentSize, Mode: modeList },
   //     //    cache: false,
   //     //    success: function (data) {

   //     //        var equipmentSize = $("#drpEqSize").val();
   //     //        var modeList = $("#drpModeList").val();

   //     //        $("#EqSize").val(equipmentSize);
   //     //        $("#ModeList").val(modeList);

   //     //        if ((equipmentSize.length > 0) && (modeList.length > 0)) {
   //     //            $("#divCPHApproval").show();
   //     //            $('#tblEdit').show();
   //     //            $('#tblSubmit').hide();
   //     //        }



   //     //        var items = [];
   //     //        $.each(data, function (i, data) {

   //     //            //if (data == null || data.data == null)
   //     //            //if (data.Result == "0")

   //     //            if (data == null) {

   //     //                $('#LimitAmt1').val("");
   //     //                $("#ChangeUserName1").val("");
   //     //                $("#ChangeTime1").val("");

   //     //                $("#LimitAmt2").val("");
   //     //                $("#ChangeUserName2").val("");
   //     //                $("#ChangeTime2").val("");

   //     //                $("#LimitAmt3").val("");
   //     //                $("#ChangeUserName3").val("");
   //     //                $("#ChangeTime3").val("");

   //     //                $("#LimitAmt4").val("");
   //     //                $("#ChangeUserName4").val("");
   //     //                $("#ChangeTime4").val("");

   //     //                $("#LimitAmt5").val("");
   //     //                $("#ChangeUserName5").val("");
   //     //                $("#ChangeTime5").val("");

   //     //                $("#LimitAmt6").val("");
   //     //                $("#ChangeUserName6").val("");
   //     //                $("#ChangeTime6").val("");

   //     //                $("#LimitAmt7").val("");
   //     //                $("#ChangeUserName7").val("");
   //     //                $("#ChangeTime7").val("");
   //     //            }
   //     //            else {

   //     //                if (i == 0) {

   //     //                    $("#LimitAmt1").val(data.LimitAmount);
   //     //                    $("#ChangeUserName1").val(data.ChangeUser);
   //     //                    $("#ChangeTime1").val(formatJSONDate(data.ChTime));

   //     //                }


   //     //                if (i == 1) {
   //     //                    $("#LimitAmt2").val(data.LimitAmount);
   //     //                    $("#ChangeUserName2").val(data.ChangeUser);
   //     //                    $("#ChangeTime2").val(formatJSONDate(data.ChTime));


   //     //                }

   //     //                if (i == 2) {
   //     //                    $("#LimitAmt3").val(data.LimitAmount);
   //     //                    $("#ChangeUserName3").val(data.ChangeUser);
   //     //                    $("#ChangeTime3").val(formatJSONDate(data.ChTime));
   //     //                }

   //     //                if (i == 3) {
   //     //                    $("#LimitAmt4").val(data.LimitAmount);
   //     //                    $("#ChangeUserName4").val(data.ChangeUser);
   //     //                    $("#ChangeTime4").val(formatJSONDate(data.ChTime));
   //     //                }

   //     //                if (i == 4) {
   //     //                    $("#LimitAmt5").val(data.LimitAmount);
   //     //                    $("#ChangeUserName5").val(data.ChangeUser);
   //     //                    $("#ChangeTime5").val(formatJSONDate(data.ChTime));
   //     //                }

   //     //                if (i == 5) {
   //     //                    $("#LimitAmt6").val(data.LimitAmount);
   //     //                    $("#ChangeUserName6").val(data.ChangeUser);
   //     //                    $("#ChangeTime6").val(formatJSONDate(data.ChTime));
   //     //                }

   //     //                if (i == 6) {
   //     //                    $("#LimitAmt7").val(data.LimitAmount);
   //     //                    $("#ChangeUserName7").val(data.ChangeUser);
   //     //                    $("#ChangeTime7").val(formatJSONDate(data.ChTime));
   //     //                }

   //     //            }
   //     //        });



   //     //    },
   //         //error: function (data) {

   //         //}
   //     }
       


    
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

