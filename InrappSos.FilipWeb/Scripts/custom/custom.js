$(document).ready(function() {
    //alert("Ready");

    $(window).load(function () {
        //alert("load");
        //setFixedTableHeader();
    });

});



//$(window).scroll(function () {
//    if ($(window).scrollTop() < 100) {
//        var height = 30;
//        //$headerStopScroll.css({ 'position': 'fixed', 'top': height + 'px' });
//        this.$headerStopScroll.css({ 'position': 'fixed' });
//    } else {
//        $headerStopScroll.css({ 'position': 'relative', 'top': 'auto' });
//    }
//});

function setFixedTableHeader() {
    var $body = $('.contentBody');
    //alert("setFixed");
    $(window).scroll(function () {
        //alert($(window).scrollTop());
        if ($.browser.msie) {
            alert("IE");
            if ($(window).scrollTop() > 40) {
                height = 196;
                $body.css({ 'position': 'fixed', 'top': height + 'px' });
            } else {
                $body.css({ 'position': 'relative', 'top': 'auto' });
            }
        } else {
            alert("Firefox");
            if ($(window).scrollTop() > 80) {
                height = 196;

                $body.css({ 'position': 'fixed', 'top': height + 'px' });
            } else {
                $body.css({ 'position': 'relative', 'top': 'auto' });
            }
        }
    });
}

