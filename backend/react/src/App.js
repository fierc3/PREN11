import './App.css';
import React from "react";
import Management from './components/Management';
import Guest from './components/Guest';
import T02 from './components/T02';
import { useRoutes } from 'react-router';

const App = () => {
  let routes = useRoutes([
    { path: "admin", element: <Management /> },
    { path: "/", element: <Guest /> },
    { path: "/t02", element: <T02 /> },

  ]);
  return routes;
}

export default App;
