import axios from "axios";
import authHeader from "./auth-header";
import handleRefresh from '../components/refresh';
import { useNavigate, Link } from "react-router-dom";
import AuthService from './auth.service';

const baseURL = "http://localhost:5152";

const API_URL = "/api/v1/";



const getAllEvents = () => {
  return axios.get(baseURL+API_URL+"events", { headers: authHeader() })
  .catch((error) => {  
    console.log("lalal");  
    
    throw error; 
  });
};

const getEventById = (eventId) => {
  return axios.get(baseURL+API_URL+"events/"+eventId, { headers: authHeader() })
  .catch((error) => {
    throw error; 
  });
};

const deleteEvent = (eventId) => {
  return axios.delete(baseURL+API_URL+"events/"+eventId, { headers: authHeader() })
  .catch((error) => {
    throw error; 
  });
};

const updateEvent = (formData) => {
  console.log(formData);
  return axios.put(baseURL+API_URL+"events", formData, { headers: authHeader() })
  .catch((error) => {
    throw error; 
  });
};

const addEvent = (formData) => {
  return axios.post(baseURL+API_URL+"events", formData, { headers: authHeader() })
  .catch((error) => {
    throw error; 
  });
};

const getFilteredEvents = (category, place) => {
  return axios.post(baseURL+API_URL+"events/getFilteredEvents", { category, place }, { headers: authHeader() })
  .catch((error) => {
    throw error; 
  });
};

const uploadPicture = (formDataWithImage) => {
  return axios.post(baseURL+API_URL+"events/upload", formDataWithImage, { headers: authHeader() })
  .catch((error) => {
    throw error; 
  });
};


const postService = {
  getAllEvents,
  getEventById,
  updateEvent,
  addEvent,
  deleteEvent,
  getFilteredEvents,
  uploadPicture,
};

export default postService;