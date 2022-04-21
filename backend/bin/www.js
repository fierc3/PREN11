#!/usr/bin/env node

/**
 * Module dependencies.
 */

var app = require('../app');
var debug = require('debug')('backend:server');
var http = require('http');
const socketIo = require("socket.io");
const db = require('../db');


var httpServer = http.createServer(app);
startHttpServer();

//sockets
const io = socketIo(httpServer, {
  cors: {
    origin: '*',
  },
  allowEIO3: true
})
let countConnections = 0;

io.on("connection", (socket) => {
  countConnections++;
  console.log(`New client connected, count[${countConnections}]`);

  socket.on("RobotInput", (msg) => {
    msg = JSON.parse(msg);
    console.log(`Robot sent message ${msg}`);
    if (msg.event_type === 'START') {
      db.beginRun();
    } else if (msg.event_type === 'END') {
      db.endRun();
    } else if (msg.event_type === 'PLANT') {
      if (msg.event_value)
        db.insertPlant(JSON.stringify(msg.event_value))
    } else {
      console.log("Command not recognized", msg);
    }

    //emitRobotMessage(socket, msg + "|" + new Date().getTime())
  });
  socket.on("disconnect", () => {
    countConnections--;
    console.log("Client disconnected");
  });
});

db.init(io);

const emitRobotMessage = (msg) => {
  console.log(`Emitting broadcast: ${msg}`);
  io.emit("RobotOutput", msg);
}


function normalizePort(val) {
  var port = parseInt(val, 10);

  if (isNaN(port)) {
    // named pipe
    return val;
  }

  if (port >= 0) {
    // port number
    return port;
  }

  return false;
}


function startHttpServer() {

  var port = normalizePort(process.env.PORT || '3001');
  app.set('port', port);
  httpServer.listen(port);
  httpServer.on('error', onError);
  httpServer.on('listening', onListening);
  console.log(`Started HTTP on ${port}`)
}

/**
 * Event listener for HTTP server "error" event.
 */

function onError(error) {
  if (error.syscall !== 'listen') {
    throw error;
  }

  var bind = typeof port === 'string'
    ? 'Pipe ' + port
    : 'Port ' + port;

  // handle specific listen errors with friendly messages
  switch (error.code) {
    case 'EACCES':
      console.error(bind + ' requires elevated privileges');
      process.exit(1);
      break;
    case 'EADDRINUSE':
      console.error(bind + ' is already in use');
      process.exit(1);
      break;
    default:
      throw error;
  }
}

/**
 * Event listener for HTTP server "listening" event.
 */

function onListening() {
  var addr = httpServer.address();
  var bind = typeof addr === 'string'
    ? 'pipe ' + addr
    : 'port ' + addr.port;
  debug('Listening on ' + bind);
}