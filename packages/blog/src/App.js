import "@bloged/ui-components";
import { Button } from "@bloged/ui-components";

function App() {
  return (
    <div>
      <h1>Blog</h1>
      <p>{window.location.host}</p>
      <p>{window.location.protocol}</p>
    </div>
  );
}

export default App;
