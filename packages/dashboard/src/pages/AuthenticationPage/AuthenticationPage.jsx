import { LoginForm, SignUpForm } from "@bloged/ui-components";

const AuthenticationPage = (props) => {
  return (
    <div>
      <p>Autenticazione:</p>
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
