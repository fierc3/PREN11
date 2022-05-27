package com.pren11.camera;

import com.github.sarxos.webcam.Webcam;
import com.github.sarxos.webcam.WebcamResolution;
import com.pren11.Utils;

import java.awt.*;

public class WebCameraModule  implements CameraModule{

    static Webcam webcam = null;

    @Override
    public void init() {
        if(webcam == null){
            Dimension[] nonStandardResolutions = new Dimension[] {
                    WebcamResolution.HD720.getSize(),
            };
            webcam = Webcam.getDefault();
            webcam.setCustomViewSizes(nonStandardResolutions);
            webcam.setViewSize(WebcamResolution.HD720.getSize());
            webcam.open();
        }
    }

    @Override
    public byte[] read() {
        try{
            return Utils.toByteArray(webcam.getImage(), "jpg");
        }catch (Exception ex){
            ex.printStackTrace();
        }
        return null;

    }

    @Override
    public void release() {
        if(webcam == null) return;
        webcam.close();
        webcam = null;
    }
}
