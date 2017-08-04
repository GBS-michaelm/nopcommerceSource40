/*!
 * nopAccelerate Simplex Theme v1.1.0 (http://themes.nopaccelerate.com/themes/nopaccelerate-simplex-theme-3/)
 * Copyright 2016 Xcellence-IT.
 * Licensed under http://www.nopaccelerate.com/terms/
 */

/* Using Simplex js for nopAccelerate Simplex Theme */

$(document).ready(function () {
    //Used js for Header Sticky Menu  
    //http://www.jqueryscript.net/menu/Sticky-Navigation-Bar-with-jQuery-Bootstrap.html
    $(window).bind('scroll', function() {
        var navHeight = $("div.header").height();
        var navWidth = $("div.header").width();
if(window.location.pathname.includes("Plugins/CcWidget/EditorPage"))
{
    $(".header-lower").hide();
    $(".header-menu").css("display", "none !important");
    $(".newmneu").css("display", "none !important");   
}
else{
        ($(window).scrollTop() > navHeight) ? $('body').addClass('goToTop').width(navWidth) : $('body').removeClass('goToTop');}
    });

    //Used js for Responsive Website Checker
    $('#exit').click(function (e) {
        $('.responsive').hide();
        $('.master-wrapper-page').css('margin-top', '0');
    });

    //Used js for Left Sliderbar Collapse(Responsive Devices)
    $('.block .title').click(function() {
        var e = window, a = 'inner';
        if (!('innerWidth' in window)) {
            a = 'client';
            e = document.documentElement || document.body;
        }
        var result = { width: e[a + 'Width'], height: e[a + 'Height'] };
        if (result.width < 991) {
            $(this).siblings('.listbox').slideToggle('slow');
            $(this).toggleClass("arrow-up-down");
        }
    });

    //Used js for flayout cart
    $(".shopping-cart-link").mouseenter(function () {
        $('#flyout-cart-wrapper').addClass('active');
    });
    $(".header-lower").mouseenter(function () {
        $('#flyout-cart-wrapper').removeClass('active');
    });
    $(".flyout-cart-wrapper").mouseleave(function () {
        $('#flyout-cart-wrapper').removeClass('active');
    });


    //Used js for Product Box and Product Thumbs Slider
    $("#home-category-slider,#sub-category-slider,#manufacturer-slider").owlCarousel({
        items: 3,
        itemsCustom: false,
        itemsDesktop: [1199, 3],
        itemsDesktopSmall: [980, 3],
        itemsTablet: [768, 3],
        itemsMobile: [640, 1],
        itemsScaleUp: true,
        autoPlay: false,
        responsive: true,
        navigation: true,
    });
    $("#product-slider").owlCarousel({
        items: 3,
        itemsCustom: false,
        itemsDesktop: [1199, 3],
        itemsDesktopSmall: [980, 3],
        itemsTablet: [768, 3],
        itemsMobile: [640, 2],
        itemsScaleUp: true,
        autoPlay: true,
        responsive: true,
        navigation: true,
    });
    $("#crosssell-products-slider,#home-bestseller-slider,#home-features-slider,#related-products-slider,#also-purchased-products-slider").owlCarousel({
        items: 4,
        itemsCustom: false,
        itemsDesktop: [1199, 4],
        itemsDesktopSmall: [980, 3],
        itemsTablet: [768, 3],
        itemsMobile: [640, 1],
        itemsScaleUp: true,
        autoPlay: false,
        responsive: true,
        navigation: true,
    });
    $("#home-news-slider").owlCarousel({
        items: 2,
        itemsCustom: false,
        itemsDesktop: [1199, 2],
        itemsDesktopSmall: [980, 2],
        itemsTablet: [768, 2],
        itemsMobile: [640, 1],
        itemsScaleUp: true,
        autoPlay: false,
        responsive: true,
        navigation: true,
    });


    /* Used js for BackTop Page Scrolling*/
    (function ($) {
        $.fn.backTop = function (options) {
            var backBtn = this;
            var settings = $.extend({
                'position': 400,
                'speed': 500,
            }, options);

            //Settings
            var position = settings['position'];
            var speed = settings['speed'];
            
            backBtn.css({
                'right': 40,
                'bottom': 40,
                'position': 'fixed',
            });

            $(document).scroll(function () {
                var pos = $(window).scrollTop();
                console.log(pos);

                if (pos >= position) {
                    backBtn.fadeIn(speed);
                } else {
                    backBtn.fadeOut(speed);
                }
            });

            backBtn.click(function () {
                $("html, body").animate({
                    scrollTop: 0
                },
                {
                    duration: 1200
                });
            });
        }
    }(jQuery));

    $('#backTop').backTop({
        'position': 200,
        'speed': 500,
    });

});

