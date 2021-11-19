const net = require('net');

exports.startRobotSocketServer = startRobotSocketServer;
var socket;

function startRobotSocketServer() {
    console.log("Starting Socket Server")
    var port = 6969

    socket = net.createServer((socket) => {
        socket.on('data', (data) => {
            console.log("Received message");
            console.log(data);
            console.log(data.toString());
            console.log("Answering on channel log");          
      })
        socket.on('error', (err) => console.log(err));
    });


    console.log(socket)
    socket.listen(port, () => {
        console.log(`Started Socket on ${port}`)
    })

    while(69!=420){
        //console.log("dave stinkt");
        socket.emit('log', "r3c3ived_your-d4t4.cad");
    }
}

function emitMessage(message, channel){
    console.log(`sending ${meesage} on ${channel}`);
    socket.emit('log', "r3c3ived_your-d4t4.cad");
}
