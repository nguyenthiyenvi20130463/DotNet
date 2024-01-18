(function($) {
    "use strict";
    if ($('.scroll-top').length) {
        var scrollTrigger = 100, // px
            backToTop = function() {
                var scrollTop = $(window).scrollTop();
                if (scrollTop > scrollTrigger) {
                    $('.scroll-top').addClass('show');

                } else {
                    $('.scroll-top').removeClass('show');
                }
            };
        backToTop();
        $(window).on('scroll', function() {
            backToTop();
            
        });
        $('.scroll-top').on('click', function(e) {
            e.preventDefault();
            $('html,body').animate({
                scrollTop: 0
            }, 700);
        });
    }
    $(window).load(function() {
        $('.slider').nivoSlider({
            effect: 'random',
            animSpeed: 500,
            pauseTime: 8000
        });
        $('.slider').on('swipeleft', function(e) {
            $('a.nivo-nextNav').trigger('click');
            e.stopPropagation();
            return false;
        });
        $('.slider').on('swiperight', function(e) {
            $('a.nivo-prevNav').trigger('click');
            e.stopPropagation();
            return false;
        });
    });

    $('.slider__product').slick({
        dots: false,
        arrows:false,
        infinite: false,
        speed: 300,
        slidesToShow: 4,
        slidesToScroll: 4,
        lazyLoad: 'ondemand',
        responsive: [{
                breakpoint: 991,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: true
                }
            }, {
                breakpoint: 768,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            }, {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
            // You can unslick at a given breakpoint now by adding:
            // settings: "unslick"
            // instead of a settings object
        ]
    });
    $('.slider-partner').slick({
        dots: false,
        arrows:false,
        infinite: true,
        speed: 300,
        autoplay: true,
        autoplaySpeed: 2000,
        slidesToShow: 8,
        slidesToScroll: 8,
        lazyLoad: 'ondemand',
        responsive: [{
                breakpoint: 991,
                settings: {
                    slidesToShow: 6,
                    slidesToScroll: 6,
                    infinite: true,
                    dots: true
                }
            }, {
                breakpoint: 768,
                settings: {
                    slidesToShow: 4,
                    slidesToScroll: 4
                }
            }, {
                breakpoint: 480,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3
                }
            }, {
                breakpoint: 320,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            }
            // You can unslick at a given breakpoint now by adding:
            // settings: "unslick"
            // instead of a settings object
        ]
    });

    $('.nav-slider__product .nav.prev').click(function(){
      $('.slider__product').slick('slickPrev');
    });

    $('.nav-slider__product .nav.next').click(function(){
      $('.slider__product').slick('slickNext');
    });
     $(".nav-toggle").click(function(){
        if($(".menu-category").hasClass("show-menu")){
            $(".menu-category").removeClass("show-menu");
            $(".common-overlay").removeClass("show");
        }else{
            $(".menu-category").addClass("show-menu");
            $(".common-overlay").addClass("show");
    

        }
    });
     $(".common-overlay").click(function(){
        if($(".common-overlay").hasClass("show")){
            $(".menu-category").removeClass("show-menu");
            $(".common-overlay").removeClass("show");
        }else{
            $(".menu-category").addClass("show-menu");
            $(".common-overlay").addClass("show");
    

        }
    });
    $(".menu-category .catalog__item").after().click(function(){
        $(this).toggleClass("active");
    });
    $('.product__view__image--thumb').slick({
        slidesToShow: 1,
        slidesToScroll: 2,
        verticalSwiping: true,
        speed: 300,
        autoplay: true,
        autoplaySpeed: 2000,
        vertical: true,
        arrows: true
    });
    $('.modal').on('shown.bs.modal', function (e) {
        $('.product__view__image--list,.product__view__image--thumb').resize();
    });
    if($("#slider__price,#lower-price,#upper-price").length){
        var slider__price = document.getElementById('slider__price');

        noUiSlider.create(slider__price, {
            connect: true,
            behaviour: 'tap',
            start: [ 500, 4000 ],
            range: {
                'min': [ 0 ],
                '10%': [ 500, 500 ],
                '50%': [ 4000, 1000 ],
                'max': [ 10000 ]
            }
        });
        var nodes = [
            document.getElementById('lower-price'), // 0
            document.getElementById('upper-price')  // 1
        ];
        slider__price.noUiSlider.on('update', function ( values, handle, unencoded, isTap, positions ) {
            nodes[handle].innerHTML = values[handle] ;
        });
    }
    if($("#elevate__zoom,#image-additional-carousel").length){
        if( $("body").width() > 700 ){ 
            var zoomCollection = '#elevate__zoom';
            $( zoomCollection ).elevateZoom({
                lensShape : "round",
                lensSize    : 50,
                easing:true,
                scrollZoom : true,
                gallery:'image-additional-carousel',
                cursor: 'pointer',
                galleryActiveClass: "active"
            });
        }   
        
    }
    if($(".fancybox").length){
        $('.fancybox').fancybox();
    }
    $(".modal__close").click(function(){
        $(".zoomContainer").remove();
    });
    $(document).ready(function(){
        $('[data-toggle="tooltip"],[data-toggle="modal"]').tooltip();   
    });
    // if($(".fancybox").length){
    //     $('.fancybox').fancybox();
    // }
})(jQuery);