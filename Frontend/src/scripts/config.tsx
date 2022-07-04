export const API_BASE = "http://bloged.co:8080/api/v1";

export const GetUrl = (endpoint: string) => {
  return API_BASE + endpoint;
};
