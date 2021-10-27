import React from "react";

import SignUpForm from "../components/SignUpForm";

export default {
  title: "Components/Authentication/SignUpForm",
  component: SignUpForm,
};

const Template = (args) => <SignUpForm {...args} />;

export const Default = Template.bind({});
