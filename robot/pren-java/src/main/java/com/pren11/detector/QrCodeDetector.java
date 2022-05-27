package com.pren11.detector;

import com.google.zxing.BinaryBitmap;
import com.google.zxing.EncodeHintType;
import com.google.zxing.ResultPoint;
import com.google.zxing.client.j2se.BufferedImageLuminanceSource;
import com.google.zxing.common.HybridBinarizer;
import com.google.zxing.qrcode.decoder.ErrorCorrectionLevel;
import com.google.zxing.qrcode.detector.Detector;
import com.pren11.Utils;
import com.pren11.camera.CameraModule;

import java.awt.image.BufferedImage;
import java.io.IOException;
import java.util.Arrays;
import java.util.HashMap;

import static com.pren11.Utils.crop;
import static com.pren11.Utils.displayImage;

public class QrCodeDetector {
    CameraModule cam;

    public void init(CameraModule camera){
        cam = camera;
        cam.init();
    }

    public void release()
    {
        cam.release();
    }

    private BufferedImage grabFrame(){
        System.out.println("pre cam read");
        var bytes = cam.read();
        System.out.println("post cam read");
        try {
            return Utils.toBufferedImage(bytes);
        } catch (IOException e) {
            e.printStackTrace();
        }
        return null;
    }

    public QrResult readQrCode(BufferedImage bufferedImage){
        // Encoding charset
        var result = new QrResult();
        var hintMap   = new HashMap<EncodeHintType, ErrorCorrectionLevel>();


        hintMap.put(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.Q);
        BinaryBitmap binaryBitmap = new BinaryBitmap(new HybridBinarizer(
                new BufferedImageLuminanceSource(bufferedImage)));
        try{
            var detect = new Detector(binaryBitmap.getBlackMatrix()).detect();
            var points = detect.getPoints();
            System.out.println(points.length);
            if(points.length > 0){
                ResultPoint point = (ResultPoint) Arrays.stream(points).sorted((x, y) -> Integer.compare((int)x.getX(), (int)y.getX())).toArray()[0];
                var x = (int)point.getX()/100*70;
                var y = (int)point.getY();
                var width = (int)(point.getX()/100*30)*4;
                var croppedImage = crop(bufferedImage,x, width, y);
                result.setImage(Utils.toByteArray(croppedImage,"jpg"));
            }
        }catch (Exception ex){
            System.err.println(ex.toString());
        }
        return result;
    }

    public QrResult detectUniqueCode()
    {
        BufferedImage image = null;
        String finalText = null;
        QrResult result = null;
        while (result == null || result.getImage() == null){
            if(image != null)
            {
                image.flush();
            }
            //Grab the current frame
            image = grabFrame();
            //Crop image to 25% - 75%, center focused
            var cropped = Utils.cropWidthToCenter(image);
            result = readQrCode(cropped);
            cropped.flush();
        }
        return result;
    }
}
