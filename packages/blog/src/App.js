import "@bloged/ui-components";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";
function App() {
  return (
    <div>
      <Router>
        <Switch>
          <Route path="/" exact component={() => <h1>Blog home</h1>} />
          <Route path="/test" exact component={() => <h1>Blog test path</h1>} />
        </Switch>
      </Router>
      <p>blog</p>
      <p>{window.location.host}</p>
      <p>{window.location.protocol}</p>
    </div>
  );
}

export default App;
