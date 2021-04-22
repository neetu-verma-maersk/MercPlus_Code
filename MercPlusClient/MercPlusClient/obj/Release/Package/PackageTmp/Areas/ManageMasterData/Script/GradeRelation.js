// Edit event handler.
$("body").on("click", "#WebGrid TBODY .Edit", function () {   
    $('#lblMessage').text('');
    var relationId = $(this).closest('td').attr('id');       

    ShowEditMode(relationId);  

    var spanUpgradedGradesSelectedValue = $('#spanUpgradedGrades_' + relationId).text();
    if (spanUpgradedGradesSelectedValue != null && spanUpgradedGradesSelectedValue != '') {
        spanUpgradedGradesSelectedValue = spanUpgradedGradesSelectedValue.trim().split(',');
        var element = document.getElementById('dropdownUpgradedGrades_' + relationId);        
        for (var i = 0; i < element.options.length; i++) {
            element.options[i].selected = spanUpgradedGradesSelectedValue.indexOf(element.options[i].value) >= 0;
        }  
    } 

    var spanDowngradedGradesSelectedValue = $('#spanDowngradedGrades_' + relationId).text();
    if (spanDowngradedGradesSelectedValue != null && spanDowngradedGradesSelectedValue != '') {
        spanDowngradedGradesSelectedValue = spanDowngradedGradesSelectedValue.trim().split(',');
        var element = document.getElementById('dropdownDowngradedGrades_' + relationId);
        for (var i = 0; i < element.options.length; i++) {
            element.options[i].selected = spanDowngradedGradesSelectedValue.indexOf(element.options[i].value) >= 0;
        }
    }
});

// Delete event handler.
$("body").on("click", "#WebGrid TBODY .Delete", function () {
    $('#lblMessage').text('');
    var relationId = $(this).closest('td').attr('id');
    DeleteGradeRelation(relationId);
});

// Update event handler.
$("body").on("click", "#WebGrid TBODY .Update", function () {  
    $('#lblMessage').text('');
    var relationId = $(this).closest('td').attr('id');    
    var upgradedGrades = $('#dropdownUpgradedGrades_' + relationId).val();
    var downgradedGrades = $('#dropdownDowngradedGrades_' + relationId).val();   
    var gradeCode = $('#spanGradeCode_' + relationId).text();
    UpdateGradeRelation(relationId, gradeCode, upgradedGrades, downgradedGrades);
});   

// Cancel event handler.
$("body").on("click", "#WebGrid TBODY .Cancel", function () {
    $('#lblMessage').text('');
    var relationId = $(this).closest('td').attr('id');
    HideEditMode(relationId);
});

// Add Grade Relation row event handler.
$("body").on("click", "#btnAddGradeRelationRow", function () {
    $('#lblMessage').text('');
    var row = $("#rowAdd");
    row.show();
    ClearAddRow();
    $("#btnAddGradeRelationRow").attr("disabled", true);
});

// Add Grade Relation event handler.
$("body").on("click", "#btnAddGradeRelation", function () {
    $('#lblMessage').text('');

    var gradecode = $('#newGradeCode').val();
    var gradedescription = $('#newGradeDescription').val();
    var upgradedGrades = $('#dropdownDowngradedGrades').val();
    var downgradedGrades = $('#dropdownUpgradedGrades').val();

    AddGradeRelation(gradecode, gradedescription, upgradedGrades, downgradedGrades);    
});

// Cancel Grade Relation event handler.
$("body").on("click", "#btnCancelGradeRelation", function () {
    $('#lblMessage').text('');
    var row = $("#rowAdd");
    row.hide();
    ClearAddRow();
    $("#btnAddGradeRelationRow").attr("disabled", false);
});

function ClearAddRow() {
    $('#newGradeCode').val('');
    $('#newGradeDescription').val('');
    $('#dropdownDowngradedGrades').val('');
    $('#dropdownUpgradedGrades').val('');
}

function HideEditMode(id) {
    var dropdownUpgradedGrades = $("#dropdownUpgradedGrades_" + id);
    var dropdownDowngradedGrades = $("#dropdownDowngradedGrades_" + id);
    var spanUpgradedGrades = $("#spanUpgradedGrades_" + id);
    var spanDowngradedGrades = $("#spanDowngradedGrades_" + id);

    var btnEdit = $("#btnEdit_" + id);
    var btnDelete = $("#btnDelete_" + id);
    var btnUpdate = $("#btnUpdate_" + id);
    var btnCancel = $("#btnCancel_" + id);

    dropdownUpgradedGrades.hide();
    dropdownDowngradedGrades.hide();
    spanUpgradedGrades.show();
    spanDowngradedGrades.show();

    btnEdit.show();
    btnDelete.show();
    btnUpdate.hide();
    btnCancel.hide();

    dropdownUpgradedGrades.val('');
    dropdownDowngradedGrades.val('');
}

