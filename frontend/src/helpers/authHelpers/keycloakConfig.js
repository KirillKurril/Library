const keycloakConfig = {
    url: process.env.REACT_APP_KEYCLOAK_HOST,
    realm: process.env.REACT_APP_KEYCLOAK_REALM,
    clientId: process.env.REACT_APP_KEYCLOAK_CLIENT_ID,
    redirectUri: "http://localhost:7021",
};
  
export default keycloakConfig