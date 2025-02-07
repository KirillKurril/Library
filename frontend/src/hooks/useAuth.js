import { useEffect, useState, useCallback, useMemo } from "react";
import {
  getAuthorizationUrl,
  exchangeCodeForToken,
  refreshTokenRequest,
  getLogoutUrl,
} from "../helpers/authHelpers/keycloakUriConfigurator";

const parseJwt = (token) => {
  try {
    return JSON.parse(atob(token.split(".")[1]));
  } catch (e) {
    console.error("Ошибка при разборе JWT:", e);
    return null;
  }
};

const isTokenExpired = (token) => {
  const decoded = parseJwt(token);
  return decoded ? decoded.exp * 1000 < Date.now() : true;
};

const saveTokenData = (tokenData) => {
  if (typeof window !== "undefined") {
    localStorage.setItem("tokenData", JSON.stringify(tokenData));
  }
};

const loadTokenData = () => {
  if (typeof window !== "undefined") {
    const data = localStorage.getItem("tokenData");
    return data ? JSON.parse(data) : null;
  }
  return null;
};

const clearTokenData = () => {
  if (typeof window !== "undefined") {
    localStorage.removeItem("tokenData");
  }
};

const useAuth = () => {
  const [tokenData, setTokenData] = useState(() => loadTokenData());
  const [user, setUser] = useState(() => {
    const savedToken = loadTokenData()?.access_token;
    return savedToken ? parseJwt(savedToken) : null;
  });

  const handleTokenData = useCallback((data) => {
    saveTokenData(data);
    setTokenData(data);
    setUser(parseJwt(data.access_token));
  }, []);

  const refreshToken = useCallback(async () => {
    if (!tokenData?.refresh_token) return;

    try {
      const newTokenData = await refreshTokenRequest(tokenData.refresh_token);
      if (newTokenData?.access_token) {
        handleTokenData(newTokenData);
      } else {
        logout();
      }
    } catch (error) {
      console.error("Ошибка обновления токена:", error);
      logout();
    }
  }, [tokenData, handleTokenData]);

  const initializeAuth = useCallback(async () => {
    const urlParams = new URLSearchParams(window.location.search);
    const code = urlParams.get("code");

    if (code) {
      try {
        const tokenData = await exchangeCodeForToken(code);
        if (tokenData?.access_token) {
          handleTokenData(tokenData);
          window.history.replaceState({}, document.title, window.location.pathname);
        }
      } catch (error) {
        console.error("Ошибка получения токена:", error);
      }
    } else if (tokenData?.access_token) {
      if (isTokenExpired(tokenData.access_token)) {
        await refreshToken();
      } else {
        setUser(parseJwt(tokenData.access_token));
      }
    }
  }, [tokenData, refreshToken, handleTokenData]);

  useEffect(() => {
    initializeAuth();
    const interval = setInterval(() => {
      if (tokenData?.access_token && isTokenExpired(tokenData.access_token)) {
        refreshToken();
      }
    }, 60000);

    return () => clearInterval(interval);
  }, [initializeAuth, tokenData, refreshToken]);

  const logout = useCallback(() => {
    const idTokenHint = tokenData?.id_token;
    clearTokenData();
    setUser(null);
    setTokenData(null);

    window.location.href = idTokenHint ? getLogoutUrl(idTokenHint) : "/";
  }, [tokenData]);

  const login = useCallback(() => {
    window.location.href = getAuthorizationUrl();
  }, []);

  const userId = useMemo(() => user?.sub, [user]);
  const userEmail = useMemo(() => user?.email || "not specified", [user]);
  const userName = useMemo(() => user?.preferred_username || "not specified", [user]);
  const userRoles = useMemo(() => user?.resource_access?.["aspnet-api"]?.roles || [], [user]);
  const isAdmin = useMemo(() => userRoles.includes("admin"), [userRoles]);
  const accessToken = useMemo(() => tokenData?.access_token, [tokenData]);
  const isAuthenticated = useMemo(() => !!user, [user]);

  return {
    user,
    login,
    logout,
    isAuthenticated,
    userId,
    userName,
    userEmail,
    userRoles,
    isAdmin,
    accessToken,
  };
};

export default useAuth;
