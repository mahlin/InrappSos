﻿/*
 * jQuery File Upload Validation Plugin 1.1.3
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2013, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/* global define, require, window */
function CheckFileName(selectedRegister, fileName) {
    var result = false;
    var regexMatch = null;
    var tmp = null;
    //Hämta regexp för valt register
    if (typeof registerLista !== 'undefined') {
        registerLista.forEach(function(register, index) {
            if (selectedRegister === register.Id.toString()) {
                var selectedFilkrav = register.SelectedFilkrav;
                register.Filkrav.forEach(function(filkrav, ix) {
                    if (selectedFilkrav === filkrav.Id) {
                        filkrav.ForvantadeFiler.forEach(function(forvFil, idx) {
                            var expression = new RegExp(forvFil.Regexp, "i");
                            //Kolla om filnamn matchar regex
                            tmp = fileName.match(expression);
                            if (tmp !== null) {
                                regexMatch = tmp;
                            }
                        });
                        //filkrav.RegExper.forEach(function(regexp, idx) {
                        //    var expression = new RegExp(regexp, "i");
                        //    //Kolla om filnamn matchar regex
                        //    tmp = fileName.match(expression);
                        //    if (tmp != null) {
                        //        regexMatch = tmp;
                        //    }
                        //});
                    }
                });
            }
        });
    }
    return regexMatch;
}

function CheckKommunKodInFileName(regexMatch) {
    var validKommunKod = $('#GiltigKommunKod').val();
    var kommunKod = (regexMatch[1]);
    //var kommunKod = regexMatch.groups["kommunkod"];
    if (validKommunKod === kommunKod)
        return true;
    return false;
}

//Ny 20190403 - alla filnamn kontrolleras mot validFileCode
function CheckFileCodeInFileName(regexMatch) {
    var filkod = (regexMatch[1]);
    if (validFileCode === filkod)
        return true;
    return false;
}

function CheckFilkodInFileName(selectedOrgUnitId, regexMatch) {
    var filkod = (regexMatch[1]);
    if (selectedOrgUnitId === filkod)
        return true;
    return false;
}

//function CheckFilkodInFileName(selectedRegister, selectedOrgUnitId, regexMatch) {
//    var filkod = (regexMatch[1]);
//    var validFilkod = "";
//    registerLista.forEach(function (register, index) {
//        if (selectedRegister === register.Id.toString()) {
//            register.Orgenheter.forEach(function (unit, ix) {
//                var x = unit.Value;
//                var y = unit.Key;
//                if (selectedOrgUnitId === unit.Key) {
//                    validFilkod = unit.Value;
//                }
//            });
//        }
//    });

//    if (validFilkod === filkod)
//        return true;
//    return false;
//}



function CheckPeriodInFileName(selectedRegister, regexMatch) {
    //var periodInFilename = regexMatch.groups["period"];
    var periodInFilename = (regexMatch[2]);
    //Get valid period for selected register
    var validPeriod = "";
    registerLista.forEach(function (register, index) {
        if (selectedRegister === register.Id.toString()) {
            var selectedFilkrav = register.SelectedFilkrav;

            register.Filkrav.forEach(function (filkrav, ix) {
                if (selectedFilkrav === filkrav.Id) {
                    validPeriod = filkrav.Perioder;
                }
            });
        }
    });
    return CheckPeriod(periodInFilename, validPeriod);
}

function arrayContains(needle, arrhaystack) {
    return (arrhaystack.indexOf(needle) > -1);
}

function CheckPeriod(periodInFilename, validPeriods) {
    result = false;
    validPeriods.forEach(function (validPeriod, index) {
        if (periodInFilename === validPeriod) {
            result = true;
            $("#SelectedPeriod").val(periodInFilename);
        }
    });
    return result;
}

//Kontrollera så att ej fler filer av samma filtyp laddas upp i samma leverans 
function DoubletFiles(selectedRegister, fileName) {
    var re;
    var x = window.filelist;
    var tmp = null;
    var antHits = 0;

    registerLista.forEach(function (register, index) {
        //alert("index:" + index + ", valt register: " + selectedRegister + ", regsiterId: " + register.Id.toString());
        if (selectedRegister === register.Id.toString()) {
            var selectedFilkrav = register.SelectedFilkrav;
            register.Filkrav.forEach(function(filkrav, ix) {
                if (selectedFilkrav === filkrav.Id) {
                    filkrav.ForvantadeFiler.forEach(function (forvFil, idx) {
                        var expression = new RegExp(forvFil.Regexp, "i");
                        //Kolla om filnamn matchar regex
                        tmp = fileName.match(expression);
                        //Om träff, kolla om nån annan fil i listan matchar samma regex
                        if (tmp !== null) {
                            window.filelist.forEach(function (file, i) {
                                //alert("Regexp" + idx + ": " + regexp);
                                if (expression.test(file.name)) {
                                    antHits++;
                                }
                            });
                        }
                    });
                }
            });
        }
    });
    if (antHits > 1)
        return true;
    return false;
}

