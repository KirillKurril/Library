import Keycloak from 'keycloak-js';

const keycloak = new Keycloak({
 url: "http://localhost:8080",
 realm: "Library",
 clientId: "react-frontend"
});

export default keycloak;