package com.pren11.modes;

import com.pren11.detector.QrCodeDetector;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.*;

public class ImageFileSystem implements Run{
    @Override
    public void run(String[] args) {
        var paths = new String[]{
                "best.jpg",
                "bad.jpg"
        };
        for (var path: paths) {
            System.out.println("Trying read image " + new File(path).getAbsolutePath());
            BufferedImage bufferedImage = null;
            try {
                bufferedImage = ImageIO.read(new FileInputStream(path));
            } catch (IOException e) {
                e.printStackTrace();
            }
            var detector = new QrCodeDetector();
            var result = detector.readQrCode(bufferedImage);
            var image = result.getImage();
            if(image.length > 0){
                File outputFile = new File("cropped_"+path);
                try (FileOutputStream outputStream = new FileOutputStream(outputFile)) {
                    outputStream.write(image);
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        }
    }
}
