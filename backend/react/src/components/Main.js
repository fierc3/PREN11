import React from 'react';
import { Routes, Route } from 'react-router-dom';

import Admin from './Management';
import Landing from './Guest';

const Main = () => {
  return (
    <Routes> {/* The Switch decides which component to show based on the current URL.*/}
      <Route exact path='/' component={Admin}></Route>
      <Route exact path='/signup' component={Landing}></Route>
    </Routes>
  );
}

export default Main;