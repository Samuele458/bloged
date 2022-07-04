import "../styles/main.scss";

import ReactDOM from "react-dom";
import App from "./App";
import BackendProvider from "../providers/backendProvider";

document.addEventListener("DOMContentLoaded", async () => {
  ReactDOM.render(
    <BackendProvider>
      <App />
    </BackendProvider>,
    document.getElementById("minting-dapp")
  );
});
