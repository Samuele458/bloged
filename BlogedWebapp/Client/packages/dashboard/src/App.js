import "@bloged/ui-components";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";

import AuthenticationPage from "./pages/AuthenticationPage";
function App() {
  return (
    <div>
      <Router>
        <Switch>
          <Route
            path="/dashboard/login"
            component={() => {
              return <AuthenticationPage type="login" />;
            }}
          />

          <Route
            path="/dashboard/signup"
            component={() => {
              return <AuthenticationPage type="signup" />;
            }}
          />
        </Switch>
      </Router>
    </div>
  );
}

export default App;
