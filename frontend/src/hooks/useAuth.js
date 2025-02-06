import { useEffect, useState, useCallback } from "react";
import { getAuthorizationUrl, exchangeCodeForToken } from "../helpers/authHelpers/keycloakUriConfigurator";

const parseJwt = (token) => {
  try {
    return JSON.parse(atob(token.split(".")[1]));
  } catch (e) {
    return null;
  }
};

const useAuth = () => {
  const [user, setUser] = useState(null);

  const initializeAuth = useCallback(async () => {
    const urlParams = new URLSearchParams(window.location.search);
    const code = urlParams.get("code");
    console.log(`code: ${code}`)
    
    if (code && !user) {
      const tokenData = await exchangeCodeForToken(code);
      if (tokenData?.access_token) {
        const decodedToken = parseJwt(tokenData.access_token);
        setUser(decodedToken);
      }
    }
  }, [user]);

  useEffect(() => {
    initializeAuth();
  }, [initializeAuth]);

  const login = () => {
    window.location.href = getAuthorizationUrl();
  };

  const logout = () => {
    setUser(null);
    window.location.href = "/";
  };

  return { user, login, logout, isAuthenticated: !!user };
};

export default useAuth;
