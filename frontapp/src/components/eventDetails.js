import React, { useState, useEffect } from "react";
import { useParams, useHistory } from "react-router-dom";
import PostService from "../services/post.service";
import AuthService from "../services/auth.service";
import { Link, useNavigate } from "react-router-dom";
import handleRefresh from './refresh';

const EventDetails = () => {
  const { id } = useParams();
  const [eventDetails, setEventDetails] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    place: "",
    date: "",
    maxParticipants: 0,
    placesLeft: 0,
  });
  const user = AuthService.getCurrentUser();

  useEffect(() => {
    const fetchEventDetails = async () => {
      try {
        await PostService.getEventById(id).then(
          (response)=>{
            setEventDetails(response.data);
            setFormData({
              id: response.data.id,
              name: response.data.name,
              description: response.data.description,
              place: response.data.place,
              date: response.data.date,
              maxParticipants: response.data.maxParticipants,
              placesLeft: response.data.placesLeft,
            });
            setLoading(false);
          },
          (error)=>{
            if (error.response == null) {
              handleRefresh(user, navigate);
            }
          }
        );
        
      } catch (error) {
        // Handle error
        setLoading(false);
        console.error("Error fetching event details:", error);
      }
    };

    fetchEventDetails();
  }, [id]);

  const handleRegister = async (e) => {
    e.preventDefault();
    try {
      await AuthService.registerOnEvent(user.user.id, id).then(
       ()=>{
        alert('You registered successfully!');
        window.location.reload();
       },
       (error)=>{
        if (error.response == null) {
          handleRefresh(user, navigate);
        }else{
          alert(error.response.data['ErrorMessage']);
        } 
       } 
      )
    } catch (error) {
      console.error("Error updating event details:", error);
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      
      {eventDetails && (
        <div className="card border-primary">
          <div className="card-header">
            {eventDetails.name}{" "}
          </div>
          <div className="card-body">
            <img
              src={`/pictures/${eventDetails.picture}`}
              className="card-img-top"
              alt={eventDetails.name}
              style={{ objectFit: "cover", height: "300px", width: "300px" }}
            />
            <p className="card-text">{eventDetails.description}</p>
            <p className="card-text">
              <small className="text-muted">Place: {eventDetails.place}</small>
            </p>
            <p className="card-text">
              <small className="text-muted">Date: {eventDetails.date.split("T")[0]}</small>
            </p>
            <p className="card-text">
              <small className="text-muted">Time: {eventDetails.date.split("T")[1]}</small>
            </p>
            <p className="card-text">
              <small className="text-muted">Max participants: {eventDetails.maxParticipants}</small>
            </p>
            <p className="card-text">
              <small className="text-muted">Places left: {eventDetails.placesLeft}</small>
            </p>
            {user && (
              <button onClick={handleRegister} className="btn btn-success">
                Register
              </button>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default EventDetails;
