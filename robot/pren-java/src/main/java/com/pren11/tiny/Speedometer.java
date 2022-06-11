package com.pren11.tiny;


import com.pi4j.io.gpio.*;

public class Speedometer {

    int outPin14 = 14;
    int outPin15 = 15;
    int outPin23 = 23;
    int inPin24 = 24;
    int startPin4 = 4;
    int startPin7 = 7;
    GpioPinDigitalInput startInput = null;
    GpioPinDigitalOutput moveOutput = null;

    static Speedometer instance = null;

    private Speedometer(){
        System.out.println("<--Pi4J--> GPIO Control Example ... started.");

        // create gpio controller
        final GpioController gpio = GpioFactory.getInstance();

        // provision gpio pin #01 as an output pin and turn on
        moveOutput = gpio.provisionDigitalOutputPin(RaspiPin.GPIO_15, "move", PinState.LOW);
        startInput = gpio.provisionDigitalInputPin(RaspiPin.GPIO_07, "start");

        // set shutdown state for this pin
        moveOutput.setShutdownOptions(true, PinState.LOW);
    }

    public static Speedometer getInstance(){
        if(instance == null){
            instance = new Speedometer();
        }
        return  instance;
    }

    public void fullStop(){
    moveOutput.low();
    }


    public void move() {
        moveOutput.high();
    }

    public void stop() {
        moveOutput.low();
    }

    public void printStates(){
        System.out.println("in4 isHigh: "+ startInput.isHigh());
    }

    public boolean isStartBeingPressed (){
        printStates();
        return startInput.isHigh();
    }

}
