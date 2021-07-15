import Logo from "../Logo";
import Button from "../Button";

const SignUpForm = () => {
  return (
    <div className="form-box p-4 shadow-default">
      <Logo />
      <h2>Sign up</h2>
      <form className="signup-form vflex">
        <div className="form-item">
          <label htmlFor="username" className="form-label">
            Username
          </label>
          <input
            type="text"
            name="username"
            className="form-input"
            placeholder="Enter username"
          />
        </div>
        <div className="form-item">
          <label htmlFor="username" className="form-label">
            Email
          </label>
          <input
            type="email"
            name="email"
            className="form-input"
            placeholder="Enter email"
          />
        </div>
        <div className="form-item">
          <label htmlFor="password" className="form-label">
            Password
          </label>
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

export default SignUpForm;
