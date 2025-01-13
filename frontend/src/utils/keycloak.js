import Keycloak from 'keycloak-js';

const keycloakConfig = {
    url: `${process.env.KEYCLOAK_HOST}/auth`,
    realm: process.env.KEYCLOAK_REALM,
    clientId: process.env.KEYCLOAK_CLIENT_ID
};

const keycloak = new Keycloak(keycloakConfig);

export default keycloak;
