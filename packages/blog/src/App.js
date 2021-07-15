import "@bloged/ui-components";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";

const isSubdomain = () => {
  console.log(window.location.host.split(".").length, 4);
  return window.location.host.split(".").length === 2;
};

const getSubdomain = () => {
  return window.location.host.split(".")[0];
};

function App() {
  const r = (
    <Router>
      <Switch>
        <Route path="/" exact component={() => <h1>Blog home</h1>} />
        <Route path="/test" exact component={() => <h1>Blog test path</h1>} />
      </Switch>
    </Router>
  );

  return (
    <div>
      {isSubdomain() ? "ciao" : r}
      <p>blog</p>
      <p>{window.location.host}</p>
      <p>{window.location.protocol}</p>
    </div>
  );
}

export default App;
