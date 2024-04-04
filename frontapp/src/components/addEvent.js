import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import PostService from "../services/post.service";
import handleRefresh from './refresh';
import AuthService from "../services/auth.service";

const EditEvent = () => {
  const user = AuthService.getCurrentUser();
  const [errorMessage, setErrorMessage] = useState('');
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    date: "", 
    time: "", 
    place: "",
    category : "",
    maxParticipants: 0,
  });  

  const navigate = useNavigate();

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const { date, time, ...rest } = formData; 
    const dateTime = `${date}T${time}`;
    try {
      await PostService.addEvent({ ...rest, date: dateTime }).then((response)=>{        
          navigate("/events");
          window.location.reload();
      },
      (error)=>{
        if (error.response == null) {
          handleRefresh(user, navigate);
        }else{
          setErrorMessage(error.response.data['ErrorMessage']);
        }
      }
      );
    } catch (error) {
      console.error("Error updating event details:", error);
    }
  };


  return (
    <div className="container mt-4">
      <h2>Add Event</h2>
      <form onSubmit={handleSubmit}>
      <div className="mb-3">
          <label htmlFor="name" className="form-label">Name:</label>
          <input type="text" className="form-control" id="name" name="name" value={formData.name} onChange={handleInputChange} required />
        </div>
        <div className="mb-3">
          <label htmlFor="description" className="form-label">Description:</label>
          <textarea className="form-control" id="description" name="description" value={formData.description} onChange={handleInputChange} required />
        </div>
        <div className="mb-3">
          <label htmlFor="place" className="form-label">Place:</label>
          <input type="text" className="form-control" id="place" name="place" value={formData.place} onChange={handleInputChange} required />
        </div>
        <div className="mb-3">
          <label htmlFor="place" className="form-label">Category:</label>
          <input type="text" className="form-control" id="category" name="category" value={formData.category} onChange={handleInputChange} required />
        </div>
        <div className="mb-3">
          <label htmlFor="maxParticipants" className="form-label">Max Participants:</label>
          <input type="number" className="form-control" id="maxParticipants" name="maxParticipants" value={formData.maxParticipants} onChange={handleInputChange} required />
        </div>
        <div className="mb-3">
          <label htmlFor="date" className="form-label">Date:</label>
          <input type="date" className="form-control" id="date" name="date" value={formData.date} onChange={handleInputChange} required />
        </div>
        <div className="mb-3">
          <label htmlFor="time" className="form-label">Time:</label>
          <input type="time" className="form-control" id="time" name="time" value={formData.time} onChange={handleInputChange} required />
        </div>
        <button type="submit" className="btn btn-primary">Add Event</button>
      </form>
      {errorMessage && <p>{errorMessage}</p>}
    </div>
  );
};

export default EditEvent;
