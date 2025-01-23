import Keycloak from 'keycloak-js';

const keycloak = new Keycloak({
 url: "http://localhost:8080",
 realm: "Library",
 clientId: "react-frontend"
});

// try {
//     const authenticated = await keycloak.init();
//     if (authenticated) {
//         console.log('User is authenticated');
//     } else {
//         console.log('User is not authenticated');
//     }
// } catch (error) {
//     console.error('Failed to initialize adapter:', error);
// }

export default keycloak;