//Used js for Product Increase and Decrease Quantity Item
function ChangeQuantity1(e) {

    var id = document.getElementById(e.id).value;
    // var pricechange = document.getElementById("pricescript").textContent;
    var pricechange = $(".pricescript-value-" + e.dataset.productid).text();

 if (id <= 0)
        return 0;
    var qtyValue = parseInt(id);
   // qtyValue = qtyValue + 1;
    var final = parseFloat(qtyValue * pricechange).toFixed(2);
   
    document.getElementById(e.id).value = qtyValue;
    //document.getElementById("eachprice").textContent = "(Ea. $" + document.getElementById("pricescript").textContent + ")";
    $(".eachprice-value-" + e.dataset.productid).text("(Ea. $" + pricechange + ")");
    //document.getElementById("newprice").textContent = "$"+final;
    $(".price-value-" + e.dataset.productid).text("$" + final);
    return true;
}

function IncreaseQuantity(e) {
    var id = document.getElementById(e.id).value;
if (id <= 0)
        id=0;
    // var pricechange = document.getElementById("pricescript").textContent;
    var pricechange = $(".pricescript-value-" + e.dataset.productid).text();

    var qtyValue = parseInt(id);
    qtyValue = qtyValue + 1;
    var final = parseFloat(qtyValue * pricechange).toFixed(2);
    document.getElementById(e.id).value = qtyValue;
    //  document.getElementById("eachprice").textContent = "(Ea. $"+document.getElementById("pricescript").textContent+")";
    $(".eachprice-value-" + e.dataset.productid).text("(Ea. $" + pricechange + ")");

    //document.getElementById("newprice").textContent = "$"+final;
    $(".price-value-" + e.dataset.productid).text("$"+final);
    return true;
}

function DecreaseQuantity(e) {
    var id = document.getElementById(e.id).value;
    // var pricechange = document.getElementById("pricescript").textContent;
    var pricechange = $(".pricescript-value-" + e.dataset.productid).text();
   
    if (id <= 0 || id==1){
        id=1;}

    var qtyValue = parseInt(id);
if (id <= 0 || id==1){
    qtyValue = 1;
}
else{
qtyValue = qtyValue - 1;
}

var final = parseFloat(qtyValue * pricechange).toFixed(2);
    document.getElementById(e.id).value = qtyValue;
    //document.getElementById("newprice").textContent = "$" + final;
    //document.getElementById("eachprice").textContent = "(Ea. $" + document.getElementById("pricescript").textContent + ")";
    $(".eachprice-value-" + e.dataset.productid).text("(Ea. $" + pricechange + ")");

    $(".price-value-" + e.dataset.productid).text("$" + final);
    
    return true;
}

//<script type="text/javascript">

$(document).on("nopAjaxFiltersFiltrationCompleteEvent", function (e) {

    //Used js for Product Box and Product Thumbs Slider
    $("#home-category-slider,#sub-category-slider,#manufacturer-slider").owlCarousel({
        items: 3,
        itemsCustom: false,
        itemsDesktop: [1199, 3],
        itemsDesktopSmall: [980, 3],
        itemsTablet: [768, 3],
        itemsMobile: [640, 1],
        itemsScaleUp: true,
        autoPlay: false,
        responsive: true,
        navigation: true,
    });
    $("#product-slider").owlCarousel({
        items: 3,
        itemsCustom: false,
        itemsDesktop: [1199, 3],
        itemsDesktopSmall: [980, 3],
        itemsTablet: [768, 3],
        itemsMobile: [640, 2],
        itemsScaleUp: true,
        autoPlay: true,
        responsive: true,
        navigation: true,
    });
    $("#crosssell-products-slider,#home-bestseller-slider,#home-features-slider,#related-products-slider,#also-purchased-products-slider").owlCarousel({
        items: 4,
        itemsCustom: false,
        itemsDesktop: [1199, 4],
        itemsDesktopSmall: [980, 3],
        itemsTablet: [768, 3],
        itemsMobile: [640, 1],
        itemsScaleUp: true,
        autoPlay: false,
        responsive: true,
        navigation: true,
    });
    $("#home-news-slider").owlCarousel({
        items: 2,
        itemsCustom: false,
        itemsDesktop: [1199, 2],
        itemsDesktopSmall: [980, 2],
        itemsTablet: [768, 2],
        itemsMobile: [640, 1],
        itemsScaleUp: true,
        autoPlay: false,
        responsive: true,
        navigation: true,
    });
});
//</script>