//function DoubletFiles2(selectedRegister) {
//    var re;
//    var result = false;
//    var x = window.filelist;

//    registerLista.forEach(function (register, index) {
//        //alert("index:" + index + ", valt register: " + selectedRegister + ", regsiterId: " + register.Id.toString());
//        if (selectedRegister === register.Id.toString()) {
//            var selectedFilkrav = register.SelectedFilkrav;
//            register.Filkrav.forEach(function (filkrav, ix) {
//                if (selectedFilkrav === filkrav.Id) {
//                    //Om valt filkrav har fler regularexpressions att uppfylla, kolla att bara en fil för varje regexp laddas upp
//                    if (filkrav.ForvantadeFiler.length > 1) {
//                        var arrUsedRegexp = new Array(filkrav.ForvantadeFiler.length);

//                        for (var i = 0; i < arrUsedRegexp.length; ++i) {
//                            arrUsedRegexp[i] = false;
//                        }

//                        filkrav.ForvantadeFiler.forEach(function (forvFil, idx) {
//                            var re = new RegExp(forvFil.Regexp, "i");

//                            window.filelist.forEach(function (file, i) {
//                                //alert("Regexp" + idx + ": " + regexp);
//                                if (re.test(file.name)) {
//                                    if (arrUsedRegexp[idx] === true)
//                                        result = true;
//                                    else
//                                        arrUsedRegexp[idx] = true;
//                                }
//                            });
//                        });
//                    }
//                }
//            });
//        }
//    });
//    return result;
//}

function getTableRows() {
    return $('#filTabell tr');
};


