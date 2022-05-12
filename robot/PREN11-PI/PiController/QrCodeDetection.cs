using Camera;
using OpenCvSharp;
using PiController;
using System;
using System.Collections.Generic;

namespace QrCodeDetection
{
    public class Detector
    {

        class Code
        {
            public string Value { get; set; }
            public RotatedRect Area { get; set; }
        }

        List<Code> codes = new List<Code>();

        ICameraModule cam;

        public void Init(ICameraModule camera)
        {
            cam = camera;
            cam.Init();
        }
        public void Release()
        {
            cam.Release();
            Cv2.DestroyAllWindows();
        }

        private Mat GrabFrame()
        {
            var bytes = cam.Read();
            //Mat(int rows, int cols, MatType type, Array data
            return Mat.FromImageData(bytes);
        }

        private Mat ConvertGrayScale(Mat image)
        {
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            return gray;
        }

        private Mat CropWidthToCenter(Mat gray)
        {
            var rect = gray.BoundingRect();
            var edge = rect.Width / 100 * 25;
            rect.Width = rect.Width - edge * 2;
            rect.X = rect.X + edge;
            var cropped = new Mat(gray, rect);
            //Utils.SaveImageAsUniqueFile(cropped.ToBytes());
            return cropped;
        }

        private void MarkFeatures(Mat image)
        {
            foreach(var code in codes)
            {
                Cv2.Rectangle(image, code.Area.BoundingRect(), new Scalar(0, 255, 0), thickness: 1);
            }
        }

        /// <summary>
        /// Method <m>Detect</m> searches for unique QR code until found (Sync)
        /// </summary>
        public (string text, Mat image) DetectUniqueCode()
        {
            Mat image = null;
            string finalResult = null;
            Rect? qrRectangle = null;
            while (finalResult == null)
            {
                if(image != null)
                {
                    image.Dispose();
                }
                //Grab the current frame
                image = GrabFrame();
                //Convert to gray scale to improve the image processing
                Mat gray = ConvertGrayScale(image);
                //
                //TODO: Crop between 25% and 50%
                //
                Mat cropped = CropWidthToCenter(gray);
                gray.Dispose();
                var detector = new QRCodeDetector();
                Point2f[] points = null;
                var result = detector.DetectAndDecode(cropped, out points);
                cropped.Dispose();
                if (result.Length > 1)
                {
                    if(codes.FindIndex(x => x.Value.Equals(result)) >= 0){
                        //is a duplicate
                        Console.WriteLine("found dupe result: " + result);
                    }
                    else
                    {
                        Console.WriteLine("result: " + result);
                        qrRectangle = Cv2.BoundingRect(points);
                        //codes.Add(new Code() { Area = rect, Value = result });
                        finalResult = result;
                    }
                }

                if (image.Empty())
                    continue;

               //System.Threading.Thread.Sleep(500);
            }

            if(qrRectangle.HasValue)
            {
               var croppedImage = CropImageForPlant(image, qrRectangle.Value); //TODO: Fix cropping as suggested in docs
                if (image != null)
                {
                    image = croppedImage;
                }
            }
            
            //image.Dispose();

            return (text: finalResult, image: image);
        }

        private Mat CropImageForPlant(Mat image, Rect boundRect)
        {
            Utils.SaveImageAsUniqueFile(image.ToBytes());
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            var iRect = gray.BoundingRect();
            iRect.Width = boundRect.Width * 2;
            iRect.X = (image.Width / 100 * 25) +boundRect.X - (boundRect.Width/2);

            var cropped = new Mat(image, iRect);
            //image.Dispose();
            Utils.SaveImageAsUniqueFile(cropped.ToBytes());
            return cropped;
        }
    }
}
