var $ = jQuery;

$(document).ready(function () {

    $(function () {
        $('#ddlRegister > option').each(function () {
            $(this).attr("title", $(this).text());
            //alert($(this).text() + ' ' + $(this).val() + ' ' + $(this).attr("title"));
        });
    });

    $('.fileinput-button').hide();
    $('.start').hide();

    window.filelist = [];
    $('#fileupload').fileupload({
        // your fileupload options
    }).on("fileuploadadd",
        function(e, data) {
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
            //TODO - detta görs även i jquery.fileupload.js => dubblerad kod, fixa
            checkOkToUpload();
           
        })
        .on('fileuploaddone',
        function (e, data) {
            //reset filelist
            filelist = [];
        });

    $('#btnSubmit').click(function() {
        var jqXHR = $('#fileupload').fileupload('send', { files: filelist })
            .success(function (result, textStatus, jqXHR) {
                    $("#filTabell tbody tr.template-upload").remove();
                    $("#updateHistoryForm").submit();
            })
            .error(function (jqXHR, textStatus, errorThrown) {/* ... */ })
            .complete(function(result, textStatus, jqXHR) {
            });;
    });

});



$(document).on('submit', '#updateHistoryForm', function () {
    $.ajax({ // create an AJAX call...'        
        data: $(this).serialize(), // get the form data
        cache: false,
        type: 'post', // GET or POST
        url: $('#StartUrl').val() + '/FileUpload/RefreshFilesHistory', // the file to call
        success: function (response) { // on success..
            $("#updateHistoryForm").html(response);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('error:' + errorThrown);
        }
    });
    return false; // cancel original event to prevent form submitting
});


