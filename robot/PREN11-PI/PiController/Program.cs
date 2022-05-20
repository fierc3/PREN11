using System;
using System.Collections.Generic;
using System.IO;
using Camera;
using OpenCvSharp;
using QrCodeDetection;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Device.Gpio;
using System.Threading;

namespace PiController
{

    class Program
    {
        private static void RunRecordingMode(string[] args)
        {
            int delay = 0;
            int imageCount = 100;
            int input = 0;

            if (args.Length > 1)
                int.TryParse(args[1], out input);
            if (args.Length > 2)
                int.TryParse(args[2], out delay);
            if (args.Length > 3)
                int.TryParse(args[3], out imageCount);

            Console.WriteLine("Running Record Mode");
            ICameraModule camera = input == Config.CAMERA_ID_USB ? new WebCameraModule() : new CameraPiModule();
            camera.Init();
            Console.WriteLine("Images will be saved under: " + Path.GetFullPath("run-xxxxx.jgp"));
            Console.WriteLine("Waiting for 5000ms in main thread for camera warm up");
            Task.Delay(5000).Wait();
            Console.WriteLine($"Delay Between Images:{delay}ms, Capture count: {imageCount}");
            for(int i = 0; i < imageCount; i++)
            {
                Console.WriteLine("Starting Image " + i);
                var bytes = camera.Read();
                Console.WriteLine("Read Image " + i);
                Utils.SaveImageAsUniqueFile(bytes);
                Task.Delay(delay).Wait();
            }
            Console.WriteLine("Finishing Record Mode");

        }


        private static void RunParcour(string[] args)
        {
            int input = 0;
            int offline = 0;

            if (args.Length > 1)
                int.TryParse(args[1], out input);
            if (args.Length > 2)
                int.TryParse(args[2], out offline);
            ICameraModule camera = input == Config.CAMERA_ID_USB ? new WebCameraModule() : new CameraPiModule();

            Console.WriteLine("++++++ PREN11 PiController is starting ++++++");
            Console.WriteLine("Using args: " + String.Join(", ", args));

            var localGuid = Guid.NewGuid();
            Console.WriteLine("Loaded args, setting up run: " + localGuid.ToString());
            Detector detector = null;
            ServerCommunicator serverCommunicator = null;
            try
            {
                detector = new QrCodeDetection.Detector();
                //1. Try and setup connection with all components (server connection, camera test...)
                try
                {
                    if (offline == 1)
                    {
                        serverCommunicator = new ServerCommunicator(Config.URL_SERVERSOCKET);
                    }
                    //set which camera module to use
                    detector.Init(camera);
                    Console.WriteLine("Waiting for 5000ms in main thread for camera warm up");
                    Task.Delay(5000).Wait();
                    Console.WriteLine("Setting up Server Communicator");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Couldn't setup run {localGuid} because of an error: {ex.Message}");
                    Console.Error.WriteLine(ex.StackTrace.ToString());
                    throw ex; // throw so it cleans up run correctly
                }
                Boolean end = false;
                //2. Start Run
                if (offline == 1)
                {
                    serverCommunicator.SendStart();
                }

                while (end == false)
                {
                    var latestValue = detector.DetectUniqueCode();
                    // process value
                    end = (latestValue.text.Equals("end"));
                    if(offline == 0) //is offline true? 
                    {
                        Console.WriteLine("Offline --> saving image locally");
                        Utils.SaveImageAsUniqueFile(latestValue.image.ToBytes());
                    }
                    if (end == false && latestValue.text.Contains("plant"))
                    {
                        //send to plantId
                        if (offline == 1) //Online Mode check
                        {
                            Console.WriteLine("Running in Online Mode, preparing to send image to plantId");
                            var imageBytes = latestValue.image.ToBytes();
                            var plantIdResult = PlantIdApiHandler.SendToPlantId(imageBytes);
                            // Send Result to Webserver via socket.io
                            Console.WriteLine("Sending found plant via socket: " + plantIdResult);
                            serverCommunicator.SendPlant(plantIdResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Run {localGuid} ended because of an error: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace.ToString());
                //we could display these errors somewhere
            }
            finally
            {
                Console.WriteLine("Ending Run");
                if (detector != null)
                {
                    detector.Release();
                }
                if (serverCommunicator != null)
                {
                    serverCommunicator.SendEnd();
                }
                Console.ReadKey(); //for debug purpose
                //end run
            }
        }

        public static void RunTestPinMode()
        {
            Console.WriteLine("Starting test code");
            //14 15 23 24
            int outPin14 = 14;
            int outPin15 = 15;
            int outPin23 = 23;
            int inPin24 = 24;
            using var controller = new GpioController();
            controller.OpenPin(outPin14, PinMode.Output);
            controller.OpenPin(outPin15, PinMode.Output);
            controller.OpenPin(outPin23, PinMode.Output);
            controller.OpenPin(inPin24, PinMode.Input);
            bool pinValue = true;
            while (true)
            {

                controller.Write(outPin14, ((pinValue) ? PinValue.High : PinValue.Low));
                controller.Write(outPin15, ((pinValue) ? PinValue.High : PinValue.Low));
                controller.Write(outPin23, ((pinValue) ? PinValue.High : PinValue.Low));
                Console.WriteLine($"Switched pins {outPin14}, {outPin15}, {outPin23}, to {pinValue}");
                Console.WriteLine($"pins {inPin24}, {inPin24}, {outPin23}, to {pinValue}");
                Console.WriteLine(controller.Read(inPin24));
                Thread.Sleep(1000);

                pinValue = !pinValue;
            }
        }

        static void Main(string[] args)
        {
            int program = 0;
            if (args.Length > 0)
                int.TryParse(args[0], out program);
            if(program == 0)
            {
                RunRecordingMode(args);
            }else if(program == 1)
            {
                RunParcour(args);
            }else if (program == 2)
            {
                RunTestPinMode();
            }
            
        }
    }
}
