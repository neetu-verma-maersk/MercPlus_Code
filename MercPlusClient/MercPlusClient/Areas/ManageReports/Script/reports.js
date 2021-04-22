/// <reference path="../../../Scripts/mainjs.js" />

$(function () {

    $('#drpReportsList').change(function () {
        // SuccessError(MESSAGETYPE.SUCCESS, "Filter loaded");
        $("#container").html("<span>Loading...</span>");
        var ReportsID = $(this).val();
        $("#container").load("/ManageReports/ManageReports/DisplayPartialView",
    { Reports_ID: ReportsID });
    });


    $("#drpCountry").change(function () {
        var country = $(this).val();
        var customer = $('#drpCustomer').val();
        var manual = $('#drpManual').val();
        $.ajax({
            url: "/ManageReports/ManageReports/GetShopList",
            type: 'POST',
            data: { CountryCode: country, CustomerCode: customer, ManualCode: manual },
            //data: JSON.stringify({ CountryCode: c, locale: locale }),
            cache: false,
            success: function (data) {
                var items;// = '<option>Select a Shop</option>';

                $.each(data.drpShop, function (i, drpShop) {

                    items += "<option value='" + drpShop.Value + "'>" + drpShop.Text + "</option>";
                });
                $('#drpShop').html(items);

                var items = '<option>**Any**</option>';
                $.each(data.drpMode, function (i, drpMode) {

                    items += "<option value='" + drpMode.Value + "'>" + drpMode.Text + "</option>";
                });
                $('#drpMode').html(items);
            },
            error: function (data) {
            }
        });
    });

    $("#drpShop").change(function () {
        var shop = $(this).val();
        var customer = $('#drpCustomer').val();
        var manual = $('#drpManual').val();
        $.ajax({
            url: "/ManageReports/ManageReports/GetModeList",
            type: 'POST',
            data: { ShopCode: shop, CustomerCode: customer, ManualCode: manual },
            //data: JSON.stringify({ CountryCode: c, locale: locale }),
            cache: false,
            success: function (data) {

                items = '<option>***Any***</option>';
                $.each(data.drpMode, function (i, drpMode) {

                    items += "<option value='" + drpMode.Value + "'>" + drpMode.Text + "</option>";
                });
                $('#drpMode').html(items);
            },
            error: function (data) {
            }
        });
    });

    $("#drpManual").change(function () {
        var shop = $('#drpShop').val();
        var customer = $('#drpCustomer').val();
        var manual = $(this).val();
        $.ajax({
            url: "/ManageReports/ManageReports/GetModeList",
            type: 'POST',
            data: { ShopCode: shop, CustomerCode: customer, ManualCode: manual },
            //data: JSON.stringify({ CountryCode: c, locale: locale }),
            cache: false,
            success: function (data) {

                items = '<option>***Any***</option>';
                $.each(data.drpMode, function (i, drpMode) {

                    items += "<option value='" + drpMode.Value + "'>" + drpMode.Text + "</option>";
                });
                $('#drpMode').html(items);
            },
            error: function (data) {
            }
        });
    });

    $("#drpCustomer").change(function () {
        var shop = $('#drpShop').val();
        var customer = $(this).val();
        var manual = $('#drpManual').val();
        $.ajax({
            url: "/ManageReports/ManageReports/GetModeList",
            type: 'POST',
            data: { ShopCode: shop, CustomerCode: customer, ManualCode: manual },
            //data: JSON.stringify({ CountryCode: c, locale: locale }),
            cache: false,
            success: function (data) {

                items = '<option>***Any***</option>';
                $.each(data.drpMode, function (i, drpMode) {

                    items += "<option value='" + drpMode.Value + "'>" + drpMode.Text + "</option>";
                });
                $('#drpMode').html(items);
            },
            error: function (data) {
            }
        });
    });

    $("#drpMode").change(function () {
        var mode = $(this).val();
        var shop = $('#drpShop').val();
        var customer = $('#drpCustomer').val();
        var manual = $('#drpManual').val();
        $.ajax({
            url: "/ManageReports/ManageReports/GetRepairCodeList",
            type: 'POST',
            data: { ShopCode: shop, CustomerCode: customer, ManualCode: manual, ModeCode: mode },
            //data: JSON.stringify({ CountryCode: c, locale: locale }),
            cache: false,
            success: function (data) {

                items = '<option>***Any***</option>';
                $.each(data.drpSTSCode, function (i, drpSTSCode) {

                    items += "<option value='" + drpSTSCode.Value + "'>" + drpSTSCode.Text + "</option>";
                });
                $('#drpSTSCode').html(items);
            },
            error: function (data) {
            }
        });
    });

    $(':radio[value=COCL]').click(function () {
        $('#MERCE01COCL').show();
        $('#MERCE01Country').hide();
        $('#MERCE01Shop').hide();
    });


    $(':radio[value=Country]').click(function () {
        $('#MERCE01COCL').hide();
        $('#MERCE01Country').show();
        $('#MERCE01Shop').hide();
    });

    $(':radio[value=Shop]').click(function () {
        $('#MERCE01COCL').hide();
        $('#MERCE01Country').hide()
        $('#MERCE01Shop').show();
    });


    $(':radio[value=Country]').click(function () {
        $('#MERCE02Country').show();
        $('#MERCE02Shop').hide();
    });


    $(':radio[value=Shop]').click(function () {
        $('#MERCE02Country').hide();
        $('#MERCE02Shop').show();
    });

});

SubmitReports = function () {
    var rID = $('#ReportsID').val();
    //var ValidatePattern = /^(\d{4})(\/|-)(\d{1,2})(\/|-)(\d{1,2})$/;
    //var date = ('#DateFrom').match(ValidatePattern);
    //var chkdateto = document.getElementById("DateTo").value
    if ((rID == 1 || rID == 2 || rID == 3 || rID == 4) && ($('.CLSDTFROM').val() == null || $('.CLSDTFROM').val() == "")) {
        alert("Date from cannot be left blank");
        return false;
    }


    if ((rID == 1 || rID == 2 || rID == 3 || rID == 4) && ($('.CLSDTTO').val() == null || $('CLSDTTO').val() == "")) {
        alert("Date to cannot be left blank");
        return false;
    }

    //if ((rID == 4) && ($('#drpSTSCode').val == null || $('#drpSTSCode').val() == "")) {
    //    alert("STS is a Mandatory field");
    //    return false;
    //}

    if ((rID == 9 || rID == 10 || rID == 11) && ($('#drpManual').val == null || $('#drpManual').val() == "")) {
        alert("Manual is a Mandatory field");
        return false;
    }

    if ((rID == 9 || rID == 10 || rID == 11) && ($('#drpDays').val == null || $('#drpDays').val() == "")) {
        alert("Days is a Mandatory field");
        return false;
    }

    if ((rID == 8) && ($('#drpCountry').val == null || $('#drpCountry').val() == "")) {
        alert("Country is a Mandatory field");
        return false;
    }
}

