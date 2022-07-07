package com.pren11.server;

public class PlantIdResult {

    public PlantIdResult(String name, String image, double p){
        setPlantName(name);
        setImageUrl(image);
        setProbability(p);
    }
    private String plantName;

    public String getPlantName() {
        return plantName;
    }

    public void setPlantName(String v) {
        this.plantName = v;
    }

    private String imageUrl;

    public String getImageUrl() {
        return imageUrl;
    }

    public void setImageUrl(String url) {
        this.imageUrl = url;
    }

    private double probability;

    public double getProbability() {
        return probability;
    }

    public void setProbability(double v) {
        this.probability = v;
    }
}
