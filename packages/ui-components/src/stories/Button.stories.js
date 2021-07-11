import React from "react";

import Button from "../components/Button";

export default {
  title: "Components/Button",
  component: Button,
};

const Template = (args) => <Button {...args} />;

export const Default = Template.bind({});
Default.args = {
  text: "Button",
  url: "#",
  type: "button",
  color: "#000000",
  backgroundColor: "#ffffff",
  hoverBackgroundColor: "#dddddd",
};
