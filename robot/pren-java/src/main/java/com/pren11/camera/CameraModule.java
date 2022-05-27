package com.pren11.camera;

public interface CameraModule {
    public void init();
    //public Mat Capture(bool save);

    public byte[] read();
    //public void ShowImage(Mat image);
    public void release();
}
