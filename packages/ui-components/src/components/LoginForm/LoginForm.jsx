const LoginForm = () => {
  return (
    <div className="form-box p-4 shadow-default">
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
      </form>
    </div>
  );
};

export default LoginForm;
