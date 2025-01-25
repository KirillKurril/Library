import axios from 'axios';
import { useKeycloak } from "@react-keycloak/web";

export const api = axios.create({
    baseURL: process.env.ASPNET_API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});
api.interceptors.request.use(
    async (config) => {
        const { keycloak } = useKeycloak();
        if (keycloak && keycloak.token) {
            config.headers.Authorization = `Bearer ${keycloak.token}`;
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