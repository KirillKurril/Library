import React, { useEffect, useState } from 'react';
import Keycloak from 'keycloak-js';


const useAuth = () => {
    const [isLogin, setLogin] = useState(false);

    useEffect(() =>{
        const client = new Keycloak({
            url: 'http://localhost:8080',
            realm: process.env.REACT_APP_KEYCLOAK_REALM,
            clientId: process.env.REACT_APP_KEYCLOAK_CLIENT_ID
        });
        
        client.init({onLoad})
    }, [])

    return isLogin;
}

export default useAuth;