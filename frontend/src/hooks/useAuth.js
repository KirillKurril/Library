import { useKeycloak } from "@react-keycloak/web";
import { useState, useEffect } from "react";

export const useAuth = () => {
    const { keycloak } = useKeycloak();

    const [authState, setAuthState] = useState({
        isAuthenticated: false,
        username: null,
        isAdmin: false,
    });

    useEffect(() => {
        const updateState = () => {
            setAuthState({
                isAuthenticated: keycloak.authenticated,
                username: keycloak.tokenParsed?.preferred_username || null,
                isAdmin: keycloak.hasResourceRole("admin"),
            });
        };

        updateState();

        keycloak.onAuthSuccess = updateState;
        keycloak.onAuthLogout = updateState;

        return () => {
            keycloak.onAuthSuccess = null;
            keycloak.onAuthLogout = null;
        };
    }, [keycloak]);

    const login = () => {
        if (keycloak && !keycloak.authenticated) {
            keycloak.login();
        }
    };

    const logout = () => {
        if (keycloak && keycloak.authenticated) {
            keycloak.logout();
        }
    };

    return {
        authState,
        login,
        logout,
    };
};

export default useAuth;