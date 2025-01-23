import { useKeycloak } from "@react-keycloak/web";

const PrivateRoute = ({ children }) => {
  const { keycloak } = useKeycloak();

  const isLoggedIn = keycloak.authenticated;

  console.info(keycloak.token);

  return isLoggedIn ? children : null;
};

export default PrivateRoute;
