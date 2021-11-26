import '../App.css';
import React from "react";

const Guest = () => {
    return (
        <>
            <h1>Hello Guest</h1>
            <h2>The server is running, nothing for you to worry about</h2>
            <div style={{width:"100vw", display: 'flex',alignItems:'center'}}>
            <img src="https://c.tenor.com/xIuewwtwSd4AAAAC/jontron-squats.gif" style={{  margin: 'auto', width: '20%'}}></img>
            </div>
        </>
    );
}

export default Guest;
