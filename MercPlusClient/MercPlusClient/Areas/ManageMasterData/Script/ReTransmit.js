// For ReTrasminnsionTool by Afroz
var optionLength;
function doAdd() {
    var str;
    var str1;
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#WorkOrder"), false);
    HighlightInputsForError($("#WorkOrderList"), false);
    str1 = document.getElementById("WorkOrder").value;
    str = trimString(str1);
    if (IsNumeric(str)) {

        var yourInt = parseInt(str);
        if (yourInt <= 2147483647) {
            document.getElementById("WorkOrder").value = "";

            optionLength = document.getElementById("WorkOrderList").options.length;

            document.getElementById("WorkOrderList").options.length = optionLength + 1;

            document.getElementById("WorkOrderList").options[optionLength].value = str;
            document.getElementById("WorkOrderList").options[optionLength].innerHTML = str;
        }
        else {
            HighlightInputsForError($("#WorkOrder"), true);
            ShowRemoveValidationMessage("WorkOrder ID range must be within 2147483647", "Warning");
            document.getElementById("WorkOrder").value = "";
        }


    }
    else {
        HighlightInputsForError($("#WorkOrder"), true);
        ShowRemoveValidationMessage("Wrong Format", "Warning");
        document.getElementById("WorkOrder").value = "";
    }


}

function trimString(str) {
    str = this != window ? this : str;
    return str.replace(/^\s+/g, '').replace(/\s+$/g, '');
}



function doRemove() {
   
    $("#ErrorMsgContainer").html("");
    HighlightInputsForError($("#WorkOrderList"), false);
    var str;
    if (document.getElementById("WorkOrderList").selectedIndex >= 0) {
        str = document.getElementById("WorkOrderList").options[document.getElementById("WorkOrderList").selectedIndex].value;
        document.getElementById("WorkOrderList").options[document.getElementById("WorkOrderList").selectedIndex] = null;
        optionLength = optionLength - 1;

    }
    else {
        HighlightInputsForError($("#WorkOrderList"), true);
        ShowRemoveValidationMessage("Select a WorkOrder to remove", "Warning");
    }
}


function getValue() {
    var str = "";
    $("#ErrorMsgContainer").html("");
    for (i = 0; i <= optionLength; i++) {

        str = str + "," + document.getElementById("WorkOrderList").options[i].value + "";
    }

    var str1 = str.substring(1, str.end);
    document.getElementById("HoldWorkOrder").value = str1;
    var url = 'RetransmitWorkOrderStatus';
    $.post(url,
        {
            WOID: document.getElementById("HoldWorkOrder").value
        },
    function (data) {

        if (data == "Success") {
            ShowRemoveValidationMessage("Retransmitted successfully", "Success");
               
        }
        else {
            ShowRemoveValidationMessage(data, "Warning");
                
        }

    });
   
}

function IsNumeric(String)
    //  check for valid numeric strings	
{
   
    var strValidChars = "0123456789";
    var strChar;
    var blnResult = true;
    var count;
    count = 0;

    var strString = String.toString();
    if (strString.length == 0) return false;


    //  test strString consists of valid characters listed above
    for (j = 0; j < strString.length && blnResult == true; j++) {

        strChar = strString.charAt(j);
        if (strValidChars.indexOf(strChar) == -1) {
            blnResult = false;


        }


    }

    return blnResult;

}

function CheckFileName() {
   
    var fileName = document.getElementById("fileupload1").value
    $("#ErrorMsgContainer").html("");
   
    if (fileName == '') {
        ShowRemoveValidationMessage("Browse to upload a valid File with xls / xlsx extension", "Warning");
        return false;
    }

    else {
        var url = 'ProcessWorkOrder';
        $.post(url,

            function (data) {
                if (data == "Retransmitted successfully") {
                    ShowRemoveValidationMessage("Retransmitted successfully", "Success");

                }
                else {
                    ShowRemoveValidationMessage(data, "Warning");

                }

            });

        return true;
    }
}


// End RetransmissionTool by Afroz