import React from "react";
import { useBackend } from "../../../hooks/useBackend";

const LoginPage = (props: any) => {
  const { LogIn } = useBackend();

  return (
    <div className="page login-page">
      <form
        className="form login-form"
        onSubmit={async (e) => {
          e.preventDefault();
          let data = await LogIn();
        }}
      >
        <label htmlFor="email">Email</label>
        <input type="text" name="email" />
        <label htmlFor="password">Password</label>
        <input type="password" name="password" />
        <input type="submit" value="Login" className="primary" />
      </form>
    </div>
  );
};

export default LoginPage;
