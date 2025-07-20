// User management functions
function getUser() {
    try {
        const userData = localStorage.getItem('user') || sessionStorage.getItem('user');
        return userData ? JSON.parse(userData) : {};
    } catch (error) {
        console.error('Error parsing user data:', error);
        return {};
    }
}

function setUser(user) {
    const userData = JSON.stringify(user);
    // Store in localStorage for persistence, fallback to sessionStorage
    if (user.remember !== false) {
        localStorage.setItem('user', userData);
    } else {
        sessionStorage.setItem('user', userData);
    }
}

function clearUser() {
    localStorage.removeItem('user');
    sessionStorage.removeItem('user');
    // Clear legacy tokens if they exist
    localStorage.removeItem('adminToken');
    localStorage.removeItem('userRole');
    localStorage.removeItem('username');
    sessionStorage.removeItem('adminToken');
    sessionStorage.removeItem('userRole');
    sessionStorage.removeItem('username');
}
