// import { useKeycloak } from "@react-keycloak/web";

// export const useAuth = () => {
//     const { keycloak, initialized } = useKeycloak();

//     const login = () => {
//         if (keycloak && !keycloak.authenticated) {
//             keycloak.login();
//         }
//     };

//     const logout = () => {
//         if (keycloak && keycloak.authenticated) {
//             keycloak.logout();
//         }
//     };

//     return {
//         initialized,
//         isAuthenticated: keycloak.authenticated,
//         userId: keycloak.subject,
//         username: keycloak.tokenParsed?.preferred_username,
//         fullName: keycloak.tokenParsed?.name,
//         email: keycloak.tokenParsed?.email,
//         isAdmin: keycloak.hasResourceRole("admin"),
//         token: keycloak.token,
//         login,
//         logout,
//         keycloak 
//     };
// };

// export default useAuth;