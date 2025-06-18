document.addEventListener('DOMContentLoaded', () => {
    const sliderTrack = document.querySelector('.slider-track');
    const sliderCards = document.querySelectorAll('.post-card');
    const prevBtn = document.querySelector('.slider-prev');
    const nextBtn = document.querySelector('.slider-next');

    let currentIndex = 0;
    const cardWidth = sliderCards[0].offsetWidth + parseInt(window.getComputedStyle(sliderTrack).gap);

    // Handle window resize
    function updateCardWidth() {
        cardWidth = sliderCards[0].offsetWidth + parseInt(window.getComputedStyle(sliderTrack).gap);
    }

    window.addEventListener('resize', updateCardWidth);

    // Move to next slide
    function moveToNextSlide() {
        if (currentIndex >= sliderCards.length - 2) {
            currentIndex = 0;
        } else {
            currentIndex++;
        }
        updateSliderPosition();
    }

    // Move to previous slide
    function moveToPrevSlide() {
        if (currentIndex <= 0) {
            currentIndex = sliderCards.length - 2;
        } else {
            currentIndex--;
        }
        updateSliderPosition();
    }

    // Update slider position
    function updateSliderPosition() {
        sliderTrack.style.transform = `translateX(-${currentIndex * cardWidth}px)`;
    }

    // Add event listeners
    prevBtn.addEventListener('click', moveToPrevSlide);
    nextBtn.addEventListener('click', moveToNextSlide);

    // Auto-scroll every 5 seconds
    setInterval(moveToNextSlide, 5000);
});
