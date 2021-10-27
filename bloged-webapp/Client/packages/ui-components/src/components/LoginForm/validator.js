export default function formValidation(values) {
  let errors = {};

  console.log("Validating...");

  if (!values.name.trim()) {
    errors.name = "name-required";
  }

  if (!values.email.trim()) {
    errors.email = "email-required";
  } else {
    const emailRegex = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    const emailMatched = emailRegex.test(
      String(values.email.trim().toLowerCase())
    );
    if (!emailMatched) errors.email = "invalid-email";
  }

  if (!values.subject.trim()) {
    errors.subject = "subject-required";
  }

  if (!values.message.trim()) {
    errors.message = "message-required";
  }

  return errors;
}
