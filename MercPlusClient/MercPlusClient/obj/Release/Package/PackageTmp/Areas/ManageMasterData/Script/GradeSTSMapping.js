// Edit event handler.

$("body").on("click", "#WebGrid TBODY .Edit", function () {
    $('#lblMessage').text('');
    var relationId = $(this).closest('td').attr('id');    

    ShowEditMode(relationId);

    var spanGradeCodeId = "spanGradeCode_" + relationId;
    var spanGradeCodes = $("[id^='" + spanGradeCodeId + "']");
    var dropDownGradeCodeId = "dropDownGradeCode_" + relationId;
    var dropDownGradeCodes = $("[id^='" + dropDownGradeCodeId + "']");   

    for (i = 0; i < spanGradeCodes.length; i++) {       
        var spanGradeCodeSelectedValue = spanGradeCodes[i].innerText;        
        if (spanGradeCodeSelectedValue != null && spanGradeCodeSelectedValue != '') {
            spanGradeCodeSelectedValue = spanGradeCodeSelectedValue.trim();            
            var element = dropDownGradeCodes[i];         
            for (var j = 0; j < element.options.length; j++) {                
                element.options[j].selected = spanGradeCodeSelectedValue.indexOf(element.options[j].value) >= 0;
            }
        }
    }
});


// Delete event handler.
$("body").on("click", "#WebGrid TBODY .Delete", function () {
    $('#lblMessage').text('');
    var relationId = $(this).closest('td').attr('id');
    DeleteGradeSTSMapping(relationId);
});

// Update event handler.
$("body").on("click", "#WebGrid TBODY .Update", function () {
    $('#lblMessage').text('');
    var relationId = $(this).closest('td').attr('id');    
    UpdateGradeSTSMapping(relationId);
});

// Cancel event handler.
$("body").on("click", "#WebGrid TBODY .Cancel", function () {
    $('#lblMessage').text('');
    var relationId = $(this).closest('td').attr('id');
    HideEditMode(relationId);
});

// Add Grade STS mapping row event handler.
$("body").on("click", "#btnAddSTSGradeMapping", function () {
    $('#lblMessage').text('');
    var row = $("#rowAdd");
    row.show();
    ClearAddRow();
    $("#btnAddSTSGradeMapping").attr("disabled", true);

    $("#divContent").animate({ scrollTop: $('#divContent')[0].scrollHeight }, 1000);
});

// Add Grade STS mapping event handler.
$("body").on("click", "#btnAddSTSGrade", function () {
    $('#lblMessage').text('');   
    AddGradeSTSMapping();
});

// Cancel Grade Relation event handler.
$("body").on("click", "#btnCancelSTSGrade", function () {
    $('#lblMessage').text('');
    var row = $("#rowAdd");
    row.hide();
    ClearAddRow();
    $("#btnAddSTSGradeMapping").attr("disabled", false);
});

function GetModeDesc(stscodevalue, modevalue) {
    var desc = "";

    $.ajax({
        url: "/ManageMasterData/ManageMasterData/JsonGetSTSDescription",
        type: 'POST',
        dataType: 'json',
        data: { stscode: stscodevalue, mode: modevalue },
        cache: false,
        success: function (data) {
            //console.log(data);
            //alert(data);
           // $('#newSTSDescription').text(data);
            $("#newSTSDescription").html(data);
            ShowActions();
            if (data != '') {
                $('#lblMessage').text('');
            }
            else 
                $('#lblMessage').text('STS Code not exist in this selected MODE');
        },
        error: function () {
            $('#lblMessage').text('STS description not found.');
            $('#lblMessage').css("color", "red");
            ShowActions();
        }
    });
   
}
$("body").on("focusout", "#newSTSCode", function () {
    var stscodevalue = $('#newSTSCode').val();
    if (stscodevalue == '') {
        return;
    }
    var modevalue = $('#dropDownSTSMode').val();

    $('#lblMessage').text('STS Description is being fetched...Please wait...');
    $('#lblMessage').css("color", "black");
    HideActions();
    
    GetModeDesc(stscodevalue, modevalue);
  
});

