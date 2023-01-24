import React, { useEffect, useState } from "react"
import { BrowserRouter } from "react-router-dom"
import { Spinner } from "reactstrap"
import "./App.css"
import ApplicationViews from "./components/ApplicationViews"
import Navbar from "./components/Navbar"
import { onLoginStatusChange } from "./modules/authManager"

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(null)

  useEffect(() => {
    onLoginStatusChange(setIsLoggedIn)
  }, [])

  if (isLoggedIn === null) {
    return <Spinner className="app-spinner dark" />
  }

  return (
    <div className="App">
      <BrowserRouter>
        <Navbar isLoggedIn={isLoggedIn} />
        <ApplicationViews isLoggedIn={isLoggedIn} />
      </BrowserRouter>
    </div>
  )
}

export default App