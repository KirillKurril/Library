import useAuth from '../hooks/useAuth';

const PrivateRoute = ({ children }) => {
  const { isAuthenticated } = useAuth();  
  return isAuthenticated ? children : <Navigate to="/" />;
};

export default PrivateRoute;
