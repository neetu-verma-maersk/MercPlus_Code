$(function () {
    $("#CountryCode").change(function () {
        var c = $(this).val();
        $.ajax({
            url: "/ManageUser/ManageUser/GetUserListOfACountry",
            type: 'POST',
            data: { CountryCode: c },
            cache: false,
            success: function (data) {
                var items = '<option>Select a User</option>';
                var itemCount = 0;
                if (data.length == 0) {
                    items = '<option>No User Exist For</option>';
                    $('#tblEditDeleteOperationButton').hide();
                }
                else {
                    items = '<option>Select a User</option>';
                    itemCount = 1;
                }
                $.each(data, function (i, UserList) {
                    items += "<option value='" + UserList.Value + "'>" + UserList.Text + "</option>";
                });
                $('#ddlLogin').html(items);
                if (itemCount == 1) {
                    $('#tblEditDeleteOperationButton').show();
                }

                $("#fieldsetAuthorisationGroup").hide();
                $("#fieldsetAuthorisationGroupDataAccess").hide();
                $("#fieldsetAuthorisationGroupWebsitePermission").hide();
                $("#fieldsetAuthorisationGroupWebPagePermission").hide();
                var errCont = $("#ErrorMsgContainer");
                $(errCont).html("");
                addErrorClass(errCont);
            },
            error: function (data) {
            }
        });
    });
});
$(function () {
    $("#ddlLogin").change(function () {
        $("#ErrorMsgContainer").html("");
        HighlightInputsForError($("#ddlLogin"), false)
        $("#UserId").val($(this).val());
        $("#fieldsetAuthorisationGroup").hide();
        $("#fieldsetAuthorisationGroupDataAccess").hide();
        $("#fieldsetAuthorisationGroupWebsitePermission").hide();
        $("#fieldsetAuthorisationGroupWebPagePermission").hide();
    });
});

$(function () {
    $('#btnBack').click(function () {

        $('#tblEditView').hide();
        $('#tblSelectUserToEdit').show();
        $('#tblEditDeleteOperationButton').show();
    });
});

