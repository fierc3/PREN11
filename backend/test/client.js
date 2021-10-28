const net = require('net');

var port = 6969
var host = "pren11backendconcept.azurewebsites.net"
//var host = "localhost"

const client = net.createConnection({ port:port, host: host}, () => {
  console.log('CLIENT: I connected to the server.');
  client.write('CLIENT: Hello this is client!');
});
client.on('data', (data) => {
  console.log(data.toString());
  client.end();
});
client.on('end', () => {
  console.log('CLIENT: I disconnected from the server.');
});