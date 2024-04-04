import React from 'react';
import AuthService from "../services/auth.service";

const Home = () => {
    const user = AuthService.getCurrentUser();
    
  
    return (
      <header>
        <nav>
          <ul>
            {user ? (
              <>
                <p>Hi, {user.user.name}</p>
              </>
            ) : (
              <>
                <p>You're not logged in</p>
              </>
            )}
          </ul>
        </nav>
      </header>
    );
  };
  
  export default Home;
  