package com.pren11.camera;

import com.pren11.Config;
import uk.co.caprica.picam.ByteArrayPictureCaptureHandler;
import uk.co.caprica.picam.Camera;
import uk.co.caprica.picam.CameraConfiguration;
import uk.co.caprica.picam.CaptureFailedException;
import uk.co.caprica.picam.enums.Encoding;

import static uk.co.caprica.picam.PicamNativeLibrary.installTempLibrary;

public class PiCameraModule implements CameraModule{

    Camera cam;

    @Override
    public void init() {
        // Extract the bundled picam native library to a temporary file and load it
        try{
            installTempLibrary();
        }catch (Exception ex){
            System.err.println("Failed to install picam native library");
            ex.printStackTrace();
        }
        var config = CameraConfiguration.cameraConfiguration()
                .width(Config.PICTURE_WIDTH)
                .height(Config.PICTURE_HEIGHT)
                .encoding(Encoding.JPEG)
                .iso(Config.ISO)
                .shutterSpeed(Config.SHUTTERSPEED)
                .quality(80);
        try {
            cam = new Camera(config);
            cam.open();
        }catch (Exception ex){
            ex.printStackTrace();
        }

    }

    @Override
    public byte[] read() {
        try {
            return cam.takePicture(new ByteArrayPictureCaptureHandler());
        } catch (CaptureFailedException e) {
            e.printStackTrace();
        }
        return null;
    }

    @Override
    public void release() {
        if(cam != null){
            cam.close();
            cam = null;
        }
    }
}
