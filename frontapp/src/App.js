import React from "react";
import { Routes, Route, BrowserRouter } from "react-router-dom";
import Login from "./components/login";
import Events from "./components/events";
import AdminPage from "./components/adminPage";
import Header from './components/Header';
import Register from './components/register';
import Home from './components/home';
import EventDetails from './components/eventDetails';
import EditEvent from './components/editEvent';
import AddEvent from './components/addEvent';
import MyEvents from './components/myEvents';

function App() {
  return (
    <div>

      <div className="container-md mt-5">
        <BrowserRouter>
          <Header/>
          <Routes>
          <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/events" element={<Events />} />
            <Route path="/adminPage" element={<AdminPage />} />
            <Route path="/myEvents" element={<MyEvents />} />
            <Route path="/eventDetails/:id" element={<EventDetails />} />
            <Route path="/editEvent/:id" element={<EditEvent />} />
            <Route path="/addEvent" element={<AddEvent />} />
          </Routes>
        </BrowserRouter>
      </div>
    </div>
  );
}

export default App;
