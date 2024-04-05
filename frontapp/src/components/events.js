import React, { useState, useEffect } from "react";
import PostService from "../services/post.service";
import AuthService from "../services/auth.service";
import { useNavigate, Link } from "react-router-dom";
import { categoryOptions, placeOptions } from "../components/optionsData";
import handleRefresh from './refresh';

const Events = () => {
  const [privatePosts, setPrivatePosts] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [currentPage, setCurrentPage] = useState(1); 
  const [selectedCategory, setSelectedCategory] = useState("");
  const [selectedPlace, setSelectedPlace] = useState("");
  const [totalPages, setTotalPages] = useState(1);
  const [totalItems, setTotalItems] = useState(0);

  const pageSize = 2;

  const user = AuthService.getCurrentUser();
  const navigate = useNavigate();

  useEffect(() => {
    console.log(user);
    const fetchPosts = async () => {
      await getEventsWithPagination(currentPage);
    };       
    fetchPosts();
  }, [currentPage]);

  const getEventsWithPagination = async (page) => {
    await PostService.getPagedEvents( page, pageSize ).then(
      (response)=>{
        setPrivatePosts(response.data);
        setTotalPages(response.headers['x-pagination'] ? JSON.parse(response.headers['x-pagination']).TotalPages : 1);
        setTotalItems(response.headers['x-pagination'] ? JSON.parse(response.headers['x-pagination']).TotalCount : 0);},
      (error)=>{
        if (error.response == null) {
          handleRefresh(user, navigate);
        }
      }  
    )    
  };

  const handleCategoryChange = (e) => {
    setSelectedCategory(e.target.value);
  };
  
  const handlePlaceChange = (e) => {
    setSelectedPlace(e.target.value);
  };

  const handleSearch = (e) => {
    setSearchTerm(e.target.value);
    setCurrentPage(1); 
  };

  const paginate = (pageNumber) => {
    if (pageNumber >= 1 && pageNumber <= totalPages) {
      setCurrentPage(pageNumber);
    }
  };

  const applyFilters = async(category, place) => {
    setCurrentPage(1); // Сбрасываем текущую страницу при применении фильтров
    const response = await PostService.getFilteredEvents(category, place, user.accessToken);
    setPrivatePosts(response.data);
    setTotalPages(response.headers['x-Pagination'] ? JSON.parse(response.headers['x-Pagination']).TotalPages : 1);
    setTotalItems(response.headers['x-Pagination'] ? JSON.parse(response.headers['x-Pagination']).TotalCount : 0);
    console.log(totalPages+"ldldl");
  };

  return (
    <div>
      <div className="mt-3">
        <h2>Events</h2>
        <select className="form-select mb-3" value={selectedCategory} onChange={handleCategoryChange}>
          {categoryOptions.map((option) => (
            <option key={option.value} value={option.value}>{option.label}</option>
          ))}
        </select>
        <select className="form-select mb-3" value={selectedPlace} onChange={handlePlaceChange}>
          {placeOptions.map((option) => (
            <option key={option.value} value={option.value}>{option.label}</option>
          ))}
        </select>
        <button className="btn btn-primary mb-3" onClick={() => applyFilters(selectedCategory, selectedPlace)}>
          Apply Filters
        </button>
        <button className="btn btn-primary mb-3" onClick={() => window.location.reload()}>
          Cancel Filters
        </button>
        <div className="input-group mb-3">
          <input
            type="text"
            className="form-control"
            placeholder="Search by name..."
            value={searchTerm}
            onChange={handleSearch}
          />
        </div>
        
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
                </div>
              </div>
            </div>
          ))}
        </div>
        {/* Пагинация */}
        <ul className="pagination justify-content-center">
          {Array.from({ length: totalPages }, (_, i) => (
            <li className={`page-item ${currentPage === i + 1 ? 'active' : ''}`} key={i}>
              <button className="page-link" onClick={() => paginate(i + 1)}>{i + 1}</button>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export default Events;
