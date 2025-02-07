import axios from 'axios';

export const api = axios.create({
    baseURL: process.env.REACT_APP_API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});
api.interceptors.request.use(
    async (config) => {

        const tokenData = JSON.parse(localStorage.getItem("tokenData"));
        const accessToken = tokenData?.access_token;

        console.log("accessToken", accessToken);

        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`;
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

export const keycloakUserApi = axios.create({
    baseURL: `${process.env.REACT_APP_KEYCLOAK_HOST}/admin/realms/${process.env.REACT_APP_KEYCLOAK_REALM}/users`,
    headers: {
        'Content-Type': 'application/json',
    },
});
keycloakUserApi.interceptors.request.use(
    async (config) => {

        const tokenData = JSON.parse(localStorage.getItem("tokenData"));
        const accessToken = tokenData?.access_token;

        console.log("accessToken", accessToken);

        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`;
        }

        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

keycloakUserApi.interceptors.response.use(
    (response) => response,
    async (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem('token');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);