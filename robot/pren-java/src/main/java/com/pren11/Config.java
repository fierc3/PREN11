package com.pren11;

import java.io.FileInputStream;
import java.util.Properties;

public class Config {
    public static String URL_PLANTID = "https://api.plant.id/v2/identify";
    public static String URL_SERVERSOCKET = "https://tactile-rigging-333212.oa.r.appspot.com";
    public static int SHUTTERSPEED = 20000;
    public static int ISO = 100;
    public static int CAMERA_ID_PI = 0;
    public static int CAMERA_ID_USB = 1;
    public static int USE_CAMERA = CAMERA_ID_PI;
    public static boolean OFFLINE_MODE = true;
    public static boolean DEBUG_MODE = true;
    public static int PICTURE_WIDTH = 1920;
    public static int PICTURE_HEIGHT = 1080;
    public static int CROP_WIDTH_MULTIPLIER = 4;
    public static int CROP_Y = 150;
    public static int DETECT_X_BUFFER = 200;

    public static void updateByRelativeFile(){
        Properties prop = new Properties();
        String fileName = "pren.config";
        try (FileInputStream fis = new FileInputStream(fileName)) {
            prop.load(fis);
            ISO = Integer.parseInt(prop.getProperty("iso"));
            SHUTTERSPEED = Integer.parseInt(prop.getProperty("shutterspeed"));
            OFFLINE_MODE = prop.getProperty("offline").toString().equals("true");
            DEBUG_MODE = prop.getProperty("debug").toString().equals("true");
            CROP_WIDTH_MULTIPLIER = Integer.parseInt(prop.getProperty("cropwidthmulti"));
            CROP_Y = Integer.parseInt(prop.getProperty("cropy"));
            DETECT_X_BUFFER = Integer.parseInt(prop.getProperty("xbuffer"));
        } catch (Exception ex) {
         System.err.println(("Failed to load config " + ex));
         ex.printStackTrace();
        }
        System.out.println("Updated config by file: ");
        System.out.println("iso: " + ISO);
        System.out.println("SHUTTERSPEED: " + SHUTTERSPEED);
        System.out.println("OFFLINE_MODE: " + OFFLINE_MODE);
        System.out.println("DEBUG_MODE: " + DEBUG_MODE);
        System.out.println("CROP_WIDTH_MULTIPLIER: " + CROP_WIDTH_MULTIPLIER);
        System.out.println("CROP_Y: " + CROP_WIDTH_MULTIPLIER);
        System.out.println("DETECT_X_BUFFER: " + CROP_WIDTH_MULTIPLIER);
        System.out.println("------------------------------------------");
    }
}
