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

  return `${authEndpoint}?${params.toString()}`;
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
  
      if (!response.ok) {
        throw new Error("Ошибка получения токена");
      }
  
      const data = await response.json();
      return data; // access_token, refresh_token, id_token
    } catch (error) {
      console.error("Ошибка:", error);
      return null;
    }
  };
  