function ModifyUserPermission() {

    $("#ErrorMsgContainer").html("");
    $("#trAvailablePermissionCount").html("");

    ShowRemoveValidationMessage("Please Wait.....", "")
    var errMsg = "";
    var isError = false;
    //if ($("#hdAuthGroupId").val() == 0) {
    //    isError = true;
    //    errMsg = "Please Select Authorisation Group ";
    //    //HighlightInputsForError($("#ddlLogin"), isError)
    //}
    if (document.getElementById('txtPermissionsPrefix').value.length < 1) {
        isError = true;
        errMsg = "Please enter a value for the Permissions Filter ";
        $("#txtPermissionsPrefix").focus();
        HighlightInputsForError($("#txtPermissionsPrefix"), isError)
    }
    if (isError == true) {
        errMsg += ".";
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    // document.getElementById("hdAuthGroupId").value = $("#cmbSecAuthGroup").val();
    var perPrefix = $("#txtPermissionsPrefix").val();
    var SecAuthGroupId = $("#hdAuthGroupId").val();
    var UserId = $("#ddlLogin").val();
    $.ajax({
        url: "/ManageUser/ManageUser/GetRSAvailablePermissionsOfaAuthGroupId",
        type: 'POST',
        data: { UserId: UserId, AuthorisationGroupId: SecAuthGroupId, PermissionsPrefix: perPrefix },
        cache: false,
        success: function (data) {
            var items = '';
            var itemText = '';

            $.each(data.AvailablePermission, function (i, AvailablePermission) {
                itemText = AvailablePermission.Value + ' - ' + AvailablePermission.Text;
                items += "<option value='" + AvailablePermission.Value + "'>" + itemText + "</option>";
            });
            $('#lstSelectedAvailablePermissionValues').html(items);

            items = '';

            $.each(data.ActivePermission, function (i, ActivePermission) {
                itemText = ActivePermission.Value + ' - ' + ActivePermission.Text;
                items += "<option value='" + ActivePermission.Value + "'>" + itemText + "</option>";
            });

            $('#lstSelectedActivePermissionValues').html(items);
            $("#trAvailablePermissionCount").html("");

            items = '';
            items = "<td style='text-align: left; width: 150px;' colspan='3'>A total of <b>" + data.AvailablePermissionCount + "</b> permissions exist for this user/authgroup.<br>Enter filter criteria to view available permissions.</td> "
            $("#trAvailablePermissionCount").html(items);

            $('#tblPermissionValues').show();
            $('#fieldsetAuthorisationGroupDataAccess').show();
            $("#ErrorMsgContainer").html("");
        },
        error: function (data) {
        }
    });
}

function GetAuthIdFromDropDown() {
    document.getElementById("hdAuthGroupId").value = $("#cmbSecAuthGroup").val();


}
function addItem() {
    $("#lstSelectedAvailablePermissionValues option:selected").appendTo("#lstSelectedActivePermissionValues");
    $("#lstSelectedAvailablePermissionValues option").attr("selected", false);
}
function removeItem() {
    $("#lstSelectedActivePermissionValues option:selected").appendTo("#lstSelectedAvailablePermissionValues");
    $("#lstSelectedActivePermissionValues option").attr("selected", false);
}
function addallItems() {
    $("#lstSelectedAvailablePermissionValues option").appendTo("#lstSelectedActivePermissionValues");
    $("#lstSelectedAvailablePermissionValues option").attr("selected", false);
}
function removeallItems() {
    $("#lstSelectedActivePermissionValues option").appendTo("#lstSelectedAvailablePermissionValues");
    $("#lstSelectedActivePermissionValues option").attr("selected", false);
}
function SavaAuthGroup() {
    var SelectedActivePermission = "";
    var SelectedAvailablePermission = "";
    $("#ErrorMsgContainer").html("");


    ShowRemoveValidationMessage("Please Wait.....", "")
    var SecAuthGroupId = $("#hdAuthGroupId").val();//$("#cmbSecAuthGroup").val();
    var UserId = $("#ddlLogin").val();
    var listbox = document.getElementById("lstSelectedActivePermissionValues");
    if (listbox.options.length > 0) {
        $("#lstSelectedActivePermissionValues option").prop("selected", "selected");
        SelectedActivePermission = $("#lstSelectedActivePermissionValues").val().toString();
    }

    var listbox2 = document.getElementById("lstSelectedAvailablePermissionValues");
    if (listbox2.options.length > 0) {
        $("#lstSelectedAvailablePermissionValues option").prop("selected", "selected");
        SelectedAvailablePermission = $("#lstSelectedAvailablePermissionValues").val().toString();
    }



    $.ajax({
        url: "/ManageUser/ManageUser/SecSetUserPermissions",
        type: 'POST',
        data: { SelectedActivePermission: SelectedActivePermission, SelectedAvailablePermission: SelectedAvailablePermission, UserId: UserId, AuthorisationGroupId: SecAuthGroupId },
        cache: false,
        success: function (data) {
            // $("#lstSelectedActivePermissionValues option").prop("selected", "selected");
            $("#lstSelectedActivePermissionValues option").prop("selected", false);
            $("#lstSelectedAvailablePermissionValues option").prop("selected", false);

            $("#fieldsetAuthorisationGroup").hide();
            $("#fieldsetAuthorisationGroupDataAccess").hide();
            $("#fieldsetAuthorisationGroupWebsitePermission").hide();
            $("#fieldsetAuthorisationGroupWebPagePermission").hide();
            // ShowRemoveValidationMessage("User Data Access Updated", "Success")
            CheckForAuthorisationGroup('UserAuthorisationPermission');
        },
        error: function (data) {
            $("#ErrorMsgContainer").html("");
        }
    });
}
function AddAllCluster() {
    var SecAuthGroupId = $("#hdAuthGroupId").val();//$("#cmbSecAuthGroup").val();
    var UserId = $("#ddlLogin").val();
    $("#ErrorMsgContainer").html("");
    ShowRemoveValidationMessage("Please Wait.....", "")
    $.ajax({
        url: "/ManageUser/ManageUser/AddAllCluster",
        type: 'POST',
        data: { UserId: UserId, AuthorisationGroupId: SecAuthGroupId },
        cache: false,
        success: function (data) {
            CheckForAuthorisationGroup('UserAuthorisationPermission');
            ShowRemoveValidationMessage(data.strMessage, (data.isSuccess == true ? "Success" : "Warning"));
        },
        error: function (data) {
            $("#ErrorMsgContainer").html("");
        }
    });
}
function ManageWebsiteAccess() {

    var SecAuthGroupId = $("#hdAuthGroupId").val();// $("#cmbSecAuthGroup").val();    
    // var UserId = $("#ddlLogin").val();
    $.ajax({
        url: "/ManageUser/ManageUser/GetWebsiteUserPermission",
        type: 'POST',
        data: { AuthGroupId: SecAuthGroupId },
        //data: null,
        cache: false,
        success: function (data) {
            $("#fieldsetAuthorisationGroupWebsitePermission").show();
            $("#tblWebSite").html("");
            var item = '';
            var isAssigned = false;
            item = "<tr><td colspan='4'><b>WARNING: </b> Any change made here implement changes across ALL USERS in the associated Authorisation Group.</td></tr>"
            item += "<tr style='font-weight: bold;  '>"
            item += "    <td style='text-align: center; width: 150px; border: 1px solid #000000; background-color: #7A7989;'>Edit Permissions:</td>"
            item += "    <td style='text-align: center; width: 50px; '>&nbsp;</td>"
            item += "    <td style='text-align: center; width: 350px;  border: 1px solid #000000; background-color: #7A7989;'>Authorisation Group Access:</td>"
            item += "    <td style='text-align: center; width: 200px; '>&nbsp;</td>"
            item += "</tr>"

            $.each(data.ActiveSecWebSitePermission, function (i, ActiveSecWebSitePermission) {
                isAssigned = false;
                $.each(data.AssignedSecWebSitePermission, function (j, AssignedSecWebSitePermission) {
                    if (ActiveSecWebSitePermission.Value == AssignedSecWebSitePermission.Value) {
                        isAssigned = true;
                    }
                });


                item += "   <tr> "
                item += "    <td style='text-align: center; vertical-align: text-top;'><input type='button' id='btn" + ActiveSecWebSitePermission.Value + "'  onClick='ManageWebsitePageLavelAccess(" + ActiveSecWebSitePermission.Value + ");' value='ID=" + ActiveSecWebSitePermission.Value + "' /></td>"
                item += "    <td style='text-align: center;'>&nbsp;</td>"
                if (isAssigned == true) {
                    item += "    <td style='text-align: left; vertical-align: text-top;'><input type='checkbox' name='websiteAccess' value='" + ActiveSecWebSitePermission.Value + "'  checked >" + ActiveSecWebSitePermission.Text + "</td>"
                }
                else {
                    item += "    <td style='text-align: left; vertical-align: text-top;'><input type='checkbox' name='websiteAccess' value='" + ActiveSecWebSitePermission.Value + "' >" + ActiveSecWebSitePermission.Text + "</td>"
                }
                item += "    <td style='text-align: center; '>&nbsp;</td>"
                item += " </tr>";
            });
            item += "<tr><td colspan='4' style='text-align: right;'><input type='button' id='btnUpdateWebsiteAccess' value='Update Website Access' onClick='UpdateWebsiteAccess()' class='btnLarge' /></td></tr>"
            $("#tblWebSite").append(item);

        },
        error: function (data) {
        }
    });
}
function ManageWebsitePageLavelAccess(id) {
    var SecAuthGroupId = $("#hdAuthGroupId").val();// $("#cmbSecAuthGroup").val();
    var WebSiteId = id;
    $.ajax({
        url: "/ManageUser/ManageUser/GetWebsitePageLevelUserPermission",
        type: 'POST',
        data: { websiteid: WebSiteId, AuthGroupId: SecAuthGroupId },
        //data: null,
        cache: false,
        success: function (data) {

            $("#fieldsetAuthorisationGroupWebPagePermission").show();
            $("#tblPageLevelAccess").html("");
            var Htmlitems = '';
            Htmlitems += "<tr style='font-weight: bold;  '>"
            Htmlitems += "    <td style='text-align: center; width: 400px;  border: 1px solid #000000; background-color: #7A7989;'>Labor Rates (ID=2) Page Level Permissions:</td>"
            Htmlitems += "    <td style='text-align: center; width: 350px; '>&nbsp;</td>"
            Htmlitems += "</tr>"
            $.each(data, function (i, data) {
                Htmlitems += "    <tr> "
                Htmlitems += "    <td style='text-align: left; vertical-align: text-top;'><input type='checkbox' name='websitePerm' value='ID=" + data.WebPageId + "' >" + data.WebPageName + "</td>"
                Htmlitems += "    <td style='text-align: center; width: 280px; '>&nbsp;</td>"
                Htmlitems += " </tr>";
            });
            Htmlitems += "<tr><td colspan='2' style='text-align: right;'><input type='button' id='btnPageLevelAccess' value='Update Webpage Access' onClick='UpdatePageLevelAccess(" + WebSiteId + ")' class='btnLarge' /></td></tr>"
            $("#tblPageLevelAccess").append(Htmlitems);
        },
        error: function (data) {
        }
    });
}

function UpdateWebsiteAccess() {
    var SecAuthGroupId = $("#hdAuthGroupId").val();
    var UserId = $("#ddlLogin").val();
    var checked = "";
    var count = 0;
    var inputElems = document.getElementsByName("websiteAccess");
    for (var i = 0; i < inputElems.length; i++) {
        if (inputElems[i].type == "checkbox" && inputElems[i].checked == true) {
            checked += inputElems[i].value + ",";
        }
    }
    $.ajax({
        url: "/ManageUser/ManageUser/UpdateWebsiteAccess",
        type: 'POST',
        data: { SelectedWebSitePermission: checked, UserId: UserId, AuthorisationGroupId: SecAuthGroupId },
        //data: null,
        cache: false,
        success: function (data) {
            $("#fieldsetAuthorisationGroup").hide();
            $("#fieldsetAuthorisationGroupDataAccess").hide();
            $("#fieldsetAuthorisationGroupWebsitePermission").hide();
            $("#fieldsetAuthorisationGroupWebPagePermission").hide();
            ShowRemoveValidationMessage("Website Access Updated Successfully.", "Success")

        },
        error: function (data) {
        }
    });
}
function UpdatePageLevelAccess(WebSiteId) {
    var SecAuthGroupId = $("#hdAuthGroupId").val();// $("#cmbSecAuthGroup").val();
    var UserId = $("#ddlLogin").val();
    var checked = "";
    var count = 0;
    var inputElems = document.getElementsByName("websiteAccess");
    for (var i = 0; i < inputElems.length; i++) {
        if (inputElems[i].type == "checkbox" && inputElems[i].checked == true) {
            checked += inputElems[i].value + ",";
        }
    }
    $.ajax({
        url: "/ManageUser/ManageUser/UpdateWebPageAccess",
        type: 'POST',
        data: { SelectedWebSitePermission: checked, AuthorisationGroupId: SecAuthGroupId, WebSiteId: WebSiteId },
        //data: null,
        cache: false,
        success: function (data) {
            $("#fieldsetAuthorisationGroup").hide();
            $("#fieldsetAuthorisationGroupDataAccess").hide();
            $("#fieldsetAuthorisationGroupWebsitePermission").hide();
            $("#fieldsetAuthorisationGroupWebPagePermission").hide();
            ShowValidationMessage('WebPage Access Updated Successfully')
        },
        error: function (data) {
        }
    });
}

function CheckForAuthorisationGroup(userCheckFor) {
    $("#ErrorMsgContainer").html("");
    $("#tblUserAuthGroupStatus").html("");
    $("#txtPermissionsPrefix").val("");

    $("#tblUserAssociateAuthGroupStatus").html("");
    $("#tblUserAuthGroupStatus").html("");


    HighlightInputsForError($("#ddlLogin"), false)
    var errMsg = "";
    var isError = false;

    if (document.getElementById("ddlLogin").selectedIndex == 0) {
        isError = true;
        errMsg = "Please Select User ";
        HighlightInputsForError($("#ddlLogin"), isError)
    }
    if (isError == true) {
        errMsg += ".";
        ShowRemoveValidationMessage(errMsg, "Warning")
        return false;
    }
    var userid = $("#ddlLogin").val();
    ShowRemoveValidationMessage("Please Wait.....", "")
    $.ajax({
        url: "/ManageUser/ManageUser/GetAuthGroupByUserID",
        type: 'POST',
        data: { UserId: userid },
        cache: false,
        success: function (data) {
            var items = '';

            $("#btnAddAllClusters").hide();
            $("#tblSecAuthGroupListSelection").hide();
            $('#fieldsetAuthorisationGroupDataAccess').hide();
            $("#tblSecAuthGroupPermssionModification").hide();
            $("#tblPermissionsPrefixSearch").hide();
            $("#hdAuthGroupId").val(data.AuthGroupId);
            if (data.AuthGroupId > 0) {
                if (userCheckFor == 'UserPermission') {
                    items += "<tr><td>This <b>" + data.AuthGroupName + "</b> User currently has the following data access:<b> ";
                    items += data.strHtml;
                    items += ".</b></td></tr> ";
                    $("#tblPermissionsPrefixSearch").show();
                }
                else if (userCheckFor == 'UserAuthorisationPermission') {
                    items += "<tr><td>This User is currently associated with the <b>" + data.AuthGroupName + "</b> Authorisation Group .</td></tr>";
                    $("#tblSecAuthGroupPermssionModification").show();
                }
                if (data.isAdminUserSelected == true || data.isCPHUserSelected) {
                    $("#btnAddAllClusters").show();
                }
                $("#fieldsetAuthorisationGroup").show();
                $("#tblUserAuthGroupStatus").show();
                $("#tblUserAuthGroupStatus").html("");
                $("#tblUserAuthGroupStatus").append(items);

            }
            else {

                //$("#tblSecAuthGroupListSelection").show();
                //$("#tblPermissionsPrefixSearch").show();

                document.getElementById("hdAuthGroupId").value = $("#cmbSecAuthGroup").val();
                items = "";
                if (userCheckFor == 'UserPermission') {
                    $("#fieldsetAuthorisationGroup").show();
                    items += "<tr><td>This user is not yet associated with an Authorisation Group or Data Access Code:<b></td></tr> ";
                    $("#tblUserAuthGroupStatus").show();
                    $("#tblUserAuthGroupStatus").append(items);

                    items = "";
                    $("#tblSecAuthGroupListSelection").show();
                    items = "<tr><td>This User currently has the following data access:</td></tr> ";
                    $("#tblUserAssociateAuthGroupStatus").show();
                    $("#tblUserAssociateAuthGroupStatus").append(items);

                    $("#tblPermissionsPrefixSearch").show();


                }
                else if (userCheckFor == 'UserAuthorisationPermission') {
                    $("#fieldsetAuthorisationGroup").show();
                    items = "<tr><td>This user is not yet associated with an Authorisation Group or Data Access Code:<b></td></tr> ";
                    $("#tblSecAuthGroupListSelection").show();
                    $("#tblUserAuthGroupStatus").show();
                    $("#tblUserAuthGroupStatus").append(items);

                }




                //$("#tblUserAuthGroupStatus").show();
                //$("#tblUserAuthGroupStatus").html("");

            }
            $("#ErrorMsgContainer").html("");

            //alert($("#hdAuthGroupId").val());
        },
        error: function (data) {
        }
    });

}

function AssociateAuthorisationGroup() {
    var items = '';
    $("#btnAddAllClusters").hide();
    document.getElementById("hdAuthGroupId").value = "";
    $("#tblUserAssociateAuthGroupStatus").hide();
    $("#tblSecAuthGroupPermssionModification").hide();
    $("#tblPermissionsPrefixSearch").hide();
    var errCont = $("#ErrorMsgContainer");
    addErrorClass(errCont);
    $(errCont).html("");
    var errMsg = "";
    var isError = false;

    if (isError == true) {
        errMsg += " Please fill the mandatory fields(*).";
        $(errCont).html(errMsg);
        addErrorClass(errCont);
        return false;
    }

    document.getElementById("hdAuthGroupId").value = $("#cmbSecAuthGroup").val();

    items += "<tr><td>This <b>" + $("#cmbSecAuthGroup option:selected").text() + "</b> User currently has the following data access.</td></tr> ";
    items += ".</b></td></tr> ";



    $("#tblSecAuthGroupPermssionModification").hide();
    // $("#tblSecAuthGroupPermssionModification").show();
    $("#tblUserAssociateAuthGroupStatus").show();
    $("#tblPermissionsPrefixSearch").show();
    $("#tblUserAssociateAuthGroupStatus").html("");
    $("#tblUserAssociateAuthGroupStatus").append(items);

    if ($("#cmbSecAuthGroup option:selected").text() == "SYSTEM ADMINISTRATOR" || $("#cmbSecAuthGroup option:selected").text() == "CPH") {
        $("#btnAddAllClusters").show();
    }
}
function DeleteUserDataAccessByUserId() {
    var UserId = $("#ddlLogin").val();
    $("#ErrorMsgContainer").html("");
    ShowRemoveValidationMessage("Please Wait.....", "")
    $.ajax({
        url: "/ManageUser/ManageUser/DeleteUserDataAccessByUserId",
        type: 'POST',
        data: { UserId: UserId },
        cache: false,
        success: function (data) {

            $("#fieldsetAuthorisationGroup").hide();
            ShowRemoveValidationMessage(data.strMessage, (data.isSuccess == true ? "Success" : "Warning"));
        },
        error: function (data) {
            $("#ErrorMsgContainer").html("");
        }
    });
}