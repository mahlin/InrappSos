var $ = jQuery;

$(document).ready(function () {

    $(function () {
        $('#ddlCase > option').each(function () {
            $(this).attr("title", $(this).text());
        });
    });

    $('.fileinput-button').hide();
    $('.start').hide();
   

    if ($("#SelectedCaseId").val() !== "0" && $("#SelectedCaseId").val() !== "" ) {
        $('.fileinput-button').show();
    }

    window.filelist = [];
    $('#fileupload').fileupload({
            // your fileupload options
        }).on("fileuploadadd",
            function (e, data) {
                for (var i = 0; i < data.files.length; i++) {
                    filelist.push(data.files[i]);
                }
                checkOkToUpload();
            })
        .on("fileuploadfail",
            function (e, data) {
                for (var i = 0; i < data.files.length; i++) {
                    filelist.splice($.inArray(data.files[i], filelist), 1);
                    //filelist.splice(data.files[i]);
                }
                checkOkToUpload();
            })
        .on('fileuploaddone',
            function (e, data) {
                //reset filelist
                filelist = [];
        });

    $('#btnSubmit').click(function () {
        var jqXHR = $('#fileupload').fileupload('send', { files: filelist })
            .success(function (result, textStatus, jqXHR) {
                $("#filTabell tbody tr.template-upload").remove();
                $("#updateHistoryForm").submit();
            })
            .error(function (jqXHR, textStatus, errorThrown) {/* ... */ })
            .complete(function (result, textStatus, jqXHR) {
            });
    });
    
});

$(document).on('change', '#ddlCase', function () {
    $("#SelectedCaseId").val($('#ddlCase').val());
    if ($("#SelectedCaseId").val() !== "") {
        $('.fileinput-button').show();
    } else {
        $('.fileinput-button').hide();
    }
    updateHistory();
});

function checkOkToUpload() {
    if (filelist.length > 0) {
        $('.start').show();
    } else {
        $('.start').hide();
    }
}

function updateHistory() {
    $.ajax({ // create an AJAX call...'        
        data: $(this).serialize(), // get the form data
        cache: false,
        type: 'post', // GET or POST
        url: $('#StartUrl').val() + '/FileDrop/RefreshFilesHistory?caseId=' + $('#SelectedCaseId').val(),
        success: function (response) { // on success..
            $("#updateHistoryForm").html(response);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('error:' + errorThrown);
        }
    });  
}

$(document).on('submit', '#updateHistoryForm', function () {
    $.ajax({ // create an AJAX call...'        
        data: $(this).serialize(), // get the form data
        cache: false,
        type: 'post', // GET or POST
        url: $('#StartUrl').val() + '/FileDrop/RefreshFilesHistory', // the file to call
        success: function (response) { // on success..
            $("#updateHistoryForm").html(response);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('error:' + errorThrown);
        }
    });
    return false; // cancel original event to prevent form submitting
});