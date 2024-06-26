import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import PostService from "../services/post.service";
import AuthService from "../services/auth.service";
import handleRefresh from './refresh';

const EditEvent = () => {
  const user = AuthService.getCurrentUser();
  const { id } = useParams();
  const [errorMessage, setErrorMessage] = useState('');
  const [formData, setFormData] = useState({
    id: "",
    name: "",
    description: "",
    date: "", 
    time: "", 
    place: "",
    picture: "",
    category : "",
    maxParticipants: 0,
  });
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  const [imageFile, setImageFile] = useState(null);

  const handleImageChange = (e) => {
    setImageFile(e.target.files[0]);
  };

  
  useEffect(() => {
    const fetchEventDetails = async () => {
      try {
        await PostService.getEventById(id).then((response)=>{
        setFormData({
          id: response.data.id,  
          name: response.data.name,
          description: response.data.description,
          place: response.data.place,
          date: response.data.date.split("T")[0], 
          time: response.data.date.split("T")[1], 
          category: response.data.category,
          maxParticipants: response.data.maxParticipants,
          picture: response.data.picture,
        });
        setLoading(false);},
        (error) => {
          if (error.response == null) {
            handleRefresh(user, navigate);
          }else{
            alert(error.response.data['ErrorMessage']);
          }
        });        
      }
      catch (err) {
        // Handle error
        setLoading(false);
        console.error("Error fetching event details:", err);
      }
    };

    fetchEventDetails();
  }, [id]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const [pict, setPicture] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    const { date, time, ...rest } = formData; 
    const dateTime = `${date}T${time}`;
    const formDataWithImage = new FormData();
    formDataWithImage.append('file', imageFile); 
    formDataWithImage.append('eventId', formData.id);
    try {
      if(imageFile != null){
        await PostService.uploadPicture(formDataWithImage).then(
          (response) =>{
            console.log(response);
            
            setPicture(response.data);
            console.log(pict);
            PostService.updateEvent({ ...rest, date: dateTime, picture: response.data }).then((response) => {
              navigate("/events");
            },
            (error) => {
              setErrorMessage(error.response.data['ErrorMessage']);
            });
          }
        )
      }else{
        await PostService.updateEvent({ ...rest, date: dateTime }).then((response) => {
          navigate("/events");
        },
        (error) => {
          setErrorMessage(error.response.data['ErrorMessage']);
        });
      }
      
    } catch (error) {
      console.error("Error updating event details:", error);
    }
  };
  


  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="container mt-4">
      <h2>Edit Event</h2>
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
        <div className="mb-3">
          <label htmlFor="photo" className="form-label">Photo:</label>
          <input type="file" className="form-control" id="photo" name="photo" onChange={handleImageChange} accept="image/*" />
        </div>

        <button type="submit" className="btn btn-primary">Update Event</button>
      </form>
      {errorMessage && <p>{errorMessage}</p>}
    </div>
  );
};

export default EditEvent;
