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

namespace Camera
{
    public class CameraPiModule : ICameraModule
    {

        MMALCamera cam;
        public void Init()
        {
            // Singleton initialized lazily. Reference once in your application.
            MMALCamera cam = MMALCamera.Instance;
            MMALCameraConfig.StillResolution = new Resolution(640, 480); // Set to 640 x 480. Default is 1280 x 720.
            MMALCameraConfig.StillFramerate = new MMAL_RATIONAL_T(20, 1); // Set to 20fps. Default is 30fps.
            MMALCameraConfig.ShutterSpeed = 2000000; // Set to 2s exposure time. Default is 0 (auto).
            MMALCameraConfig.ISO = 400;
        }

        public byte[] Read()
        {
            using (var imgCaptureHandler = new InMemoryCaptureHandler())
            {
                cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420).Wait();
                return imgCaptureHandler.WorkingData.ToArray();
            }
            return null;
        }

        public void Release()
        {
            cam.Cleanup();
        }
    }
}
