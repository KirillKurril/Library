import { useKeycloak } from "@react-keycloak/web";
import { Navigate } from "react-router-dom";

const AdminRoute = ({ children }) => {
  const { keycloak } = useKeycloak();

  const isLoggedIn = keycloak.authenticated;
  const hasAdminRole = isLoggedIn && keycloak.hasResourceRole("admin", "library-frontend");

  return isLoggedIn && hasAdminRole ? children : <Navigate to="/" />; 
};

export default AdminRoute;

