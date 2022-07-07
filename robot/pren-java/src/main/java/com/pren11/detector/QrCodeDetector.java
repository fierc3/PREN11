package com.pren11.detector;

import boofcv.alg.fiducial.qrcode.QrCode;
import boofcv.factory.fiducial.ConfigQrCode;
import boofcv.factory.fiducial.FactoryFiducial;
import boofcv.io.image.ConvertBufferedImage;
import boofcv.struct.image.GrayU8;
import com.google.zxing.BinaryBitmap;
import com.google.zxing.EncodeHintType;
import com.google.zxing.ResultPoint;
import com.google.zxing.client.j2se.BufferedImageLuminanceSource;
import com.google.zxing.common.HybridBinarizer;
import com.google.zxing.qrcode.decoder.ErrorCorrectionLevel;
import com.google.zxing.qrcode.detector.Detector;
import com.pren11.Config;
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
        var bytes = cam.read();
        try {
            return Utils.toBufferedImage(bytes);
        } catch (IOException e) {
            e.printStackTrace();
        }
        return null;
    }

    private String tryReadCode(BufferedImage image){
        try{
            GrayU8 gray = ConvertBufferedImage.convertFrom(image, (GrayU8)null);
            var config = new ConfigQrCode();
            var detector = FactoryFiducial.qrcode(config, GrayU8.class);
            detector.process(gray);

            // Gets a list of all the qr codes it could successfully detect and decode
            var detections = detector.getDetections();
            if(detections.size()>0){
                var msg = detections.get(0).message;;
                System.out.println("Read QR: " + msg);
                return msg;
            }
        }catch (Exception ex){
            System.err.println(ex.getMessage());
        }

        System.out.println("Failed to read qr, returning null");
        return null;
    }

    public static float previousX = 0;

    public QrResult readQrCode(BufferedImage croppedImage, BufferedImage uncroppedImage){
        // Encoding charset
        var result = new QrResult();
        var hintMap   = new HashMap<EncodeHintType, ErrorCorrectionLevel>();

        hintMap.put(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.Q);
        BinaryBitmap binaryBitmap = new BinaryBitmap(new HybridBinarizer(
                new BufferedImageLuminanceSource(croppedImage)));
        try{
            var detect = new Detector(binaryBitmap.getBlackMatrix()).detect();
            var points = detect.getPoints();

            if(points.length > 0){
                ResultPoint point = (ResultPoint) Arrays.stream(points).sorted((x, y) -> Integer.compare((int)x.getX(), (int)y.getX())).toArray()[0];

                //check if this is still the same qr code as before
                if(point.getX() < previousX){
                    previousX = point.getX() + Config.DETECT_X_BUFFER; //add a buffer
                    return null;
                }

                System.out.println("Points on screen: " +points.length);
                System.out.println("previousX: " +previousX);
                System.out.println("previousX: " +previousX);
                System.out.println("first x: " +point.getX());

                var edge = uncroppedImage.getWidth() / 100 * 25;
                var x = (int)point.getX()/100*70;
                var y = uncroppedImage.getHeight();
                //var y = (int)point.getY(); set height manual since we dont know where the plant is
                var width = (int)(point.getX()/100*30)*Config.CROP_WIDTH_MULTIPLIER;
                var croppedPlant = crop(uncroppedImage,x+edge, width, y);
                result.setImage(Utils.toByteArray(croppedPlant,"jpg"));
                previousX = point.getX() + Config.DETECT_X_BUFFER; //add a buffer
                result.setText(tryReadCode(croppedImage));
            }
        }catch (Exception ex){
            //System.err.println(ex.toString());
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
            result = readQrCode(cropped,image);
            cropped.flush();
        }
        return result;
    }
}
