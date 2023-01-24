import React from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import Login from "./Login";
import Register from "./Register";
import VideoList from "./VideoList";

export default function ApplicationViews({ isLoggedIn }) {
    return (
        <Routes>
            <Route path="/">
                <Route index element={isLoggedIn ? <VideoList /> : <Navigate to="/login" />} />
                <Route path="videos">
                    <Route index element = {isLoggedIn ? <VideoList /> : <Navigate to="/login" />} />
                    <Route path="add" element = {isLoggedIn ? <VideoList /> : <Navigate to="/login" />} />
                    <Route path=":id" element = {isLoggedIn ? <VideoList /> : <Navigate to="/login" />} />
                </Route>
                <Route path="login" element = {<Login />} />
                <Route path="register" element= {<Register />} />
                <Route path="*" element={<p>Whoops, nothing here...</p>} />
            </Route>
        </Routes>
    );
}
