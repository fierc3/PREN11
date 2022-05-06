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
        MyInMemoryCaptureHandler imgCaptureHandler = new MyInMemoryCaptureHandler();

        public class MyInMemoryCaptureHandler : InMemoryCaptureHandler
        {
            public static byte[] lastImage = new byte[0];
            private static object syncObj = new object();

            public byte[] GetLastImage()
            {
                Console.WriteLine(lastImage.Length);
                return lastImage;
            }

            public override void Process(ImageContext ctx)
            {
                // The InMemoryCaptureHandler parent class has a property called "WorkingData". 
                // It is your responsibility to look after the clearing of this property.

                // The "eos" parameter indicates whether the MMAL buffer has an EOS parameter, if so, the data that's currently
                // stored in the "WorkingData" property plus the data found in the "data" parameter indicates you have a full image frame.

                // The call to base.Process will add the data to the WorkingData list.
                WorkingData.AddRange(ctx.Data);
                base.Process(ctx);

                if (ctx.Eos)
                {

                    lock (syncObj)
                    {
                        lastImage = this.WorkingData.ToArray();

                        Console.WriteLine("I have a full frame. Clearing working data.");
                        this.WorkingData.Clear();
                    }
                }
            }
        }

        public void Init()
        {
            MMALCameraConfig.StillResolution = new Resolution(640, 480); // Set to 640 x 480. Default is 1280 x 720.
            //MMALCameraConfig.VideoResolution = new Resolution(640, 480); // Set to 640 x 480. Default is 1280 x 720.
            //MMALCameraConfig.VideoStabilisation = true;

            MMALCameraConfig.StillFramerate = new MMAL_RATIONAL_T(20, 1); // Set to 20fps. Default is 30fps.
            MMALCameraConfig.ShutterSpeed = 200000; //2000000; // Set to 2s exposure time. Default is 0 (auto).
            MMALCameraConfig.ISO = 400;
            MMALCameraConfig.StillBurstMode = true;

            cam = MMALCamera.Instance;
        }

        public byte[] Read()
        {
            using (var splitter = new MMALSplitterComponent())
            using (var imgEncoder = new MMALImageEncoder(continuousCapture: true))
            using (var nullSink = new MMALNullSinkComponent())
            {
                cam.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, quality: 50);


                // Create our component pipeline.         
                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                cam.Camera.VideoPort.ConnectTo(splitter);
                splitter.Outputs[0].ConnectTo(imgEncoder);
                cam.Camera.PreviewPort.ConnectTo(nullSink);

                //Task.Delay(2000).Wait();
                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));

                cam.ProcessAsync(cam.Camera.VideoPort, cts.Token).Wait();
                return imgCaptureHandler.GetLastImage();
            }
        }

        public void Release()
        {
            cam.Cleanup();
        }
    }
}