$(function () {
    $("#dropDownSTSMode").change(function () {
        //alert($('option:selected', this).text());
        var stscodevalue = $('#newSTSCode').val();
        if (stscodevalue == '') {
            return;
        }
        var modevalue = $('#dropDownSTSMode').val();

        $('#lblMessage').text('STS Description is being fetched...Please wait...');
        $('#lblMessage').css("color", "black");
        HideActions();

        GetModeDesc(stscodevalue, modevalue);
    });
});

function ClearAddRow() {
    $('#newSTSCode').val('');
    $('#newSTSDescription').val('');   
}

function HideEditMode(id) {
    var dropDownGradeCodeId = "dropDownGradeCode_" + id;
    var dropDownGradeCodes = $("[id^='" + dropDownGradeCodeId + "']");
    var spanGradeCodeId = "spanGradeCode_" + id;
    var spanGradeCodes = $("[id^='" + spanGradeCodeId + "']");

    var btnEdit = $("#btnEdit_" + id);
    var btnDelete = $("#btnDelete_" + id);
    var btnUpdate = $("#btnUpdate_" + id);
    var btnCancel = $("#btnCancel_" + id);

    dropDownGradeCodes.hide();    
    spanGradeCodes.show();    

    btnEdit.show();
    btnDelete.show();
    btnUpdate.hide();
    btnCancel.hide();

    dropDownGradeCodes.val('');    
}

function ShowEditMode(id) {
    var dropDownGradeCodeId = "dropDownGradeCode_" + id;
    var dropDownGradeCodes = $("[id^='" + dropDownGradeCodeId + "']");
    var spanGradeCodeId = "spanGradeCode_" + id;
    var spanGradeCodes = $("[id^='" + spanGradeCodeId + "']");   
    

    dropDownGradeCodes.val('');    

    var btnEdit = $("#btnEdit_" + id);
    var btnDelete = $("#btnDelete_" + id);
    var btnUpdate = $("#btnUpdate_" + id);
    var btnCancel = $("#btnCancel_" + id);

    dropDownGradeCodes.show();    
    spanGradeCodes.hide();    

    btnEdit.hide();
    btnDelete.hide();
    btnUpdate.show();
    btnCancel.show();
}

function UpdateGradeSTSMapping(relationId) {
    var stscodevalue = $('#spanSTSCode_' + relationId).text();
    var modevalue = $('#spanSTSMode_' + relationId).text();
    var gradeapplicablelist = new Array();

    var spanGradeCodeId = "spanGradeCode_" + relationId;
    var spanGradeCodes = $("[id^='" + spanGradeCodeId + "']");
    var dropDownGradeCodeId = "dropDownGradeCode_" + relationId;
    var dropDownGradeCodes = $("[id^='" + dropDownGradeCodeId + "']"); 

    for (i = 0; i < dropDownGradeCodes.length; i++) {        
        var element = dropDownGradeCodes[i];              
        var gradeApplicableValue = element.options[element.selectedIndex].text;
        gradeapplicablelist.push(gradeApplicableValue);
    }   

    var gradeApplicableValues = gradeapplicablelist.join(',');    
    
    $('#lblMessage').text('Grade STS mapping is being updated...Please wait...');
    $('#lblMessage').css("color", "black");
    HideActions();

    $.ajax({
        url: "/ManageMasterData/ManageMasterData/JsonUpdateGradeSTSMapping",
        type: 'POST',
        data: { stscode: stscodevalue, mode: modevalue, gradeapplicablevalues: gradeApplicableValues },
        cache: false,
        success: function (data) {
            if (data == true) {
                $('#lblMessage').text('Grade relation has been updated successfully.');
                $('#lblMessage').css("color", "green");                

                for (i = 0; i < spanGradeCodes.length; i++) {
                    var element = dropDownGradeCodes[i];
                    spanGradeCodes[i].innerText = element.options[element.selectedIndex].text;    
                    if (spanGradeCodes[i].innerText == "Applicable") {
                        spanGradeCodes[i].style.color = "green";
                    }
                    else if (spanGradeCodes[i].innerText == "Not Applicable") {
                        spanGradeCodes[i].style.color = "orange";
                    }
                }
            } else {
                $('#lblMessage').text('Grade STS mapping has not been updated.');
                $('#lblMessage').css("color", "red");
            }
            HideEditMode(relationId);
            ShowActions();
        },
        error: function () {
            $('#lblMessage').text('Grade STS mapping has not been updated.');
            $('#lblMessage').css("color", "red");
            ShowEditMode(relationId);
            ShowActions();
        }
    });
}

