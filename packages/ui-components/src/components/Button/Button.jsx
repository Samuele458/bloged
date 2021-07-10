import React from "react";
import PropTypes from "prop-types";

import "../../style/style.scss";

const Button = ({ text, type, url, ...props }) => {
  let ButtonTag;

  if (type === "button") {
    ButtonTag = "button";
  } else if (type === "link") {
    ButtonTag = "a";
  } else if (type === "submit") {
    ButtonTag = "button";
  }

  return <ButtonTag>{text}</ButtonTag>;
};

Button.propTypes = {
  text: PropTypes.string,
  url: PropTypes.string,
  type: PropTypes.oneOf(["button", "link", "submit"]),
};

Button.defaultProps = {
  text: "Button",
  url: null,
  type: "button",
};

export default Button;
