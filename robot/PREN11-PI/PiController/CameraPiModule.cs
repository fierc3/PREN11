using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Handlers;
using MMALSharp.Common.Utility;
using MMALSharp.Native;
using MMALSharp.Components;
using MMALSharp.Ports;
using System.Threading;

namespace Camera
{
    public class CameraPiModule : ICameraModule
    {
        MMALCamera cam;
        CustomInMemoryCaptureHandler imgCaptureHandler = new CustomInMemoryCaptureHandler();

        class CustomInMemoryCaptureHandler : InMemoryCaptureHandler
        {
            public byte[] lastImage = new byte[0];
            override public void PostProcess()
            {
                Console.WriteLine(WorkingData.Count());
                lastImage = this.WorkingData.ToArray();
                this.WorkingData.Clear();
            }
        }

        public void Init()
        {
            MMALCameraConfig.StillResolution = new Resolution(640, 480); // Set to 640 x 480. Default is 1280 x 720.
            MMALCameraConfig.StillFramerate = new MMAL_RATIONAL_T(20, 1); // Set to 20fps. Default is 30fps.
            MMALCameraConfig.ShutterSpeed = 200000; //2000000; // Set to 2s exposure time. Default is 0 (auto).
            MMALCameraConfig.ISO = 400;
            MMALCameraConfig.StillBurstMode = true;

            cam = MMALCamera.Instance;
        }

        public byte[] Read()
        {

            using (var imgCaptureHandler = new InMemoryCaptureHandler())
            {
                cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420).Wait();
                return imgCaptureHandler.WorkingData.ToArray();
            }
        }

        public void Release()
        {
            cam.Cleanup();
        }
    }
}