$(document).on('change','#ddlRegister',
    function() {
        var selectedRegister = $('#ddlRegister').val();

        $('#fileupload').fileupload(
            'option',
            'selectedRegister',
            selectedRegister
        );

        filelist = [];
        $("#filTabell tbody tr").remove();
        $("#thText").text("Filer för uppladdning");
        $("#SelectedRegisterId").val(selectedRegister);
        $('#IngetAttRapporteraForRegisterId').val(selectedRegister);
        
        registerLista.forEach(function (register, index) {
            if (selectedRegister === register.Id.toString()) {
                if (register.Filkrav.length === 0) {
                    $('#registerInfo').html("Det finns ingen pågående insamling av de valda uppgifterna."); 
                    $('.fileinput-button').hide();
                    $('.start').hide();
                    $('#ingetAttRapportera').hide();
                    $('#enhetsInfo').hide();
                }
                //No1 - Check if more than one filkrav => show dropdown to allow user to make choice
                else if (register.Filkrav.length > 1) {
                    $('.fileinput-button').hide();
                    $('.start').hide();
                    $('#registerInfo').html("");
                    $('#ingetAttRapportera').hide();
                    $('#enhetsInfo').hide();
                    //visa dropdown för filkrav
                    addSelectFilkrav("FileRequirementsSelect-container", register.Filkrav);
                    $('#parallellaForeskrifter').show();
                } else {
                    $('.fileinput-button').show();
                    $('#parallellaForeskrifter').hide();
                    var info = register.InfoText + register.Filkrav[0].InfoText;
                    register.SelectedFilkrav = register.Filkrav[0].Id;
                    $('#registerInfo').html(info);

                    // No2 - Check if obligatory for this org to report for this register
                    if (!register.Filkrav[0].Obligatorisk) {
                        var periodLista = register.Filkrav[0].Perioder;
                        addSelect("select-container", register.Filkrav[0].Perioder);
                        $('#ingetAttRapporteraBtn').attr("disabled", "disabled");
                        $('#ingetAttRapportera').show();
                    } else {
                        $('#ingetAttRapportera').hide();
                    }
                    // No3 - Check if organisation is supposed to leave files per unit
                    if (register.RapporterarPerEnhet) {
                        //Populate unit-dropdown
                        var vals = {};
                        register.Organisationsenheter.forEach(function(unit, index) {
                            vals[unit.Key] = unit.Value;
                        });

                        var $ddlUnits = $("#ddlUnits");
                        $ddlUnits.empty();
                        $ddlUnits.append("<option> - Välj enhet - </option>");
                        $.each(vals,
                            function(index, value) {
                                $ddlUnits.append("<option value=" + index + " >" + value + "</option>");
                                //alert(index + ' : ' + value);
                            });

                        $('#enhetsInfo').show();
                        $('#ingetAttRapportera').hide();
                        $('.fileinput-button').hide();
                        $('#fileinputButton').prop('disabled', true);
                        $('#fileinputButton').addClass('disabled');
                        $('#fileinputButton').parent().addClass('disabled');
                        $('#fileinputButton').prop('readonly', true);
                        $('#fileinputButton').addClass('readonly');
                        //$('.fileinput-button')
                        //    .prop('disabled', true)
                        //    .parent().addClass('disabled');
                        $('.start').hide();
                    } else {
                        $('#enhetsInfo').hide();
                        $('.fileinput-button').show();
                        $('#fileinputButton').prop('disabled', false);
                        $('#fileinputButton').removeClass('disabled');
                        $('#filesExplorerOpener').prop('disabled', false);
                        $('#filesExplorerOpener').removeClass('disabled');
                        //$('.fileinput-button')
                        //    .prop('disabled', false)
                        //    .parent().removeClass('disabled');
                        $('.start').hide();
                    }
                }
            } else if (selectedRegister === ""){
                $('#registerInfo').html("");
                $('.fileinput-button').hide();
                $('.start').hide();
            }
        });

        $(document).on('change', '#ddlUnits', function () {
            var selectedIndex = $('#ddlUnits').prop('selectedIndex');
            var x = $('#ddlUnits').val();
            $("#SelectedUnitId").val($('#ddlUnits').val());
            $("#IngetAttRapporteraForSelectedUnitId").val($('#ddlUnits').val());
            var y = $("#SelectedUnitId").val();
            //alert($('#ddlUnits').prop('selectedIndex'));
            if (selectedIndex === 0) {
                $('.fileinput-button').hide();
                $('#fileinputButton').prop('disabled', true);
                $('#fileinputButton').addClass('disabled');
                $('#filesExplorerOpener').prop('disabled', true);
                $('#filesExplorerOpener').addClass('disabled');
                $('#ingetAttRapportera').hide();
                //$('.fileinput-button')
                //    .prop('disabled', true)
                //    .parent().addClass('disabled');
                $('.start').hide();
            } else {
                $('.fileinput-button').show();
                $('#fileinputButton').prop('disabled', false);
                $('#fileinputButton').removeClass('disabled');
                $('#filesExplorerOpener').prop('disabled', false);
                $('#filesExplorerOpener').removeClass('disabled');
                if ($('#ddlPerioder').length > 0) {
                    $('#ingetAttRapportera').show(); 
                }
                //$('.fileinput-button')
                //    .prop('disabled', false)
                //    .parent().removeClass('disabled');
            }
        });

        $(document).on('change', '#ddlPerioder', function () {
            var x = $('#ddlPerioder').val();
            $("#IngetAttRapporteraForPeriod").val($('#ddlPerioder').val());
            $("#IngetAttRapporteraForRegisterId").val($('#SelectedRegisterId').val());
            if ($('#ddlPerioder').val() !== "0") {
                $('.fileinput-button').hide();
                $('#fileinputButton').prop('disabled', true);
                $('#ingetAttRapporteraBtn').attr("disabled", false);
            } else {
                $('#ingetAttRapporteraBtn').attr("disabled", "disabled");
                $('.fileinput-button').show();
                $('#fileinputButton').prop('disabled', false);
            }
        });

        $(document).on('change', '#ddlFilkrav', function () {
            var selectedFilkravId = parseInt($('#ddlFilkrav').val());
            if (selectedFilkravId !== 0) {
                var selectedRegister = $("#SelectedRegisterId").val();
                registerLista.forEach(function(register, index) {
                    if (selectedRegister === register.Id.toString()) {
                        register.Filkrav.forEach(function(filkrav, index) {
                            if (selectedFilkravId === filkrav.Id) {
                                var info = "<br><br><br><br>" + register.InfoText + filkrav.InfoText;
                                register.SelectedFilkrav = selectedFilkravId;
                                $('#registerInfo').html(info);
                                //Check if obligatory for this org to report for this register
                                if (!filkrav.Obligatorisk) {
                                    var periodLista = filkrav.Perioder;
                                    addSelect("select-container", filkrav.Perioder);
                                    $('#ingetAttRapportera').show();
                                } else {
                                    $('#ingetAttRapportera').hide();
                                }
                                $('.fileinput-button').show();
                                $('#fileinputButton').prop('disabled', false);
                                $('#fileinputButton').removeClass('disabled');
                                $('#filesExplorerOpener').prop('disabled', false);
                                $('#filesExplorerOpener').removeClass('disabled');
                            }
                        });
                    }
                });
            } else {
                $('.fileinput-button').hide();
                $('#ingetAttRapportera').hide();
                $('#registerInfo').html("");
            }
        });

        //$(document).on('change','#ddlFileRequirements',function() {
        //    var selectedFilkrav = $('#ddlFileRequirements').val();
        //    $("#SelectedFilkrav").val(selectedFilkrav);
        //});

    });


