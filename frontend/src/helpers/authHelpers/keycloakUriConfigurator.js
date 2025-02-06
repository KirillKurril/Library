import keycloakConfig from "./keycloakConfig"

export const getAuthorizationUrl = () => {
  const { url, realm, clientId, redirectUri } = keycloakConfig;
  const authEndpoint = `${url}/realms/${realm}/protocol/openid-connect/auth`;
  
  const params = new URLSearchParams({
    client_id: clientId,
    response_type: "code",
    scope: "openid profile email",
    redirect_uri: redirectUri,
  });

  const authUrl = `${authEndpoint}?${params.toString()}`;
  return authUrl;   
};

export const exchangeCodeForToken = async (code) => {
    const { url, realm, clientId, redirectUri } = keycloakConfig;
    const tokenEndpoint = `${url}/realms/${realm}/protocol/openid-connect/token`;
  
    const params = new URLSearchParams({
      grant_type: "authorization_code",
      client_id: clientId,
      code: code,
      redirect_uri: redirectUri,
    });

    
  
    try {
      const response = await fetch(tokenEndpoint, {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
        body: params,
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error("Ошибка получения токена");
      }
  
      return data; 
    } catch (error) {
      console.error("Ошибка:", error);
      return null;
    }
  };
  
  export const refreshTokenRequest = async (refreshToken) => {
    const { url, realm, clientId } = keycloakConfig;
    const tokenEndpoint = `${url}/realms/${realm}/protocol/openid-connect/token`;
  
    const params = new URLSearchParams({
      grant_type: "refresh_token",
      client_id: clientId,
      refresh_token: refreshToken,
    });
  
    try {
      const response = await fetch(tokenEndpoint, {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
        body: params,
      });
  
      if (!response.ok) {
        throw new Error("Ошибка обновления токена");
      }
  
      return await response.json();
    } catch (error) {
      console.error("Ошибка обновления токена:", error);
      return null;
    }
  };

  export const getLogoutUrl = (idTokenHint) => {
    const { url, realm, redirectUri } = keycloakConfig;
    const logoutEndpoint = `${url}/realms/${realm}/protocol/openid-connect/logout`;
  
    const params = new URLSearchParams({
      post_logout_redirect_uri: redirectUri, 
      id_token_hint: idTokenHint,            
    });
  
    return `${logoutEndpoint}?${params.toString()}`;
  };