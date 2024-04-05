import React, { useState, useEffect } from "react";
import PostService from "../services/post.service";
import AuthService from "../services/auth.service";
import { useNavigate, Link } from "react-router-dom";
import handleRefresh from './refresh';

const AdminPage = () => {
  const [privatePosts, setPrivatePosts] = useState([]);

  const user = AuthService.getCurrentUser();
  const navigate = useNavigate();

  useEffect(() => {
    console.log(user);
    const fetchPosts = async () => {
      await PostService.getAllEvents().then(
        (response)=>{
          setPrivatePosts(response.data);
        },
        async (error)=>{
          if (error.response == null) {
            handleRefresh(user, navigate);
          }
        }      
    )};       
    fetchPosts();
  }, [navigate]);
   

  const handleDelete = async (postId) => {
    try {
      await PostService.deleteEvent(postId).then(
        (response)=>{
          const updatedPosts = privatePosts.filter((post) => post.id !== postId);
          setPrivatePosts(updatedPosts);
          alert("You deleted event successfully");
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
      console.error("Error deleting event:", error);
    }
  };

  return (
    <div>
      <div className="mt-3">
        <h2>Admin Panel</h2>
        <Link to="/addEvent" className="btn btn-primary mb-3">Add Event</Link>
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
                  <button className="btn btn-danger" onClick={() => handleDelete(post.id)}>Delete</button>
                  <Link to={`/editEvent/${post.id}`} className="btn btn-primary">
                    Edit
                  </Link>
                </div>
              </div>
            </div>
        ))},
        </div>
      </div>
    </div>
  );
};

export default AdminPage;
