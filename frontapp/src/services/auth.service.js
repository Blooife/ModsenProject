import axios from "axios";

const baseURL = "http://localhost:5152";

const API_URL = "/api/auth";

const login = (email, password) => {
  return axios
    .post(baseURL + API_URL + "/login", {
      email,
      password,
    })
    .then((response) => {
      console.log(response);
      if (response && response.data && response.data.token) {

        localStorage.setItem("user", JSON.stringify(response.data));
        console.log(response.data);
      }
      return response;
    })
    .catch((error) => {
      throw error; 
    });
};


const loginWithRefreshToken = (refreshToken) => {
  return axios
    .post(baseURL + API_URL + "/refreshToken", { refreshToken, })
    .then((response) => {
      if (response.data.token) {
        localStorage.setItem("user", JSON.stringify(response.data));
        console.log("set item");
      }      
      return response;
    }
    );
};

const logout = () => {
  localStorage.removeItem("user");
};

const getCurrentUser = () => {
  return JSON.parse(localStorage.getItem("user"));
};

const register = (formData) => {
  return axios
    .post(baseURL + API_URL + "/register", 
      formData)
    .then((response) => {      
      
      return response;
    })
    .catch((error) => {
      throw error; 
    });
};

const authService = {
  login,
  loginWithRefreshToken,
  logout,
  getCurrentUser,
  register,
};

export default authService;