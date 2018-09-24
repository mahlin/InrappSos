/*
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
    //Hämta regexp för valt register
    registerLista.forEach(function(register, index) {
        if (selectedRegister === register.Id.toString()) {
            var selectedFilkrav = register.SelectedFilkrav;
            register.Filkrav.forEach(function(filkrav, ix) {
                if (selectedFilkrav === filkrav.Id) {
                    filkrav.RegExper.forEach(function(regexp, idx) {
                        var expression = new RegExp(regexp, "i");
                        //Kolla om filnamn matchar regex
                        regexMatch = fileName.match(expression);
                    });
                }
            });
        }
    });
    return regexMatch;
}


function CheckFileName2(selectedRegister, fileName) {
    var re;
    var result = false;
    //Hämta regexp för valt register
    registerLista.forEach(function (register, index) {
        if (selectedRegister === register.Id.toString()) {
            var selectedFilkrav = register.SelectedFilkrav;
            register.Filkrav.forEach(function (filkrav, ix) {
                if (selectedFilkrav === filkrav.Id) {
                    filkrav.RegExper.forEach(function (regexp, idx) {
                        re = new RegExp(regexp, "i");
                        var expression = new RegExp(regexp, "i");
                        var res = fileName.match(expression);
                        if (res !== null) {
                            result = true; //Filnamn ok utifrån regexp
                            //Kontrollera kommunkod
                            var kommunkod = res.groups["kommunkod"];
                            result = CheckKommunKod(kommunkod);
                            //Kontrollera period
                            var periodInFilename = res.groups["period"];
                            //Hämta giltiga perioder för valt register
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
                            result = CheckPeriod(periodInFilename, validPeriod);
                        }
                        //if (re.test(fileName)) {
                        //    result = true; //Filnamn ok utifrån regexp
                        //}
                    });
                }
            })
            //re = new RegExp(register.RegExp, "i");
        }
    });
    return result;
}

//function CheckFileName(selectedRegister, fileName) {
//    var re;
//    var result = false;
//    //Hämta regexp för valt register
//    registerLista.forEach(function (register, index) {
//        if (selectedRegister === register.Id.toString()) {
//            var selectedFilkrav = register.SelectedFilkrav;
//            register.Filkrav.forEach(function (filkrav, ix) {
//                if (selectedFilkrav === filkrav.Id) {
//                    filkrav.RegExper.forEach(function (regexp, idx) {
//                        re = new RegExp(regexp, "i");
//                        if (re.test(fileName)) {
//                            result = true;
//                        }
//                    });
//                }})
//            //re = new RegExp(register.RegExp, "i");
//        }
//    });
//    return result;
//}

////TODO - använd SelectedRegisterId/kortnamn istället?
//function CheckKommunKodInFileName2(fileName) {
//    var chunkedFileName = fileName.split("_");
//    var fileTypeA = [ 'SOL1', 'SOL2', 'KHSL','KHSL1','KHSL2','LSS'];
//    var fileTypeB = ['BU'];
//    var fileTypeC = ['EKB'];


//    if (arrayContains(chunkedFileName[0].toUpperCase(), fileTypeA)) {
//        return CheckKommunKod(chunkedFileName[1]);
//    }
//    else if (arrayContains(chunkedFileName[0].toUpperCase(), fileTypeB)) {
//        return CheckKommunKod(chunkedFileName[2]);
//    }
//    else if (arrayContains(chunkedFileName[0].toUpperCase(), fileTypeC)) {
//        if (chunkedFileName[1].toUpperCase() === 'AO')
//                return CheckKommunKod(chunkedFileName[2]);
//            else
//                return CheckKommunKod(chunkedFileName[1]);
//    }

//}

function CheckKommunKodInFileName(regexMatch) {
    var validKommunKod = $('#GiltigKommunKod').val();
    var kommunKod = regexMatch.groups["kommunkod"];
    if (validKommunKod === kommunKod)
        return true;
    return false;
}

function CheckPeriodInFileName(selectedRegister, regexMatch) {
    var periodInFilename = regexMatch.groups["period"];
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

//function CheckPeriodInFileName2(selectedRegister, fileName) {
//    var chunkedFileName = fileName.split("_");
//    var fileTypeA = ['SOL1', 'SOL2', 'KHSL','KHSL1','KHSL2','LSS'];
//    var fileTypeB = ['BU'];
//    var fileTypeC = ['EKB'];

//    //Get valid period for selected register
//    var validPeriod = "";
//    registerLista.forEach(function (register, index) {
//        if (selectedRegister === register.Id.toString()) {
//            var selectedFilkrav = register.SelectedFilkrav;
            
//            register.Filkrav.forEach(function (filkrav, ix) {
//                if (selectedFilkrav === filkrav.Id) {
//                    validPeriod = filkrav.Perioder;
//                }
//            });
//        }
//    });

//    if (arrayContains(chunkedFileName[0].toUpperCase(), fileTypeA)) {
//        return CheckPeriod(chunkedFileName[2], validPeriod);
//    }
//    else if (arrayContains(chunkedFileName[0].toUpperCase(), fileTypeB)) {
//        return CheckPeriod(chunkedFileName[3], validPeriod);
//    }
//    else if (arrayContains(chunkedFileName[0].toUpperCase(), fileTypeC)) {
//        if (chunkedFileName[1].toUpperCase() === 'AO')
//            return CheckPeriod(chunkedFileName[3], validPeriod);
//        else
//            return CheckPeriod(chunkedFileName[2], validPeriod);
//    }
//}

function CheckPeriod(periodInFilename, validPeriods) {
    result = false;
    validPeriods.forEach(function (validPeriod, index) {
        if (periodInFilename === validPeriod) {
            result = true;
            $("#SelectedPeriod").val(periodInFilename);
        }
    });
    return result;

    //if (periodInFilename === validPeriod)
    //    return true;
    //return false;
}

function DoubletFiles(selectedRegister) {
    var re;
    var result = false;
    var x = window.filelist;

    registerLista.forEach(function (register, index) {
        //alert("index:" + index + ", valt register: " + selectedRegister + ", regsiterId: " + register.Id.toString());
        if (selectedRegister === register.Id.toString()) {
            var selectedFilkrav = register.SelectedFilkrav;
            register.Filkrav.forEach(function(filkrav, ix) {
                if (selectedFilkrav === filkrav.Id) {
                    //Om valt filkrav har fler regularexpressions att uppfylla, kolla att bara en fil för varje regexp laddas upp
                    if (filkrav.RegExper.length > 1) {
                        var arrUsedRegexp = new Array(filkrav.RegExper.length);

                        for (var i = 0; i < arrUsedRegexp.length; ++i) {
                            arrUsedRegexp[i] = false;
                        }

                        filkrav.RegExper.forEach(function(regexp, idx) {
                            var re = new RegExp(regexp, "i");

                            window.filelist.forEach(function(file, i) {
                                //alert("Regexp" + idx + ": " + regexp);
                                if (re.test(file.name)) {
                                    if (arrUsedRegexp[idx] === true)
                                        result = true;
                                    else
                                        arrUsedRegexp[idx] = true;
                                }
                            });
                        });
                    }
                }
            });
        }
    });
    return result;
}

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
                incorrectPeriodInFileName: '@',
                filetypAlreadySelected: '@',
            disabled: '@disableValidation'
        }
    );

    // The File Upload Validation plugin extends the fileupload widget
    // with file validation functionality:
    $.widget('blueimp.fileupload', $.blueimp.fileupload, {

        options: {
            
            // The regular expression for allowed file types, matches
            // against either file type or file name:
            acceptFileTypes: /(\.|\/)(txt|xls|xlsx)$/i,
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
                incorrectPeriodInFileName: ('Felaktig period i filnamnet'),
                filetypAlreadySelected: ('En fil av denna typ är redan vald'),
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
                //Regexp-kontroller
                var regexMatch = CheckFileName(data.selectedRegister, file.name);
                if (regexMatch === null) {
                    file.error = settings.i18n('incorrectFileName');
                } else if (!CheckKommunKodInFileName(regexMatch)) {
                    file.error = settings.i18n('incorrectKommunKodInFileName');
                } else if (!CheckPeriodInFileName(data.selectedRegister, regexMatch)) {
                    file.error = settings.i18n('incorrectPeriodInFileName');
                } else if (DoubletFiles(data.selectedRegister)) {
                    file.error = settings.i18n('filetypAlreadySelected');
                } else {
                    delete file.error;
                }
                if (file.error || data.files.error) {
                    data.files.error = true;
                    $('.start').hide();
                    var selectedRegister = $('#ddlRegister').val();
                    var numberOfFilesForSelectedRegister = 0;
                    //get number of required files for chosen register
                    registerLista.forEach(function (register, index) {
                        if (selectedRegister === register.Id.toString()) {
                            var selectedFilkrav = register.SelectedFilkrav;
                            register.Filkrav.forEach(function(filkrav, ix) {
                                if (selectedFilkrav === filkrav.Id) {
                                    numberOfFilesForSelectedRegister =
                                        filkrav.AntalFiler;
                                }
                            });
                        }
                    });
                    if (filelist.length === numberOfFilesForSelectedRegister) {
                        $('#fileinputButton').prop('disabled', true);
                        $('#fileinputButton').addClass('disabled');
                        //$('.fileinput-button')
                        //    .prop('disabled', true)
                        //    .parent().addClass('disabled');
                    }
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



