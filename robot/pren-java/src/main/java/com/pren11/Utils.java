package com.pren11;

import javax.imageio.ImageIO;
import javax.swing.*;
import java.awt.*;
import java.awt.image.BufferedImage;
import java.io.*;

public class Utils {


    public static void saveImageLocally(byte[] image, String path){
        File outputFile = new File(path);
        try (FileOutputStream outputStream = new FileOutputStream(outputFile)) {
            outputStream.write(image);
        }catch(Exception ex){
            ex.printStackTrace();
        }
    }

    public static BufferedImage crop(BufferedImage src, int x, int width, int y)
    {
        return src.getSubimage(x,0,width,y);
    }

    public static BufferedImage cropWidthToCenter(BufferedImage image)
    {

        var edge = image.getWidth() / 100 * 25;
        var width = image.getWidth() - edge * 2;
        return Utils.crop(image, edge,width,image.getHeight() );
    }

    public static byte[] toByteArray(BufferedImage bi, String format)
            throws IOException {

        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        ImageIO.write(bi, format, baos);
        byte[] bytes = baos.toByteArray();
        return bytes;

    }

    // convert byte[] to BufferedImage
    public static BufferedImage toBufferedImage(byte[] bytes)
            throws IOException {

        InputStream is = new ByteArrayInputStream(bytes);
        BufferedImage bi = ImageIO.read(is);
        return bi;

    }

    public static void displayImage(BufferedImage img){
        JFrame frame = new JFrame();
        frame.getContentPane().setLayout(new FlowLayout());
        frame.getContentPane().add(new JLabel(new ImageIcon(img)));
        frame.pack();
        frame.setVisible(true);
    }

}
