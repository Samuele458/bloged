import "@bloged/ui-components";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";
function App() {
  return (
    <div>
      <Router>
        <Switch>
          <Route path="/" exact component={() => <h1>Dashboard home</h1>} />
          <Route path="/test" exact component={() => <h1>Dashboard test path</h1>} />
        </Switch>
      </Router>
      <p>dashboard</p>
      <p>{window.location.host}</p>
      <p>{window.location.protocol}</p>
    </div>
  );
}

export default App;
