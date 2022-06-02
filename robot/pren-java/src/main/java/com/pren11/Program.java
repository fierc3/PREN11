package com.pren11;

import com.pren11.modes.Parcour;
import com.pren11.modes.Run;
import java.io.FileOutputStream;
import java.io.PrintStream;

public class Program {


    public static void main(String[] args) throws Exception {
        //System.setOut(new PrintStream(new FileOutputStream("pren11-out.txt")));
        //System.setErr(new PrintStream(new FileOutputStream("pren11-err.txt")));
        Config.updateByRelativeFile();
        Run mode = new Parcour();
        mode.run(args);
    }

}