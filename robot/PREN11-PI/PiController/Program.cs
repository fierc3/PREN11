using System;
using System.Collections.Generic;
using System.IO;
using Camera;
using OpenCvSharp;
using QrCodeDetection;
using System.Threading;

namespace PiController
{

    class Program
    {
        static int visualize = 1; //1 = visualize, 2 = dont visualize
        static int mode = 1; //1 = offline, 2 = online
        static ICameraModule camera = new CameraPiModule();

        private static void loadFlags(string[] args)
        {
            if (args.Length > 0)
                int.TryParse(args[0], out visualize);
            if (args.Length > 1)
                int.TryParse(args[1], out mode);
        }

        private static void runRecordingMode(string[] args)
        {
            int delay = 0;
            int max = 100;
            int input = 0;

            if (args.Length > 0)
                int.TryParse(args[0], out delay);
            if (args.Length > 1)
                int.TryParse(args[1], out max);
            if (args.Length > 2)
                int.TryParse(args[2], out input);

            Console.WriteLine("Running Record Mode");
            ICameraModule camera = input == 0 ? new WebCameraModule() : new CameraPiModule();
            camera.Init();
            Console.WriteLine("Images will be saved under: " + Path.GetFullPath("run-xxxxx.jgp"));
            Console.WriteLine($"Delay Between Images:{delay}ms, Capture count: {max}");
            for(int i = 0; i < max; i++)
            {
                Console.WriteLine("Saving Image " + i);
                var bytes = camera.Read();
                long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                File.WriteAllBytes("run-" + milliseconds + ".jpg", bytes);
                Thread.Sleep(2000);

            }
            Console.WriteLine("Finishing Record Mode");

        }
        private static void runParcour(string[] args)
        {

            Console.WriteLine("++++++ PREN11 PiController is starting ++++++");
            Console.WriteLine("Using args: " + String.Join(", ", args));

            loadFlags(args);
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
                    if (mode == 2)
                    {
                        serverCommunicator = new ServerCommunicator("https://tactile-rigging-333212.oa.r.appspot.com");
                    }
                    //set if should visualize  and which camera module to use
                    detector.Init(visualize == 1, camera);
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
                if (mode == 2)
                {
                    serverCommunicator.SendStart();
                }

                while (end == false)
                {
                    var latestValue = detector.DetectUniqueCode();
                    // process value
                    end = (latestValue.text.Equals("end"));
                    if (end == false && latestValue.text.Contains("plant"))
                    {
                        //send to plantId
                        if (mode == 2) //Online Mode check
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

        static void Main(string[] args)
        {
            runRecordingMode(args);
        }
    }
}
