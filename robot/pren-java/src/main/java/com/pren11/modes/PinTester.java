package com.pren11.modes;

import com.pi4j.Pi4J;
import com.pi4j.io.gpio.digital.DigitalInput;
import com.pi4j.io.gpio.digital.DigitalOutput;

import java.util.Arrays;
import java.util.concurrent.TimeUnit;

public class PinTester {
    public void run (){
        System.out.println("Running pin tester");
        var pi4j = Pi4J.newAutoContext();
        var out14 = pi4j.dout().create(14);
        var out15 = pi4j.dout().create(15);
        var out23 = pi4j.dout().create(23);
        var in24 = pi4j.din().create(24);
        var in4 = pi4j.din().create(4);
        var in7 = pi4j.din().create(7);
        var allOut = new DigitalOutput[]{out14,out15,out23};
        var allIn = new DigitalInput[]{in24,in4,in7};

        System.out.println("Testing Pi4J.newAutoContext()---------------------------------");
        for(int i = 0; i <5 ; i++){
            try {
            Arrays.stream(allOut).forEach(x ->{
                System.out.println(x.getName()+" isHigh: "+ x.isHigh() + " - on: " +x.isOn());
            } );
            Arrays.stream(allIn).forEach(x ->{
                System.out.println(x.getName()+" isHigh: "+ x.isHigh() + " - on: " +x.isOn());
            } );
            System.out.println("++++++++++++++++++++++++++++++"+i+"++++++++++++++++++++++++++++++++++");

                TimeUnit.SECONDS.sleep(1);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }



    }
}
