function initOwlCarousel() {
    $('.owl-carousel').owlCarousel({
        loop: true,
        margin: 20,
        nav: true,
        dots: true,
        autoplay: true,
        autoplayTimeout: 5000,
        autoplayHoverPause: true,
        responsive: {
            0: {
                items: 1,
                margin: 10,
                nav: false,
                dots: true
            },
            480: {
                items: 2,
                margin: 20,
                nav: true,
                dots: true
            },
            768: {
                items: 3,
                margin: 20,
                nav: true,
                dots: true
            },
            1024: {
                items: 3,
                margin: 20,
                nav: true,
                dots: true
            }
        },
        navText: [
            '<i class="fas fa-chevron-left"></i>',
            '<i class="fas fa-chevron-right"></i>'
        ],
        smartSpeed: 450,
        animateOut: 'fadeOut',
        animateIn: 'fadeIn'
    });

    // Add aria-label to the previous button
    $('.owl-prev').attr('aria-label', 'Previous Slide');

    // Add aria-label to the next button
    $('.owl-next').attr('aria-label', 'Next Slide');
}
