import "@bloged/ui-components";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";

import AuthenticationPage from "./pages/AuthenticationPage";
function App() {
  return (
    <div>
      <Router>
        <Switch>
          <Route
            path="/login"
            component={() => {
              return <AuthenticationPage type="login" />;
            }}
          />

          <Route
            path="/signup"
            component={() => {
              return <AuthenticationPage type="signup" />;
            }}
          />
        </Switch>
      </Router>
      <p>dashboard</p>
      <p>{window.location.host}</p>
      <p>{window.location.protocol}</p>
    </div>
  );
}

export default App;
