import React from "react";
import { BackendContext } from "../contexts/backendContext";

export const useBackend = () => {
  const context = React.useContext(BackendContext);

  if (context === undefined) {
    throw new Error("`useHook` must be within a `provider`");
  }

  return context;
};
