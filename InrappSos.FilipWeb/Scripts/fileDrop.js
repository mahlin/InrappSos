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
        var okToUpload = sessionOpen();
        if (okToUpload) {
        var jqXHR = $('#fileupload').fileupload('send', { files: filelist })
            .success(function (result, textStatus, jqXHR) {
                $("#filTabell tbody tr.template-upload").remove();
                updateHistory();
                $('#ddlCase').val("");
            })
            .error(function (jqXHR, textStatus, errorThrown) {/* ... */ })
            .complete(function (result, textStatus, jqXHR) {
                });
        } else {
            location.href = "/Account/Login";
        }
    });
    
});

function sessionOpen() {
    var sessionOk = false;
    $.ajax({
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        url: '/FileDrop/CheckSession',
        async: false,
        data: '{}',
        success: function (data) {
            if (data === true) {
                sessionOk = true;
            }
            else {
                sessionOk = false;
            }
        },
        error: function (xhr) {
            alert('error');
        }
    });
    return sessionOk;
}

$(document).on('change', '#ddlCase', function () {
    filelist = [];
    $("#filTabell tbody tr").remove();
    $("#thTextFildropp").text("Filer för uppladdning");
    $("#SelectedCaseId").val($('#ddlCase').val());
    if ($("#SelectedCaseId").val() !== "") {
        $('.fileinput-button').show();
    } else {
        $('.fileinput-button').hide();
    }
    updateHistory();
});

function checkIfDisabled(event) {
    //alert('click egen check');
    if ($('#fileinputButton').hasClass('disabled')) {
        $('#filesExplorerOpener').prop('disabled', true);
        $('#filesExplorerOpener').addClass('disabled');
        return false;
    } else {
        $('#filesExplorerOpener').prop('disabled', false);
        $('#filesExplorerOpener').removeClass('disabled');

    }
    return true;
}


function checkOkToUpload() {
    var okUpload = true;
    if (filelist.length > 0) {
        for (var i = 0; i < filelist.length; i++) {
            filelist[i].custom = "Arende";
            if (filelist[i].error) {
                okUpload = false;
            }
        }
        if (okUpload) {
            $('.start').prop('disabled', false);
            $('.start').show();        
        }
        else {
            $('.start').hide();
        }
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