function ShowEditMode(id) {
    var dropdownUpgradedGrades = $("#dropdownUpgradedGrades_" + id);
    var dropdownDowngradedGrades = $("#dropdownDowngradedGrades_" + id);
    var spanUpgradedGrades = $("#spanUpgradedGrades_" + id);
    var spanDowngradedGrades = $("#spanDowngradedGrades_" + id);

    dropdownUpgradedGrades.val('');
    dropdownDowngradedGrades.val('');
      
    var btnEdit = $("#btnEdit_" + id);
    var btnDelete = $("#btnDelete_" + id);
    var btnUpdate = $("#btnUpdate_" + id);
    var btnCancel = $("#btnCancel_" + id);

    dropdownUpgradedGrades.show();
    dropdownDowngradedGrades.show();
    spanUpgradedGrades.hide();
    spanDowngradedGrades.hide();

    btnEdit.hide();
    btnDelete.hide();
    btnUpdate.show();
    btnCancel.show();       
}

function UpdateGradeRelation(relationId, gradecode, upgradedGrades, downgradedGrades) {    
    if (upgradedGrades != '' && upgradedGrades != null && upgradedGrades != undefined) {
        upgradedGrades = upgradedGrades.join(",");        
    } else {
        upgradedGrades = '';
    }

    if (downgradedGrades != '' && downgradedGrades != null && downgradedGrades != undefined) {
        downgradedGrades = downgradedGrades.join(",");        
    } else {
        downgradedGrades = '';
    }

    var errorMessage = ClientValidationUpgradedAndDowngradedGrades(gradecode, upgradedGrades, downgradedGrades);
    if (errorMessage != '') {
        $('#lblMessage').text(errorMessage);
        $('#lblMessage').css("color", "red");
        return;
    }

    $('#lblMessage').text('Grade relation is being updated...Please wait...');
    $('#lblMessage').css("color", "black");
    HideActions();

    $.ajax({
        url: "/ManageMasterData/ManageMasterData/JsonUpdateGradeRelation",
        type: 'POST',
        data: { gradeRelationId: relationId, upgradedGrades: upgradedGrades, downgradedGrades: downgradedGrades },
        cache: false,
        success: function (data) {
            if (data == true) {
                $('#lblMessage').text('Grade relation has been updated successfully.');
                $('#lblMessage').css("color", "green");
                $('#spanUpgradedGrades_' + relationId).text(upgradedGrades);
                $('#spanDowngradedGrades_' + relationId).text(downgradedGrades);
            } else {
                $('#lblMessage').text('Grade relation has not been updated.');
                $('#lblMessage').css("color", "red");
            }
            HideEditMode(relationId);
            ShowActions();
        },
        error: function () {
            $('#lblMessage').text('Grade relation has not been updated.');
            $('#lblMessage').css("color", "red");
            ShowEditMode(relationId);
            ShowActions();
        }
    });    
}

function DeleteGradeRelation(relationId) {
    $('#lblMessage').text('Grade relation is being deleted...Please wait...');
    $('#lblMessage').css("color", "black");

    HideActions();

    $.ajax({
        url: "/ManageMasterData/ManageMasterData/JsonDeleteGradeRelation",
        type: 'POST',
        data: { gradeRelationId: relationId },
        cache: false,
        success: function (data) {
            if (data == true) {
                $('#lblMessage').text('Grade relation has been deleted successfully.');
                $('#lblMessage').css("color", "green");
                $('#row_' + relationId).hide();
            } else {
                $('#lblMessage').text('Grade relation has not been deleted due to assigned with other Grade');
                $('#lblMessage').css("color", "red");
            }
            HideEditMode(relationId);
            ShowActions();
        },
        error: function () {
            $('#lblMessage').text('Grade relation has not been deleted.');
            $('#lblMessage').css("color", "red");
            ShowEditMode(relationId);
            ShowActions();
        }
    });
}

