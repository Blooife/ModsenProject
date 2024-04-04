import React from 'react';
import AuthService from "../services/auth.service";
import { useNavigate, Link } from "react-router-dom";
import '../css/Header.css'; 

const Header = () => {
  const user = AuthService.getCurrentUser();
  const navigate = useNavigate();

  const logOut = () => {
    AuthService.logout();
    navigate("/home");
    window.location.reload();
  };

  return (
    <header>
      <nav>
        <ul className="navbar">
          {user ? (
            <>
              <li>
                <Link to="/">Home</Link>
              </li>
              <li>
                <Link to="/events">Events</Link>
              </li>
              <li>
                <Link to="/myEvents">My events</Link>
              </li>
              {user.role && user.role.includes("ADMIN") && ( 
                <li>
                  <Link to="/adminPage">Admin Panel</Link>
                </li>
              )}
              <li>
                <button className="logout-button" onClick={logOut}>Logout</button>
              </li>
            </>
          ) : (
            <>
              <li>
                <Link to="/login">Login</Link>
              </li>
              <li>
                <Link to="/register">Register</Link>
              </li>
            </>
          )}
        </ul>
      </nav>
    </header>
  );
};

export default Header;
