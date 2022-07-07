import '../App.css';
import React, { useState, useEffect } from "react";
import ReactConsole from '@webscopeio/react-console';
import { config } from '../Constants';
import { getMainSocket, disconnectMain } from './MainSocketSingleton';
var ENDPOINT = config.url.API_URL;

const Management = () => {
  const [activeCons, setActiveCons] = useState(-1)
  const [robotMessages, setRobotMessages] = useState([
    "Robot Messages will be displayed here"
  ])



  const connect = () => {
    let socket = getMainSocket(ENDPOINT)
    socket.on("RobotOutput", roboMessage => {
      console.log("Received on Robotoutput " + roboMessage);
      setRobotMessages(arr => [...arr, roboMessage + "|" + new Date().getTime()])
    })
  }

  const sendStart = () => {
    let socket = getMainSocket(ENDPOINT);
    socket.emit('RobotInput', JSON.stringify({event_type:"START"}));
  }

  const sendEnd = () => {
    let socket = getMainSocket(ENDPOINT);
    socket.emit('RobotInput', JSON.stringify({event_type:"END"}));
  }

  const sendPlant = (name, url) => {
    let socket = getMainSocket(ENDPOINT);
    socket.emit('RobotInput', JSON.stringify({event_type:"PLANT", event_value:{plantName:name,image:url}}));
  }



  useEffect(() => {
    connect();
  }, []);

  const calcAverage = (numbers) => {
    const sum = numbers.reduce((a, b) => a + b, 0);
    const avg = (sum / numbers.length) || 0;
    return avg;
  }

  const customCommands = {
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
    rickroll: {
      description: '¯\\_(ツ)_/¯.',
      fn: (...args) => {
        return new Promise((resolve, reject) => {
          setTimeout(() => {
            window.open("https://www.youtube.com/watch?v=dQw4w9WgXcQ")
            resolve("hihihihi");
          }, 1000)
        })
      }
    },
    help: {
      description: 'List hints & commands',
      fn: (...args) => {
        return new Promise((resolve, reject) => {
          setTimeout(() => {
            var res = Object.entries(customCommands).map((key, value) => key[0] + " - " + key[1].description + "\n")
            resolve(res.join(""));
          }, 1000)
        })
      }
    },
    forceconnect: {
      description: 'Force to create new connection',
      fn: (...args) => {
        return new Promise((resolve, reject) => {
          connect();
          resolve();
        })
      }
    },
    setport: {
      description: 'Set New Port',
      fn: (...args) => {
        return new Promise((resolve, reject) => {
          var port = args[0]
          ENDPOINT = "http://localhost:" + port;
          resolve("Updated port to " + port);
        })
      }
    },
    setendpoint: {
      description: 'Set New Endpoint (https://xxx:xx)',
      fn: (...args) => {
        return new Promise((resolve, reject) => {
          ENDPOINT = args[0]
          resolve("Updated endpoint to " + ENDPOINT);
        })
      }
    },
    endpoint: {
      description: 'Display Endpoint',
      fn: (...args) => {
        return new Promise((resolve, reject) => {
          resolve("Current " + ENDPOINT);
        })
      }
    },
    disconnectmain: {
      description: 'Disconnects main socket',
      fn: (...args) => {
        return new Promise((resolve, reject) => {
          disconnectMain();
          resolve("Disconnected main socket, use forceconnect to reconned ");
        })
      }
    },
    sendStart: {
      description: 'Send start run',
      fn: (...args) => {
        return new Promise((resolve, reject) => {
            sendStart()
            resolve("Done");
        })
      }
    },
    sendEnd: {
      description: 'Send end run',
      fn: (...args) => {
        return new Promise((resolve, reject) => {
            sendEnd()
            resolve("Done");
        })
      }
    },
    sendPlant: {
      description: 'Send plant',
      fn: (...args) => {
        return new Promise((resolve, reject) => {
            sendPlant(args[0], args[1])
            resolve("Done");
        })
      }
    }
  }

  return (
    <main className="area" >
      <h1>PREN 11</h1>
      <div className="live-data" style={{ borderBottom: "3px solid green" }} >
        <h3>Analytics</h3>
        <div style={{ display: 'flex', flexDirection: 'row', width: '100v', height:'30vh' }}>
          <div style={{ flexGrow: 1 }}>
            <p>
            </p>
            <p>
            </p>
            <h2>
            </h2>
          </div>
          <div style={{ flexGrow: 1, textAlign: 'end',  height:'100%', paddingRight: '3em',}}>
          <p>active connections: {activeCons}</p>
          <div style={{float: 'right', paddingRight: '10px', overflow:'auto', height:'100px'}}>
            {robotMessages.map(x => {
              return(<p> {x} </p>)
            })}
            </div>
          </div>
        </div>
      </div>
      <div className="console" style={{ height: "50%%", flexGrow: "1", display: "flex", flexDirection: "column", zIndex: 100, background: "none" }}>
        <ReactConsole
          wrapperStyle={{ flexGrow: "1" }}
          autoFocus
          welcomeMessage={"######################\n Welcome to PREN11 CLI\n###################### \n Use help for all commands"}
          commands={customCommands}
        />

      </div>

    </main>
  );
}

export default Management;