function AddGradeRelation(gradecode, gradedescription, upgradedGrades, downgradedGrades) {
    if (upgradedGrades != '' && upgradedGrades != null && upgradedGrades != undefined) {
        upgradedGrades = upgradedGrades.join(",");
    } else {
        upgradedGrades = '';
    }

    if (downgradedGrades != '' && downgradedGrades != null && downgradedGrades != undefined) {
        downgradedGrades = downgradedGrades.join(",");
    } else {
        downgradedGrades = '';
    }
    
    var errorMessage = ClientValidationAddGradeRelation(gradecode, gradedescription, upgradedGrades, downgradedGrades);
    if (errorMessage != '') {
        $('#lblMessage').text(errorMessage);
        $('#lblMessage').css("color", "red");
        return;
    }
    $('#lblMessage').text('Grade relation is being added...Please wait...');
    $('#lblMessage').css("color", "black");

    HideActions();

    $.ajax({
        url: "/ManageMasterData/ManageMasterData/AddGradeRelation",
        type: 'POST',
        data: { gradecode: gradecode, gradedescription: gradedescription, upgradedGrades: upgradedGrades, downgradedGrades: downgradedGrades },
        cache: false,
        success: function (data) {
            if (data != '' && data != null) {
                window.location.href = data.Url;
            } else {
                $('#lblMessage').text('Grade relation has not been added.');
                $('#lblMessage').css("color", "red");
            }   
            ShowActions();
        },
        error: function () {
            $('#lblMessage').text('Grade relation has not been added.');
            $('#lblMessage').css("color", "red");
            ShowActions();
        }
    });
}

function ClientValidationAddGradeRelation(gradecode, gradedescription, upgradedgrades, downgradedgrades) {

    var errMsg = '';
    
    var regex = new RegExp("^[a-zA-Z]+$");
    if (!regex.test(gradecode)) {
        errMsg = 'Grade code must be a single character';
        return errMsg;
    }
    if (gradecode.length > 1) {
        errMsg = 'Grade code must be a single character';
        return errMsg;
    }

    if (gradedescription == '' || gradedescription == null) {
        errMsg = 'Grade description should not be empty';
        return errMsg;
    }
    if (gradedescription.length > 50) {
        errMsg = 'Grade description should not exceed 50 character';
        return errMsg;
    }    
    errMsg = ClientValidationUpgradedAndDowngradedGrades(gradecode, upgradedgrades, downgradedgrades);

    return errMsg;
}

function ClientValidationUpgradedAndDowngradedGrades(gradecode, upgradedgrades, downgradedgrades) {
    var errMsg = '';

    if ((upgradedgrades != null && upgradedgrades != '') && upgradedgrades.toUpperCase().search(gradecode.toUpperCase()) != -1) {
        errMsg = 'Upgraded grades can not contain the same grade';
        return errMsg;
    }

    if ((downgradedgrades != null && downgradedgrades != '') && downgradedgrades.toUpperCase().search(gradecode.toUpperCase()) != -1) {
        errMsg = 'Downgraded grades can not contain the same grade';
        return errMsg;
    }

    if ((downgradedgrades != null && downgradedgrades != '') && (upgradedgrades != null && upgradedgrades != '') && downgradedgrades.toUpperCase().search(upgradedgrades.toUpperCase()) != -1) {
        errMsg = 'Downgraded grades can not contain the same grade as Upgraded grade code';
        return errMsg;
    }
    if ((upgradedgrades != null && upgradedgrades != '') && (downgradedgrades != null && downgradedgrades != '') && upgradedgrades.toUpperCase().search(downgradedgrades.toUpperCase()) != -1) {
        errMsg = 'Upgraded grades can not contain the same grade as Downgraded grade Code';
        return errMsg;
    }
   // downgradedgrades = downgradedgrades.trim.split(',');
    //upgradedgrades = upgradedgrades.trim.split(',');

    if (downgradedgrades != null && downgradedgrades != '') {
        downgradedgrades = downgradedgrades.trim().split(',');
    }

    if (upgradedgrades != null && upgradedgrades != '') {
        upgradedgrades = upgradedgrades.trim().split(',');
    }

    
    var found = false;
    for (var i = 0; i < downgradedgrades.length; i++) {
        if (upgradedgrades.indexOf(downgradedgrades[i]) > -1 && downgradedgrades[i]!="," ) 
        {
            found = true;
            break;
        }
    }
    if (found==true) {
        errMsg = 'Same Grade cannot be assigned as Up or down graded grade code';

        return errMsg;
    }
    return errMsg;
}



function HideActions() {
    $(".actioncolumn").hide();
    $("#btnAddGradeRelationRow").attr("disabled", true);
}

function ShowActions() {
    $(".actioncolumn").show();
    $("#btnAddGradeRelationRow").attr("disabled", false);
}