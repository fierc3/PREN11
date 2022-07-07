package com.pren11.server;
import com.pren11.Config;
import io.socket.client.IO;
import io.socket.client.Socket;
import io.socket.emitter.Emitter;

import java.net.URI;

import static java.lang.System.out;
import static java.lang.System.err;

public class ServerCommunicator {

    Socket io;

    public ServerCommunicator(String url){
        init(url);
    }

    private void init(String url)
    {
        if(Config.OFFLINE_MODE) return;
        out.println("Setting up SocketIO Client");
        URI uri = URI.create(url);
        IO.Options options = IO.Options.builder()
                // ...
                .build();

        io = IO.socket(uri, options);
        io.on("connection", objects -> {
            out.println("Socket Setup with Server");
        });

        io.on("error", o -> {
            out.println("ERRROR SOCKET: " + o.toString());
        });

        io.connect();

        do{
            System.out.println("Is Connected? " + io.connected());
        }while(io.connected() == false);
        System.out.println("Final Connection Test? " + io.connected());


       // await client.ConnectAsync();
        Runtime.getRuntime().addShutdownHook(new Thread(new Runnable() {

            public void run() {
                // Do what you want when the application is stopping
                System.out.println("OHNOOOOOO, gotta tip quick");
                disconnect();
            }
        }));
    }

    public void disconnect() {
        io.disconnect();
        io.close();
    }


    public void sendStart()
    {
        if(Config.OFFLINE_MODE) return;
        sendEvent("START");
    }

    public void sendEnd()
    {
        if(Config.OFFLINE_MODE) return;
        sendEvent("END");
    }

    private void sendEvent(String sendEvent)
    {
        if(Config.OFFLINE_MODE) return;
        out.println("Sending " + sendEvent + " Event to Server, active: " + io.isActive() + " connected: " + io.connected());
        io.emit("RobotInput", "{\"event_type\":\"" + sendEvent + "\"}");

    }

    public void sendPlant(String plantName, String image)
    {
        if(Config.OFFLINE_MODE) return;
        out.println("Sending Plant" + plantName + " to Server");
        var emitter = io.emit("RobotInput","{\"event_type\":\"PLANT\", \"event_value\":{\"plantName\":\""+plantName+"\",\"image\":\""+image+"\"}}");
    }


}
