var $ = jQuery;
var validFileCode = "";
var obligatoryRegister = true;

$(document).ready(function () {

    $(function () {
        $('#ddlRegister > option').each(function () {
            $(this).attr("title", $(this).text());
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

    $('#btnSubmit').click(function () {
        var okToUpload = sessionOpen();
        if (okToUpload) {
            var jqXHR = $('#fileupload').fileupload('send', { files: filelist })
                .success(function (result, textStatus, jqXHR) {
                    $("#filTabell tbody tr.template-upload").remove();
                    $("#updateHistoryForm").submit();
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
        url: '/FileUpload/CheckSession',
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
                //Set validFileCode
                var foundKommun = false;
                var foundLandsting = false;
                register.Organisationstyper.forEach(function (unit, ix) {
                    if (unit.Value === "Kommun") {
                        foundKommun = true;
                    }
                    else if (unit.Value === "Landsting") {
                        foundLandsting = true;
                    }
                });
                if (foundKommun) {
                    validFileCode = $("#GiltigKommunKod").val();
                }
                else if (foundLandsting) {
                    validFileCode = $("#GiltigLandstingsKod").val();
                    //Om org inte har Landsstingskod, hämta inrapporteringskod istället
                    if (validFileCode === "") {
                        validFileCode = $("#GiltigInrapporteringsKod").val();
                    }
                }
                else
                    validFileCode = $("#GiltigInrapporteringsKod").val();
                var info = "";
                if (register.Filkrav.length === 0) {
                    info = "<b>Det finns ingen pågående insamling av de valda uppgifterna.</b><br>" + register.InfoText ;
                    $('#registerInfo').html(info); 
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
                    if (register.Kortnamn === 'LVM' || register.Kortnamn === 'OJ2') {
                        info = register.InfoText + register.Filkrav[0].InfoText;
                    } else {
                        info = "<span style='color:red'>Notera att våra kontrollprogram inte accepterar <b>rubrikrad</b> i filerna. Kontrollera att dina filer inte innehåller rubrikrad innan uppladdning.</span><br>" + register.InfoText + register.Filkrav[0].InfoText;
                    }
                    register.SelectedFilkrav = register.Filkrav[0].Id;
                    $('#registerInfo').html(info);

                    // No2 - Check if obligatory for this org to report for this register
                    if (!register.Filkrav[0].ForvantadeFiler[0].Obligatorisk) {
                        obligatoryRegister = false;
                        var periodLista = register.Filkrav[0].Perioder;
                        addSelect("select-container", register.Filkrav[0].Perioder);
                        $('#ingetAttRapporteraBtn').attr("disabled", "disabled");
                        $('#ingetAttRapportera').show();
                    } else {
                        obligatoryRegister = true;
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
                if ($('#ddlPerioder').length > 0 && !obligatoryRegister) {
                    $('#ingetAttRapportera').show(); 
                }
                //Set validFileCode
                registerLista.forEach(function(register, index) {
                    if (selectedRegister === register.Id.toString()) {
                        register.Orgenheter.forEach(function (unit, ix) {
                            if (unit.Key === $("#SelectedUnitId").val()) {
                                validFileCode = unit.Value;
                            }
                        });
                    }
                });
            }
        });

        $(document).on('change', '#ddlPerioder', function () {
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
                                if (!filkrav.ForvantadeFiler[0].Obligatorisk) {
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
    //Check if desired number of files reached and no errors found => enable upload
    var errorExists = false;
    var filkravForSelectedRegister;
    for (var i = 0; i < filelist.length; i++) {
        if (filelist[i].error) {
            //TODO - refactor this
            if (filelist[i].error !== 'Alla filer i en leverans måste ha samma period') {
                errorExists = true;
            } 
            //else {
            //    filelist[i].error = null;
            //    $('.template-download').reload();
            //}
        }
    }
    var selectedRegister = $('#ddlRegister').val();
    var selectedFilkravIdStr = $('#ddlFilkrav').val();
    var selectedFilkravId = 1;
    if (selectedFilkravIdStr !== undefined) {
        selectedFilkravId = parseInt(selectedFilkravIdStr);
    }
    var numberOfFilesForSelectedRegister = 0;
    var numberOfRequiredFilesForSelectedRegister = 0;
    var numberOfNotRequiredFilesForSelectedRegister = 0;

    //get number of required files for chosen register
    registerLista.forEach(function (register, index) {
        if (selectedRegister === register.Id.toString()) {
            register.Filkrav.forEach(function(filkrav, index) {
                if (selectedFilkravId === filkrav.Id) {
                    filkravForSelectedRegister = filkrav;
                    numberOfFilesForSelectedRegister = filkrav.AntalFiler;
                    numberOfRequiredFilesForSelectedRegister = filkrav.AntalObligatoriskaFiler;
                    numberOfNotRequiredFilesForSelectedRegister = filkrav.AntalEjObligatoriskaFiler;
                }
            });
        }
    });


    if (!errorExists && filelist.length !== 0) {
        //Check if files to upload all are required
        antAddedRequiredFiles = 0;
        antAddedNotRequiredFiles = 0;
        for (var idx = 0; idx < filelist.length; idx++) {
            if (IsRequiredFile(selectedRegister, filelist[idx].name)) {
                antAddedRequiredFiles++;
            } else {
                antAddedNotRequiredFiles++;
            }
        }
        if (antAddedRequiredFiles === numberOfRequiredFilesForSelectedRegister && filelist.length <= numberOfFilesForSelectedRegister) {
            if (antAddedNotRequiredFiles === numberOfNotRequiredFilesForSelectedRegister) {
                $('#fileinputButton').prop('disabled', true);
                $('#fileinputButton').addClass('disabled');
                $('#filesExplorerOpener').prop('disabled', true);
                $('#filesExplorerOpener').addClass('disabled');
            } else {
                $('#fileinputButton').prop('disabled', false);
                $('#fileinputButton').removeClass('disabled');
                $('#filesExplorerOpener').prop('disabled', false);
                $('#filesExplorerOpener').removeClass('disabled');
            }
            //if right number of files, check that all files in upload have same period in filenamne (#374)
            var ok = true;
            if (antAddedRequiredFiles > 1) {
                var regexMatch = GetRegExpMatch(filkravForSelectedRegister, filelist[0].name);
                var periodInFirstFilename = (regexMatch[2]);
                for (var index = 1; index < filelist.length; index++) {
                    regexMatch = GetRegExpMatch(filkravForSelectedRegister, filelist[index].name);
                    var periodInFilename = (regexMatch[2]);
                    if (periodInFirstFilename !== periodInFilename) {
                        ok = false;
                        filelist[index].error = 'Alla filer i en leverans måste ha samma period';
                    }
                }
            }
            if (ok) {
                $('.start').prop('disabled', false);
                $('.start').show();
            }
        }
        else {
            $('.start').prop('disabled', true);
            $('.start').hide();
            $('#fileinputButton').prop('disabled', false);
            $('#fileinputButton').removeClass('disabled');
            $('#filesExplorerOpener').prop('disabled', false);
            $('#filesExplorerOpener').removeClass('disabled');
        }
    } else {
        $('.start').prop('disabled', true);
        $('.start').hide();
        $('#fileinputButton').prop('disabled', false);
        $('#fileinputButton').removeClass('disabled');
        $('#filesExplorerOpener').prop('disabled', false);
        $('#filesExplorerOpener').removeClass('disabled');
    }
}

function GetRegExpMatch(filkravForSelectedRegister, fileName) {
    var regexMatch = null;
    var tmp = null;
    filkravForSelectedRegister.ForvantadeFiler.forEach(function (forvFil, idx) {
        var expression = new RegExp(forvFil.Regexp, "i");
        //Kolla om filnamn matchar regex
        tmp = fileName.match(expression);
        if (tmp !== null) {
            regexMatch = tmp;
        }
    });
    return regexMatch;
}

function IsRequiredFile(selectedRegister, fileName) {
    var req = false;
    registerLista.forEach(function (register, index) {
        if (selectedRegister === register.Id.toString()) {
            var selectedFilkrav = register.SelectedFilkrav;
            register.Filkrav.forEach(function (filkrav, ix) {
                if (selectedFilkrav === filkrav.Id) {
                    filkrav.ForvantadeFiler.forEach(function (forvFil, idx) {
                        var expression = new RegExp(forvFil.Regexp, "i");
                        //Kolla om filnamn matchar regex och är obligatorisk. Refactor this!
                        tmp = fileName.match(expression);
                        if (tmp !== null) {
                            if (forvFil.Obligatorisk) {
                                req = true;
                            }
                        }
                    });
                }
            });
        }
    });
    return req;
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
