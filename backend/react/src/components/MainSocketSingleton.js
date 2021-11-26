import socketIOClient from "socket.io-client";

var mainSocket = undefined;
var ENDPOINT_CURRENT = undefined

export const getMainSocket = (ENDPOINT) => {
    console.log("get main socket", ENDPOINT)
  if(mainSocket === undefined || ENDPOINT_CURRENT !== ENDPOINT){
    ENDPOINT_CURRENT = ENDPOINT;
    mainSocket=socketIOClient(ENDPOINT_CURRENT);
  }
  return mainSocket;
}

export const disconnectMain = () => {
    if(!mainSocket){
        console.log("disonnect not possible, no existing connection")
        return;
    }

    mainSocket.disconnect();
}

