import { LoginForm, SignUpForm } from "@bloged/ui-components";

const AuthenticationPage = (props) => {
  return (
    <div className="authentication-page mt-9">
      {props.type === "login" ? (
        <LoginForm />
      ) : props.type === "signup" ? (
        <SignUpForm />
      ) : (
        <></>
      )}
    </div>
  );
};

export default AuthenticationPage;
