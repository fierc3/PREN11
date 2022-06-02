package com.pren11.modes;
import com.beust.jcommander.Strings;
import com.pren11.Config;
import com.pren11.Utils;
import com.pren11.camera.CameraModule;
import com.pren11.camera.PiCameraModule;
import com.pren11.camera.WebCameraModule;
import com.pren11.detector.QrCodeDetector;
import com.pren11.server.PlantIdApiHandler;
import com.pren11.server.ServerCommunicator;
import com.pren11.tiny.Speedometer;

import java.util.concurrent.TimeUnit;

import static java.lang.System.*;

public class Parcour implements Run{

    @Override
    public void run (String[] args) {
        if(Config.USE_CAMERA == Config.CAMERA_ID_PI)Speedometer.getInstance().fullStop();
        out.println("++++++ PREN11 PiController Java Edition is starting ++++++");
        out.println("Using args: " + Strings.join(",",args));
        CameraModule camera = Config.USE_CAMERA == Config.CAMERA_ID_USB ? new WebCameraModule() : new PiCameraModule();


        var runUuid = java.util.UUID.randomUUID();
        out.println("Loaded args, setting up run: " + runUuid.toString());
        QrCodeDetector detector = null;
        ServerCommunicator serverCommunicator = null;

        try{
            detector = new QrCodeDetector();
            //1. Try and setup connection with all components (server connection, camera test...)
            try
            {
                serverCommunicator = new ServerCommunicator(Config.URL_SERVERSOCKET);
                //set which camera module to use
                detector.init(camera);
                out.println("Waiting for 5000ms in main thread for camera warm up");
                TimeUnit.SECONDS.sleep(5);
                out.println("Setting up Server Communicator");
                out.println("Saving image for preview using settings: iso:"+Config.ISO + " shutterMs: " + Config.SHUTTERSPEED );

                Utils.saveImageLocally(camera.read(),"preview_"+currentTimeMillis()+".jpg");
            }
            catch (Exception ex)
            {
                err.println("Couldn't setup run "+runUuid.toString()+" because of an error: "+ex.getLocalizedMessage());
                ex.printStackTrace();
                throw ex; // throw so it cleans up run correctly
            }
            out.println("Setup complete, start movement");

            Boolean end = false;
            //2. Start Run
            serverCommunicator.sendStart();

            while(end == false){
                if(Config.USE_CAMERA == Config.CAMERA_ID_PI) Speedometer.getInstance().move();
                var latestValue = detector.detectUniqueCode();
                if(Config.USE_CAMERA == Config.CAMERA_ID_PI) Speedometer.getInstance().stop();
                // process value
                end = (latestValue.getText() != null && latestValue.getText().equals("end"));
                if(Config.DEBUG_MODE){
                    out.println("For Debugging --> saving image locally");
                    Utils.saveImageLocally(latestValue.getImage(),"debug_"+currentTimeMillis()+".jpg");
                }

                if (end == false) {
                    //if its not the end it must be a plant
                    //send to plantId
                    out.println("Preparing to send image to plantId, offlineMode: " +Config.OFFLINE_MODE);
                    var imageBytes = latestValue.getImage();
                    var plantIdResult = PlantIdApiHandler.sendToPlantId(imageBytes);
                    // Send Result to Webserver via socket.io
                    out.println("Sending found plant via socket: " + plantIdResult);
                    serverCommunicator.sendPlant(plantIdResult);
                    }
                }
        }catch (Exception ex){
            err.println("Error occured during run: " + ex.getLocalizedMessage());
            ex.printStackTrace();
        }finally{
            out.println("Ending run!");
            if (detector != null)
            {
                detector.release();
            }
            if (serverCommunicator != null)
            {
                serverCommunicator.sendEnd();
            }
        }
    }
}
