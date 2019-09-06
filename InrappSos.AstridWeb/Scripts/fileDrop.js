var $ = jQuery;

$(document).ready(function () {

    $('.fileinput-button').show();
    $('.start').hide();

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
            })
            .error(function (jqXHR, textStatus, errorThrown) {/* ... */ })
            .complete(function (result, textStatus, jqXHR) {
            });
    });
    
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
    if (filelist.length > 0) {
        for (var i = 0; i < filelist.length; i++) {
            filelist[i].custom = "Mall";
        }
        $('.start').prop('disabled', false);
        $('.start').show();
    } else {
        $('.start').hide();
    }
}