function addSelectFilkrav(divname, filkrav) {
    //$('#IngetAttRapporteraForPeriod').val(namn[0]);
    var newDiv = document.createElement('div');
    var html = ' <span style="white-space: nowrap; width: 360px;display: inline-block;">Typ av rapportering: &nbsp;&nbsp<select id="ddlFilkrav" class="form-control ddl" style="width:175px;display:inline-block;padding-left:10px;">', i;
    html += "<option value='0'> - Välj - </option>";
    for (i = 0; i < filkrav.length; i++) {
        html += "<option value='" + filkrav[i].Id + "'>" + filkrav[i].Namn + "</option>";
    }
    html += '</select></span>';
    newDiv.innerHTML = html;
    document.getElementById(divname).innerHTML = newDiv.innerHTML;
    //document.getElementById(divname).appendChild(newDiv);
}

function addSelect(divname, perioder) {
    $('#IngetAttRapporteraForPeriod').val(perioder[0]);
    //var x = $('#SelectedRegister').val();
    var y = $('#IngetAttRapporteraForRegisterId').val();
    //$('#IngetAttRapporteraForRegisterId').val($('#SelectedRegister').val());
    var newDiv = document.createElement('div');
    var html = ' <span style="white-space: nowrap">Inget att rapportera för period: &nbsp;&nbsp;<select id="ddlPerioder" class="form-control ddl" style="width:95px;display:inline-block;padding-left:10px;">', i;
    html += "<option value='0'> - Välj - </option>";
    for (i = 0; i < perioder.length; i++) {
        html += "<option value='" + perioder[i] + "'>" + perioder[i] + "</option>";
    }
    html += '</select></span>';
    newDiv.innerHTML = html;
    document.getElementById(divname).innerHTML = newDiv.innerHTML;
    //document.getElementById(divname).appendChild(newDiv);
}

//function dateGenerate() {
//    var date = new Date(), dateArray = new Array(), i;
//    curYear = date.getFullYear();
//    for (i = 0; i < 5; i++) {
//        dateArray[i] = curYear + i;
//    }
//    return dateArray;
//}

//function periodGenerate() {
//    var date = new Date(), dateArray = new Array(), i;
//    curYear = date.getFullYear();
//    for (i = 0; i < 5; i++) {
//        dateArray[i] = curYear + i;
//    }
//    return dateArray;
//}

function checkIfDisabled(event) {
    //alert('click egen check');
    var x = $('#fileinputButton');
    var y = $('#filesExplorerOpener');
    //if ($('#fileinputButton').attr('disabled', 'true')) {
    //    return false;
    //}
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
    //Check if desired number of files reached and no errors found => enable upload
    var errorExists = false;
    for (var i = 0; i < filelist.length; i++) {
        if (filelist[i].error) {
            errorExists = true;
        }
    }
    var selectedRegister = $('#ddlRegister').val();
    var selectedFilkravIdStr = $('#ddlFilkrav').val();
    var selectedFilkravId = 1;
    if (selectedFilkravIdStr !== undefined) {
        selectedFilkravId = parseInt(selectedFilkravIdStr);
    }
    var numberOfFilesForSelectedRegister = 0;
    //get number of required files for chosen register
    registerLista.forEach(function (register, index) {
        if (selectedRegister === register.Id.toString()) {
            register.Filkrav.forEach(function(filkrav, index) {
                if (selectedFilkravId === filkrav.Id) {
                    numberOfFilesForSelectedRegister = filkrav.AntalFiler;
                }
            });
        }
    });
    if (filelist.length === numberOfFilesForSelectedRegister && !errorExists) {
        $('.start').prop('disabled', false);
        $('.start').show();
        $('#fileinputButton').prop('disabled', true);
        $('#fileinputButton').addClass('disabled');
        $('#filesExplorerOpener').prop('disabled', true);
        $('#filesExplorerOpener').addClass('disabled');
    } else {
        $('.start').prop('disabled', true);
        $('.start').hide();
        $('#fileinputButton').prop('disabled', false);
        $('#fileinputButton').removeClass('disabled');
        $('#filesExplorerOpener').prop('disabled', false);
        $('#filesExplorerOpener').removeClass('disabled');
    }
}

//function disableFileInputButton() {
//    $('.fileinput-button')
//        .prop('disabled', true)
//        .parent().addClass('disabled');
//}

//function isIE() {
//    var ua = window.navigator.userAgent;
//    var msie = ua.indexOf('MSIE '); // IE 10 or older
//    var trident = ua.indexOf('Trident/'); //IE 11

//    return (msie > 0 || trident > 0);
//}
