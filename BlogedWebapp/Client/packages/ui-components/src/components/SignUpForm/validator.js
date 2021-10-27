export default function formValidation(values) {
  let errors = {};
  console.log(values);
  if (!values.username || !values.username.trim()) {
    errors.username = "username-required";
  }

  if (!values.email || !values.email.trim()) {
    errors.email = "email-required";
  } else {
    const emailRegex = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    const emailMatched = emailRegex.test(
      String(values.email.trim().toLowerCase())
    );
    if (!emailMatched) errors.email = "invalid-email";
  }

  if (!values.password || !values.password.trim()) {
    errors.password = "subject-required";
  }

  return errors;
}
