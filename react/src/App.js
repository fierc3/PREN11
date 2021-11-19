import './App.css';
import React, { useState, useEffect } from "react";
import socketIOClient from "socket.io-client";
import ReactConsole from '@webscopeio/react-console';
const ENDPOINT = "http://localhost:3000";

const App = () => {
  const [response, setResponse] = useState("NOTYETLOADED")
  const [localTime, setLocalTime] = useState("NOTYETLOADED")
  const [diffTime, setDiffTime] = useState(0)
  const [history, setHistory] = useState([
    "History: "
  ])
  const [health, setHealth] = useState("UNKNOWN");



  useEffect(() => {
    const socket = socketIOClient(ENDPOINT);
    socket.on("FromAPI", data => {
      const currTime = new Date().getTime();
      const remoteMs = Date.parse(data);

      const diff = currTime - remoteMs;

      setDiffTime(diff);
      setResponse(remoteMs + "");
      setLocalTime(currTime + "");
    });
  }, []);

  const healthCheck = ()  => {
    const currTime = new Date().getTime();
    var state = "UNKNOWN"
    if(localTime - 3000 > currTime){
      state = "CRITICAL"
    }else{
      state ="HEALTHY"
    }
    setHealth(state);
    return state;
  }

  return (
    <main className="area" >
      <h1>PREN 11</h1>
      <div className="live-data" style={{ borderBottom: "3px solid green" }} >
        <h3>Analytics (HEALTH CHECK: <span className={`health-result ${health.toLowerCase()}`}>{health}</span>)</h3>
        <p>
          Time on Remote &emsp; {response}
        </p>
        <p>
          Time  on Localhost &emsp; {localTime}
        </p>
        <h2>Difference in MS: <span>{diffTime}</span>
        </h2>
      </div>
      <div className="console" style={{ height: "50%%", flexGrow: "1", display: "flex", flexDirection: "column" }}>
        <ReactConsole
          wrapperStyle={{ flexGrow: "1" }}
          autoFocus
          welcomeMessage="Welcome"
          commands={{
            history: {
              description: 'History',
              fn: () => new Promise(resolve => {
                resolve(`${history.join('\r\n')}`)
              })
            },
            echo: {
              description: 'Echo',
              fn: (...args) => {
                return new Promise((resolve, reject) => {
                  setTimeout(() => {
                    resolve(`${args.join(' ')}`)
                  }, 2000)
                })
              }
            },
            test: {
              description: 'Test',
              fn: (...args) => {
                return new Promise((resolve, reject) => {
                  setTimeout(() => {
                    resolve('Hello world \n\n hello \n')
                  }, 2000)
                })
              }
            },
          health: {
            description: 'Check health of backend server',
            fn: (...args) => {
              return new Promise((resolve, reject) => {
                setTimeout(() => {
                  setHistory([...history, "health"])
                  resolve(healthCheck())
                }, 1000)
              })
            }
          }
          }}
        />

      </div>

    </main>
  );
}

export default App;
