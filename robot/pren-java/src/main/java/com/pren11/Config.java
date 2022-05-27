package com.pren11;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
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

    public static void updateByRelativeFile(){
        Properties prop = new Properties();
        String fileName = "pren.config";
        try (FileInputStream fis = new FileInputStream(fileName)) {
            prop.load(fis);
            ISO = Integer.parseInt(prop.getProperty("iso"));
            SHUTTERSPEED = Integer.parseInt(prop.getProperty("shutterspeed"));
            OFFLINE_MODE = prop.getProperty("offline").toString().equals("true");
        } catch (Exception ex) {
         System.err.println(("Failed to load config " + ex));
         ex.printStackTrace();
        }
        System.out.println("Updated config by file: iso: " + ISO + ", shutterspeed: " + SHUTTERSPEED + ", offline: " +OFFLINE_MODE);
    }
}