function DeleteGradeSTSMapping(relationId) {
    $('#lblMessage').text('Grade STS mapping is being deleted...Please wait...');
    $('#lblMessage').css("color", "black");
    var stscodevalue = $('#spanSTSCode_' + relationId).text();
    var modevalue = $('#spanSTSMode_' + relationId).text();

    HideActions();

    $.ajax({
        url: "/ManageMasterData/ManageMasterData/JsonDeleteGradeSTSMapping",
        type: 'POST',
        data: { stscode: stscodevalue, mode: modevalue},
        cache: false,
        success: function (data) {
            if (data == true) {
                $('#lblMessage').text('Grade STS mapping has been deleted successfully.');
                $('#lblMessage').css("color", "green");
                $('#row_' + relationId).hide();
            } else {
                $('#lblMessage').text('Grade STS mapping has not been deleted.');
                $('#lblMessage').css("color", "red");
            }
            HideEditMode(relationId);
            ShowActions();
        },
        error: function () {
            $('#lblMessage').text('Grade STS mapping has not been deleted.');
            $('#lblMessage').css("color", "red");
            ShowEditMode(relationId);
            ShowActions();
        }
    });
}

function AddGradeSTSMapping() {
    var stscodevalue = $('#newSTSCode').val();
    var stsdescriptionvalue = $('#newSTSDescription').text();
    var modevalue = $('#dropDownSTSMode').val();

    var gradeapplicablelist = new Array();
   
    var dropDownGradeCodeId = "dropDownGradeCode_000";
    var dropDownGradeCodes = $("[id^='" + dropDownGradeCodeId + "']");

    for (i = 0; i < dropDownGradeCodes.length; i++) {
        var element = dropDownGradeCodes[i];
        var gradeApplicableValue = element.options[element.selectedIndex].text;
        gradeapplicablelist.push(gradeApplicableValue);
    }

    var gradeApplicableValues = gradeapplicablelist.join(',');

    var errorMessage = ClientValidationAddGradeSTSMapping(stscodevalue, stsdescriptionvalue);
    if (errorMessage != '') {
        $('#lblMessage').text(errorMessage);
        $('#lblMessage').css("color", "red");
        return;
    }
    $('#lblMessage').text('Grade STS mapping is being added...Please wait...');
    $('#lblMessage').css("color", "black");
    HideActions();

    $.ajax({
        url: "/ManageMasterData/ManageMasterData/JsonAddGradeSTSMapping",
        type: 'POST',
        data: { stscode: stscodevalue, mode: modevalue, gradeapplicablevalues: gradeApplicableValues },
        cache: false,
        success: function (data) {
            if (data != '' && data != null) {
                window.location.href = data.Url;
            } else {
                $('#lblMessage').text('STS-Grade mapping exist or other reason Grade STS mapping has not been added.');
                $('#lblMessage').css("color", "red");
            }
            ShowActions();
        },
        error: function () {
            $('#lblMessage').text('Grade STS mapping has not been added.');
            $('#lblMessage').css("color", "red");
            ShowActions();
        }
    });
}

function ClientValidationAddGradeSTSMapping(stscode, stsdescription) {

    var errMsg = '';

    if (stscode == '' || stscode == null) {
        errMsg = 'STS Code should not be empty';
        return errMsg;
    }

    if (stsdescription == '' || stsdescription == null) {
        errMsg = 'STS Description should not be empty';
        return errMsg;
    }   

    return errMsg;
}

function HideActions() {
    $(".actioncolumn").hide();
    $("#btnAddSTSGradeMapping").attr("disabled", true);
}

function ShowActions() {
    $(".actioncolumn").show();
    $("#btnAddSTSGradeMapping").attr("disabled", false);
}