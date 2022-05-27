package com.pren11.camera;

import uk.co.caprica.picam.ByteArrayPictureCaptureHandler;
import uk.co.caprica.picam.Camera;
import uk.co.caprica.picam.CameraConfiguration;
import uk.co.caprica.picam.CaptureFailedException;
import uk.co.caprica.picam.enums.Encoding;

public class PiCameraModule implements CameraModule{

    Camera cam;

    @Override
    public void init() {
        var config = CameraConfiguration.cameraConfiguration()
                .width(1920)
                .height(1080)
                .encoding(Encoding.JPEG)
                .iso(100)
                .shutterSpeed(1000)
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
