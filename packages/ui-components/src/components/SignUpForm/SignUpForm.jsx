import Logo from "../Logo";
import Button from "../Button";

import React from "react";
import { useForm } from "react-hook-form";

const SignUpForm = () => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const onSubmit = async (data) => {
    console.log(errors, data);
  };

  console.log(errors);

  return (
    <div className="form-box p-4 shadow-default">
      <Logo />
      <h2>Sign up</h2>
      <form className="signup-form vflex" onSubmit={handleSubmit(onSubmit)}>
        <div className="form-item">
          <label htmlFor="username" className="form-label">
            Username
          </label>
          {errors.username && (
            <p className="form-error">{errors.username.message}</p>
          )}
          <input
            type="text"
            className="form-input"
            placeholder="Enter username"
            {...register("username", { required: true, minLength: 8 })}
          />
        </div>
        <div className="form-item">
          <label htmlFor="username" className="form-label">
            Email
          </label>
          <input
            type="text"
            className="form-input"
            placeholder="Enter email"
            {...register("email", { required: true })}
          />
        </div>
        <div className="form-item">
          <label htmlFor="password" className="form-label">
            Password
          </label>
          <input
            type="password"
            className="form-input"
            placeholder="Enter password"
            {...register("password", { required: true })}
          />
        </div>
        <div className="form-item">
          <label htmlFor="repeatPassword" className="form-label">
            Repeat password
          </label>
          <input
            type="password"
            className="form-input"
            placeholder="Enter password again"
            {...register("repeatPassword", { required: true })}
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
