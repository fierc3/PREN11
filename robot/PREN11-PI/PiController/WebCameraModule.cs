using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera
{
    class WebCameraModule : ICameraModule
    {
        VideoCapture videoCapture;
        public void Init()
        {
            //Initialise the video capture module
            videoCapture = new VideoCapture(0);

             videoCapture.Set(3, videoCapture.FrameWidth / 2); //Set the frame width
             videoCapture.Set(4, videoCapture.FrameHeight / 2); //Set the frame height
           
        }

        public bool Read(Mat image)
        {
            return videoCapture.Read(image);
        }

        public void Release()
        {
            videoCapture.Release();
        }
    }
}
