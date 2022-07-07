package com.pren11;

import com.pren11.camera.CameraModule;
import com.pren11.camera.PiCameraModule;
import com.pren11.camera.WebCameraModule;
import com.pren11.modes.Parcour;
import com.pren11.modes.PinTester;
import com.pren11.modes.Run;
import com.pren11.tiny.Speedometer;

import java.io.FileOutputStream;
import java.io.PrintStream;
import java.util.concurrent.TimeUnit;

import static java.lang.System.currentTimeMillis;

public class Program {


    public static void main(String[] args) throws Exception {
        if(Config.USE_CAMERA == Config.CAMERA_ID_PI){
            System.setOut(new PrintStream(new FileOutputStream("pren11-out.txt")));
            System.setErr(new PrintStream(new FileOutputStream("pren11-err.txt")));
            System.out.println("Waiting for start to be pressed");
            Speedometer.getInstance().stop();
            boolean startPressed = false;
            while(startPressed == false){
                startPressed = Speedometer.getInstance().isStartBeingPressed();
                TimeUnit.SECONDS.sleep(1);
                Speedometer.getInstance().printStates();
            }
            System.out.println("Start has been pressed, its go time baby");
        }

        Config.updateByRelativeFile();
        Run mode = new Parcour();
        mode.run(args);
    }



}