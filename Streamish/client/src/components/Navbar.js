import React from "react";
import { Link } from "react-router-dom";

const Navbar = ({ isLoggedIn }) => {
    return (
        <nav className="navbar navbar-expand navbar-dark bg-info">
            {
                isLoggedIn
                    ? <>
                        <Link to="/" className="navbar-brand">
                            StreamISH
                        </Link>
                        <ul className="navbar-nav mr-auto">
                            <li className="nav-item">
                                <Link to="/" className="nav-link">Feed</Link>
                            </li>
                            <li className="nav-item">
                                <Link to="/videos/add" className="nav-link">New Video</Link>
                            </li>
                            {'ADD LOGOUT HERE'} 
                        </ul>
                    </>
                    : <>
                        <ul className="navbar-nav mr-auto">
                            <li className="nav-item">
                                <Link to="/login" className="nav-link">Login</Link>
                            </li>
                            <li className="nav-item">
                                <Link to="/register" className="nav-link">Register</Link>
                            </li>
                        </ul>
                    </>
            }
        </nav>
    )
};

export default Navbar;