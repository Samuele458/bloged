import React from "react";
import PropTypes from "prop-types";

import "../../style/style.scss";

const Button = ({
  text,
  type,
  url,
  color,
  backgroundColor,
  hoverBackgroundColor,
  ...props
}) => {
  let ButtonTag;

  if (type === "button") {
    ButtonTag = "button";
  } else if (type === "link") {
    ButtonTag = "a";
  } else if (type === "submit") {
    ButtonTag = "button";
  }

  return (
    <ButtonTag
      className="button pt-1 pb-1 pr-4 pl-4"
      style={{
        color: color,
        backgroundColor: backgroundColor,
        "&:hover": { backgroundColor: "green" },
      }}
    >
      {text}
    </ButtonTag>
  );
};

Button.propTypes = {
  text: PropTypes.string,
  url: PropTypes.string,
  type: PropTypes.oneOf(["button", "link", "submit"]),
  color: PropTypes.color,
  backgroundColor: PropTypes.color,
};

Button.defaultProps = {
  text: "Button",
  url: null,
  type: "button",
  color: PropTypes.color(),
  backgroundColor: "#ffffff",
};

export default Button;
