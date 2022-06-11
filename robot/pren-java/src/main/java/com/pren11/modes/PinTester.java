package com.pren11.modes;


import com.pren11.tiny.Speedometer;

import java.util.Arrays;
import java.util.concurrent.TimeUnit;

public class PinTester {
    public void run (){
        System.out.println("Running pin tester");
        Speedometer.getInstance().printStates();
        Speedometer.getInstance().move();
        System.out.println(Speedometer.getInstance().isStartBeingPressed());

    }
}
