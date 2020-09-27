var myVar;

function loader() {
    myVar = setTimeout(showPage, 1000);
}

function showPage() {
    document.getElementById("loader").style.display = "none";
    document.body.style.backgroundImage = "none";
    document.getElementById("myDiv").style.display = "block";
}

$('#scrollbar').slimscroll();

var today = '@DateTime.Now.ToShortDateString()';

$('.table tr').each(function () {
    debugger;
    var _date = new Date($(this).children("td:eq(3)").val());
    var year = d.getFullYear();
    var month = d.getMonth();
    var day = d.getDate();

    var date = year + '/' + month + '/' + day;


    if (date == today) {

    }

});


$('html, body').animate({
    scrollTop: $('.today').offset().top
}, 800, function () {

});

window.onload = function () {
    if (typeof (Storage) !== "undefined") {
        $('body').removeClass("light");
    } else {
        if (localStorage.setItem('theme') == 'light') {
            $('body').addClass("light");
        } else {
            $('body').removeClass("light");
        }
    }
}
