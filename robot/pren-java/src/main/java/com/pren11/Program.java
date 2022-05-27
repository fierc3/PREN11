package com.pren11;


import com.pren11.detector.QrCodeDetector;
import com.pren11.modes.Parcour;
import com.pren11.modes.Run;
import com.pren11.server.PlantIdApiHandler;

import javax.imageio.ImageIO;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;

public class Program {


    public static void main(String[] args) throws Exception {
        Run mode = new Parcour();
        mode.run(args);
    }

}