
//Dropdowner för Organisation - <enter> => sök vald organisation
$(document).on('keydown', '#ddlOrganisation', function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#searchBtn').click();
    }
});