import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import Policy from "./pages/Policy";
import Navbar from "./components/Navbar";
import Footer from "./components/Footer";
import LoginPage from "./pages/LoginPage/LoginPage";

const App = () => {
  const socials = {};

  return (
    <div className="app">
      <BrowserRouter>
        <Navbar
          // Title="Crypto Business Men X"
          Logo="/build/images/logo-small.png"
          Pages={[]}
          Socials={socials}
        />
        <Routes>
          <Route path="/" element={<Home />}></Route>
          {/* <Route
            path="/terms-and-conditions"
            element={
              <Policy filePath="/assets/policies/terms-and-conditions.md" />
            }
          /> */}
          <Route path="/login" element={<LoginPage />}></Route>
        </Routes>
        <Footer
          Links={
            [
              // {
              //   To: "/terms-and-conditions",
              //   Text: "Terms and conditions",
              // },
            ]
          }
          Socials={socials}
        />
      </BrowserRouter>
    </div>
  );
};

export default App;
