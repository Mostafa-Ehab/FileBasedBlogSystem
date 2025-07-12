class RequestError extends Error {
    constructor(message) {
        super(message);
        this.data = message;
        this.name = "RequestError";
    }
}

function showError(message) {
    hideMessages();
    const errorDiv = document.getElementById('error-message');
    const errorText = document.getElementById('error-text');

    errorText.textContent = message;
    errorDiv.style.display = 'flex';

    // Auto-hide after 5 seconds
    setTimeout(() => {
        errorDiv.style.display = 'none';
    }, 5000);
}

function showSuccess(message) {
    hideMessages();
    const successDiv = document.getElementById('success-message');
    const successText = document.getElementById('success-text');

    successText.textContent = message;
    successDiv.style.display = 'flex';

    // Auto-hide after 3 seconds
    setTimeout(() => {
        successDiv.style.display = 'none';
    }, 3000);
}

function hideMessages() {
    document.getElementById('error-message').style.display = 'none';
    document.getElementById('success-message').style.display = 'none';
}
