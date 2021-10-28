#!/usr/bin/env node

/**
 * Module dependencies.
 */

var app = require('../app');
var debug = require('debug')('backend:server');
var http = require('http');
const net = require('net');

var httpServer = http.createServer(app);
startHttpServer();
startRobotSocketServer();




function startHttpServer() {

  console.log("Starting HTTP Server")
  var port = normalizePort(process.env.PORT || '3000');
  app.set('port', port);
  httpServer.listen(port);
  httpServer.on('error', onError);
  httpServer.on('listening', onListening);
  console.log(`Started HTTP on ${port}`)
}

function startRobotSocketServer() {
  console.log("Starting Socket Server")
  var port = 6969

  const socket = net.createServer((socket) => {
    socket.on('data', (data) => {
      console.log(data.toString());
    })
  }).on('error', (err) => {
    console.error(err);
  });

  socket.listen(port, () => {
    console.log(`Started Socket on ${port}`)
  })

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