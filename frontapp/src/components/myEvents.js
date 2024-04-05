import React, { useState, useEffect } from "react";
import AuthService from "../services/auth.service";
import PostService from "../services/post.service";
import { useNavigate, Link } from "react-router-dom";
import handleRefresh from './refresh';

const Events = () => {
  const [privatePosts, setPrivatePosts] = useState([]);
  const [refreshEvents, setRefreshEvents] = useState(false); 
  const user = AuthService.getCurrentUser();
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    PostService.getUsersEvents(user.user.id).then(
      (response) => {
        setPrivatePosts(response.data);
      },
      async (error) => {
        if (error.response == null) {
          handleRefresh(user, navigate);
        }else{
          alert(error.response.data['ErrorMessage']);
        }          
      }
    );
  }, [navigate, refreshEvents]); 

  const handleUnregister = (eventId) => async (e) => {
    e.preventDefault();
    try {
      await PostService.unregisterFromEvent(user.user.id, eventId).then(
        () => {
          setRefreshEvents((prev) => !prev); 
        },
        (error) => {
          if (error.response == null) {
            handleRefresh(user, navigate);
          }else{
            setErrorMessage(error.response.data['ErrorMessage'])
          }            
        },)        
    } catch (error) {
      console.error("Error unregistering from event:", error);
    }
  };

  return (
    <div>
      <div className="mt-3">
        <h2>My events</h2>
        <div className="row row-cols-1 row-cols-md-3 g-4">
          {privatePosts.map((post) => (
            <div className="col" key={post.id}>
              <div className="card border-primary">
                <div className="card-header">
                  <Link to={`/eventDetails/${post.id}`}>{post.name}</Link>
                </div>
                <img
                  src={`/pictures/${post.picture}`}
                  className="card-img-top"
                  alt={post.name}
                  style={{ objectFit: "cover", height: "300px" }}
                />
                <div className="card-body">
                  <p className="card-text">{post.description}</p>
                  <button
                    onClick={handleUnregister(post.id)}
                    className="btn btn-danger"
                  >
                    Unregister
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default Events;
