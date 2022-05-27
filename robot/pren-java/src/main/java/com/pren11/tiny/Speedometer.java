package com.pren11.tiny;

import com.pi4j.Pi4J;
import com.pi4j.io.gpio.Gpio;
import com.pi4j.io.gpio.digital.DigitalInput;
import com.pi4j.io.gpio.digital.DigitalOutput;
import com.pi4j.io.gpio.digital.DigitalState;

public class Speedometer {

    int outPin14 = 14;
    int outPin15 = 15;
    int outPin23 = 23;
    int inPin24 = 24;
    int startPin4 = 4;

    DigitalOutput out14;
    DigitalOutput out15;
    DigitalOutput out23;
    DigitalInput in24;
    DigitalInput in4;


    static Speedometer instance = null;

    private Speedometer(){
        var pi4j = Pi4J.newAutoContext();
        out14 = pi4j.dout().create(outPin14);
        out15 = pi4j.dout().create(outPin15);
        out23 = pi4j.dout().create(outPin23);
        in24 = pi4j.din().create(inPin24);
        in4 = pi4j.din().create(startPin4);
    }

    public static Speedometer getInstance(){
        if(instance == null){
            instance = new Speedometer();
        }
        return  instance;
    }

    public void fullStop(){
        out14.low();
        out15.low();
        out23.low();
    }

    public void increaseSpeed(){
        if(out14.isLow()){
            out14.high();
        }else if(out15.isLow()){
            out15.high();
        }else if(out23.isLow()){
            out23.high();
        }
    }

    public void decreaseSpeed(){
        if(out23.isLow()){
            out23.low();
        }else if(out15.isHigh()){
            out15.low();
        }else if(out14.isHigh()){
            out14.low();
        }
    }

}
