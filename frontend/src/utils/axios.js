import axios from 'axios';
import useAuth from '../hooks/useAuth';

export const api = axios.create({
    baseURL: process.env.ASPNET_API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});
api.interceptors.request.use(
    async (config) => {
        const {isAuthenticated, accessToken} = useAuth();
        if(isAuthenticated && accessToken)
        {
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