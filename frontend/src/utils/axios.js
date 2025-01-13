import axios from 'axios';

export const api = axios.create({
    baseURL: process.env.ASPNET_API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});


export const keycloakApi = axios.create({
    baseURL: `${process.env.KEYCLOAK_HOST}/realms/${KEYCLOAK_REALM}/`,
    headers: {
        'Content-Type': 'application/json',
    },
});

api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

api.interceptors.response.use(
    (response) => response,
    async (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem('token');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);