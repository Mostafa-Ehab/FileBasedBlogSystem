function formatReadableDate(dateStr) {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
    });
}

function estimateReadingTime(html, wordsPerMinute = 200) {
    // Create a temporary DOM element to parse HTML
    const tempDiv = document.createElement('div');
    tempDiv.innerHTML = html;

    // Get the plain text content
    const text = tempDiv.textContent || tempDiv.innerText || '';

    // Count words
    const words = text.trim().split(/\s+/).length;
    const minutes = Math.ceil(words / wordsPerMinute);

    return minutes;
}

function getIconClass(platform) {
    switch (platform) {
        case 'twitter':
            return 'fab fa-twitter';
        case 'linkedin':
            return 'fab fa-linkedin';
        case 'github':
            return 'fab fa-github';
        case 'facebook':
            return 'fab fa-facebook';
        case 'instagram':
            return 'fab fa-instagram';
        default:
            return 'fa-solid fa-globe';
    }
}

function generateSlug(title) {
    return title.toLowerCase()
        .replace(/[^a-z0-9 -]/g, '')
        .replace(/\s+/g, '-')
        .replace(/-+/g, '-')
        .replace(/\./g, '-')
        .trim();
}

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}
