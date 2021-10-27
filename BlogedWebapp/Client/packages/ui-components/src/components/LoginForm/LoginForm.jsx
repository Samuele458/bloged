import Logo from "../Logo";
import Button from "../Button";

const LoginForm = () => {
  return (
    <div className="form-box p-4 shadow-default">
      <Logo />
      <h2>Login</h2>
      <form className="login-form vflex">
        <div className="form-item">
          <label htmlFor="usernameOrEmail">Username or email</label>
          <input
            type="text"
            name="usernameOrEmail"
            className="form-input"
            placeholder="Enter username or email"
          />
        </div>
        <div className="form-item">
          <label htmlFor="password">Password</label>
          <input
            type="password"
            name="password"
            className="form-input"
            placeholder="Enter password"
          />
        </div>
        <div className="toolbar pt-6 pb-3">
          <Button
            type="submit"
            text="Submit"
            color="white"
            backgroundColor="#0454ed"
          />
        </div>
      </form>
    </div>
  );
};

export default LoginForm;