(function (factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // Register as an anonymous AMD module:
        define([
            'jquery',
            './jquery.fileupload-process'
        ], factory);
    } else if (typeof exports === 'object') {
        // Node/CommonJS:
        factory(require('jquery'));
    } else {
        // Browser globals:
        factory(
            window.jQuery
        );
    }
}(function ($) {
    'use strict';

    // Append to the default processQueue:
        $.blueimp.fileupload.prototype.options.processQueue.push(
            {
                action: 'validate',
                // Always trigger this action,
                // even if the previous action was rejected: 
                always: true,
                // Options taken from the global options map:
                acceptFileTypes: '@',
                maxFileSize: '@',
                minFileSize: '@',
                maxNumberOfFiles: '@',
                incorrectFileName: '@',
                incorrectKommunKodInFileName: '@',
                incorrectFilkodInFileName: '@',
                incorrectPeriodInFileName: '@',
                filetypeAlreadySelected: '@',
            disabled: '@disableValidation'
        }
    );

    // The File Upload Validation plugin extends the fileupload widget
    // with file validation functionality:
    $.widget('blueimp.fileupload', $.blueimp.fileupload, {

        options: {
            
            // The regular expression for allowed file types, matches
            // against either file type or file name:
            acceptFileTypes: /(\.|\/)(txt|xls|xlsx|pdf)$/i,
            // The maximum allowed file size in bytes:
            maxFileSize: 1000000000, // 1000 MB = 1GB
            // The minimum allowed file size in bytes:
            minFileSize: 1, // No minimal file size
            // The limit of files to be uploaded:
            //maxNumberOfFiles: 10,
            

            // Function returning the current number of files,
            // has to be overriden for maxNumberOfFiles validation:
            getNumberOfFiles: $.noop,

            // Error and info messages:
            messages: {
                maxNumberOfFiles: 'Maximalt antal filer har överskridits',
                acceptFileTypes: 'Felaktig filtyp',
                maxFileSize: 'Filen är för stor',
                minFileSize: ('Filen är tom'),
                incorrectFileName: ('Felaktigt filnamn'),
                incorrectKommunKodInFileName: ('Felaktig kommunkod i filnamnet'),
                incorrectFilkodInFileName: ('Felaktig filkod i filnamnet'),
                incorrectPeriodInFileName: ('Felaktig period i filnamnet'),
                filetypeAlreadySelected: ('En fil av denna typ är redan vald'),
            }
        },

        processActions: {

            validate: function (data, options) {
                if (options.disabled) {
                    return data;
                }
                var dfd = $.Deferred(),
                    settings = this.options,
                    file = data.files[data.index],
                    fileSize;

                if (options.minFileSize || options.maxFileSize) {
                    fileSize = file.size;
                }
                if ($.type(options.maxNumberOfFiles) === 'number' &&
                        (settings.getNumberOfFiles() || 0) + data.files.length >
                            options.maxNumberOfFiles) {
                    file.error = settings.i18n('maxNumberOfFiles');
                } else if (options.acceptFileTypes &&
                        !(options.acceptFileTypes.test(file.type) ||
                        options.acceptFileTypes.test(file.name))) {
                    file.error = settings.i18n('acceptFileTypes');
                } else if (fileSize > options.maxFileSize) {
                    file.error = settings.i18n('maxFileSize');
                } else if ($.type(fileSize) === 'number' &&
                    fileSize < options.minFileSize) {
                    file.error = settings.i18n('minFileSize');
                }

                if (!file.error && file.custom !== "Mall") {
                    //Regexp-kontroller
                    var currRegister = file.name.substring(0, 3);
                    var selectedOrgUnitId = $('#SelectedUnitId').val();

                    var regexMatch = CheckFileName(data.selectedRegister, file.name);
                    if (regexMatch === null) {
                        file.error = settings.i18n('incorrectFileName');
                    }
                    else if (!CheckFileCodeInFileName(regexMatch)) {
                    //else if ((currRegister === 'CAN' || currRegister === 'OV_' || currRegister === 'SV_') && !CheckFilkodInFileName(selectedOrgUnitId, regexMatch)) {
                    //else if ((currRegister === 'CAN' || currRegister === 'OV_') && !CheckFilkodInFileName(data.selectedRegister, selectedOrgUnitId, regexMatch)) {
                        file.error = settings.i18n('incorrectFilkodInFileName');
                    //} else if ((currRegister !== 'LVM' && currRegister !== 'CAN' && currRegister !== 'OV_' && currRegister !== 'SV_') && !CheckKommunKodInFileName(regexMatch)) {
                    //    file.error = settings.i18n('incorrectKommunKodInFileName');
                    }
                    else if (!CheckPeriodInFileName(data.selectedRegister, regexMatch)) {
                        file.error = settings.i18n('incorrectPeriodInFileName');
                    } else if (DoubletFiles(data.selectedRegister, file.name)) {
                        file.error = settings.i18n('filetypeAlreadySelected');
                    } else {
                        delete file.error;
                    }
                }
                if (file.error || data.files.error) {
                    data.files.error = true;
                    $('.start').prop('disabled', true);
                    $('.start').hide();
                    $('#fileinputButton').prop('disabled', false);
                    $('#fileinputButton').removeClass('disabled');
                    $('#filesExplorerOpener').prop('disabled', false);
                    $('#filesExplorerOpener').removeClass('disabled');
                    //var selectedRegister = $('#ddlRegister').val();
                    //var numberOfFilesForSelectedRegister = 0;
                    //var numberOfRequiredFilesForSelectedRegister = 0;
                    ////get number of required files for chosen register
                    //registerLista.forEach(function (register, index) {
                    //    if (selectedRegister === register.Id.toString()) {
                    //        var selectedFilkrav = register.SelectedFilkrav;
                    //        register.Filkrav.forEach(function(filkrav, ix) {
                    //            if (selectedFilkrav === filkrav.Id) {
                    //                numberOfFilesForSelectedRegister = filkrav.AntalFiler;
                    //                numberOfRequiredFilesForSelectedRegister = filkrav.AntalObligatoriskaFiler;
                    //            }
                    //        });
                    //    }
                    //});
                    //if (antAddedRequiredFiles === numberOfRequiredFilesForSelectedRegister && filelist.length <= numberOfFilesForSelectedRegister) {
                    //    $('#fileinputButton').prop('disabled', true);
                    //    $('#fileinputButton').addClass('disabled');
                    //    //$('.fileinput-button')
                    //    //    .prop('disabled', true)
                    //    //    .parent().addClass('disabled');
                    //}
                    dfd.rejectWith(this, [data]);
                } else {
                    dfd.resolveWith(this, [data]);
                }
                return dfd.promise();
            }

        }

    });

    }

));



