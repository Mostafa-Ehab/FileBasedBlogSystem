class RequestError extends Error {
    constructor(message) {
        super(message);
        this.data = message;
        this.name = "RequestError";
    }
}