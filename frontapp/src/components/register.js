import React, { useState } from 'react';
import { useNavigate } from "react-router-dom";
import AuthService from "../services/auth.service";

const Register = () => {
  const [formData, setFormData] = useState({
    email: '',
    name: '',
    lastName: '',
    birthDate: '',
    password: ''
  });
  const [responseMessage, setResponseMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  const navigate = useNavigate();
  
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await AuthService.register(formData).then(
        (response) => {
          navigate("/login");
          window.location.reload();
        },
        (error) => {
          setErrorMessage(error.response.data['ErrorMessage'])
        }
      );
    } catch (err) {
    }
  };

  const togglePasswordVisibility = () => {
    setFormData({
      ...formData,
      showPassword: !formData.showPassword  
    });
  };

  return (
    <div className="container">
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
        <h3 className="Auth-form-title">Sign Up</h3>
          <label htmlFor="email" className="form-label">Email:</label>
          <input type="email" id="email" name="email" value={formData.email} onChange={handleChange} className="form-control" required />
        </div>
        <div className="mb-3">
          <label htmlFor="name" className="form-label">Name:</label>
          <input type="text" id="name" name="name" value={formData.name} onChange={handleChange} className="form-control" />
        </div>
        <div className="mb-3">
          <label htmlFor="lastName" className="form-label">Lastname:</label>
          <input type="text" id="lastName" name="lastName" value={formData.lastName} onChange={handleChange} className="form-control" required />
        </div>
        <div className="mb-3">
          <label htmlFor="birthDate" className="form-label">Date of birth:</label>
          <input type="date" id="birthDate" name="birthDate" value={formData.birthDate} onChange={handleChange} className="form-control" required />
        </div>
        <div className="mb-3">
          <label htmlFor="password" className="form-label">Password:</label>
          <div className="input-group">
            <input type={formData.showPassword ? 'text' : 'password'} id="password" name="password" value={formData.password} onChange={handleChange} className="form-control" required />
            <button type="button" className="btn btn-outline-secondary" onClick={togglePasswordVisibility}>
              {formData.showPassword ? 'Hide' : 'Show'}
            </button>
          </div>
        </div>
        <button type="submit" className="btn btn-primary">Register</button>
      </form>
      {errorMessage && <p className="text-danger">{errorMessage}</p>}
    </div>
  );
};

export default Register;
