import { useState, useEffect } from "react";

const useSignUpForm = (validate) => {
  const [values, setValues] = useState({
    email: "",
    username: "",
    password: "",
  });

  const [errors, setErrors] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setValues({ ...values, [name]: value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setIsSubmitted(true);
    setErrors(validate(values));
  };

  useEffect(() => {
    if (isSubmitted && Object.keys(errors).length === 0) {
      setValues({
        name: "",
        email: "",
        password: "",
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [errors]);

  return { handleChange, values, handleSubmit, errors, isSubmitted };
};

export default useSignUpForm;
