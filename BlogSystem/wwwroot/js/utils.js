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

function getRequest(url, params = {}) {
    const queryString = new URLSearchParams(params).toString();
    const token = getToken();
    return fetch(`${url}?${queryString}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
    }).then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    });
}

function postRequest(url, data = {}, params = {}) {
    const token = getToken();
    const queryString = new URLSearchParams(params).toString();
    return fetch(`${url}?${queryString}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(data)
    }).then(async response => {
        if (!response.ok) {
            throw new RequestError(await response.json());
        }
        return response.json();
    });
}

function putRequest(url, data = {}, params = {}) {
    const token = getToken();
    const queryString = new URLSearchParams(params).toString();
    return fetch(`${url}?${queryString}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(data)
    }).then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    });
}

function deleteRequest(url, params = {}) {
    const token = getToken();
    const queryString = new URLSearchParams(params).toString();
    return fetch(`${url}?${queryString}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    }).then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    });
}

function setToken(token) {
    localStorage.setItem('token', token);
}

function getToken() {
    return localStorage.getItem('token');
}
