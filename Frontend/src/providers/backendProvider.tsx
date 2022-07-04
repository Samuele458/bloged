import axios from "axios";
import React from "react";
import { BackendContext } from "../contexts/backendContext";
import { GetUrl } from "../scripts/config";

export interface LoginRequestDto {
  email: string;
  password: string;
}

export interface LoginResponseDto {}

const BackendProvider = (props: any) => {
  const LogIn = async (email: string, password: string) => {
    let body: LoginRequestDto = {
      email: email,
      password: password,
    };

    console.log(GetUrl("/Accounts/Login"));
    let res = await axios.post(GetUrl("/Accounts/Login"), body);

    console.log(res.data);
  };

  return (
    <BackendContext.Provider value={{ LogIn }}>
      {props.children}
    </BackendContext.Provider>
  );
};

export default BackendProvider;
