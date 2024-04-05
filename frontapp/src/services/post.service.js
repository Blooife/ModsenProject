import axios from "axios";
import authHeader from "./auth-header";

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

const registerOnEvent = (userId, eventId) => {
  return axios
    .post(baseURL + API_URL + "events/registerOnEvent/",  { userId, eventId}, { headers: authHeader() } )
    .then((response) => {
      
      return response;
    }).catch((error) => {
      throw error; 
    });
};

const unregisterFromEvent = (userId, eventId) => {
  console.log(eventId);
  return axios
    .post(baseURL + API_URL + "events/unRegisterOnEvent/",  { userId, eventId }, { headers: authHeader() })
    .then((response) => {
      
      return response;
    }).catch((error) => {
      throw error; 
    });
};

const getUsersEvents = (userId) => {
  return axios
    .get(baseURL + API_URL + "events/getEvents/"+userId, { headers: authHeader() })
    .then((response) => {
      
      return response;
    }).catch((error) => {
      throw error; 
    });
};

const getPagedEvents = (pageNumber, pageSize) => {
  return axios
    .post(baseURL + API_URL + "events/getPagedEvents",  { pageNumber, pageSize }, { headers: authHeader() })
    .then((response) => {     
      return response;
    }).catch((error) => {
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
  registerOnEvent,
  unregisterFromEvent,
  getUsersEvents,
  getPagedEvents,
};

export default postService;