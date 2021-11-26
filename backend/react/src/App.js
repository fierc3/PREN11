import './App.css';
import React from "react";
import Management from './components/Management';
import Guest from './components/Guest';
import { useRoutes } from 'react-router';

const App = () => {
  let routes = useRoutes([
    { path: "admin", element: <Management /> },
    { path: "/", element: <Guest /> }
  ]);
  return routes;
}

export default App;
