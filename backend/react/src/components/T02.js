import socketIOClient from "socket.io-client";
import React, { useState, useEffect } from "react";
import '../App.css';

const T02 = () => {
    const [latest, setLatest] = useState("")
    const [activeCons, setActiveCons] = useState(-1)
    const [avg, setAvg] =useState(0)

    var ENDPOINT_CURRENT = 'https://tactile-rigging-333212.oa.r.appspot.com'

    const createConnection = () => {
        let socket = socketIOClient(ENDPOINT_CURRENT, {transports: ['websocket'],
        secure: true});
        socket.on("RobotOutput", roboMessage => {
            roboMessage = roboMessage + "|" +new Date().getTime();
            let msgParts = roboMessage.split("|");
            let start = parseInt(msgParts[0]);
            setLatest(new Date().getTime()-start);
        })
        socket.on("ClientCount", data => {
            const activeConData = data;
            //active connections
            setActiveCons(activeConData)
          });
        setTimeout(() => { if(activeCons < 200){createConnection()} }, 500);
    }

    useEffect(() => {
        createConnection();
    }, [])

    return (
        <>
            <h1>t02</h1>
            <h3>Active Connections: {activeCons}</h3>
            <h3>Latest Connection Speed: {latest}</h3>
            <button onClick={() => {
                let socket = socketIOClient(ENDPOINT_CURRENT, {transports: ['websocket'],
                secure: true});
                socket.emit('robot', new Date().getTime());
            }}> Send Message</button>
        </>
    );
}

export default T02;
