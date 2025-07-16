function getRequest(url, params = {}) {
    const queryString = new URLSearchParams(params).toString();
    const token = getUser()?.token;
    return fetch(`${url}?${queryString}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
    }).then(async response => {
        if (!response.ok) {
            throw new RequestError(await response.json());
        }
        return response.json();
    });
}

function postRequest(url, data = {}, params = {}) {
    const token = getUser()?.token;
    const queryString = new URLSearchParams(params).toString();
    return fetch(`${url}?${queryString}`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
            ...(data instanceof FormData ? {} : { 'Content-Type': 'application/json' })
        },
        body: data instanceof FormData ? data : JSON.stringify(data)
    }).then(async response => {
        if (!response.ok) {
            throw new RequestError(await response.json());
        }
        return response.json();
    });
}

function putRequest(url, data = {}, params = {}) {
    const token = getUser()?.token;
    const queryString = new URLSearchParams(params).toString();
    return fetch(`${url}?${queryString}`, {
        method: 'PUT',
        headers: {
            'Authorization': `Bearer ${token}`,
            ...(data instanceof FormData ? {} : { 'Content-Type': 'application/json' })
        },
        body: data instanceof FormData ? data : JSON.stringify(data)
    }).then(async response => {
        if (!response.ok) {
            throw new RequestError(await response.json());
        }
        return response.json();
    });
}

function deleteRequest(url, params = {}) {
    const token = getUser()?.token;
    const queryString = new URLSearchParams(params).toString();
    return fetch(`${url}?${queryString}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    }).then(async response => {
        if (!response.ok) {
            throw new RequestError(await response.json());
        }
        return;
    });
}
