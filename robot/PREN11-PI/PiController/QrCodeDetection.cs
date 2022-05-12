using Camera;
using OpenCvSharp;
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
                var detector = new QRCodeDetector();
                Point2f[] points = null;
                var result = detector.DetectAndDecode(gray, out points);
                Console.WriteLine(result);
                gray.Dispose();
                if (result.Length > 1)
                {
                    if(codes.FindIndex(x => x.Value.Equals(result)) >= 0){
                        //is a duplicate
                        Console.WriteLine("found dupe result: " + result);
                    }
                    else
                    {
                        Console.WriteLine("result: " + result);
                        var rect = Cv2.MinAreaRect(points);
                        codes.Add(new Code() { Area = rect, Value = result });
                        finalResult = result;
                    }
                }

                if (image.Empty())
                    continue;

               //System.Threading.Thread.Sleep(500);
            }

            var croppedImage = CropImageForPlant(image); //TODO: Fix cropping as suggested in docs
            //image.Dispose();

            //display result
            if (image != null)
            {
                image = croppedImage;
            }
            return (text: finalResult, image: image);
        }

        private Mat CropImageForPlant(Mat image)
        {
            var boundRect = codes[codes.Count - 1].Area.BoundingRect();
            var widthCropWithSpace = boundRect.Width * 2;
            if(widthCropWithSpace < image.Width)
            {
                Console.WriteLine("Attempting to crop width");
                boundRect.Width = widthCropWithSpace;
                var moveToLeft = boundRect.Width / 2;
                var moveToX = boundRect.X - moveToLeft;
                boundRect.X = moveToX > 0 ? moveToX : 0;
            }
            else
            {
                boundRect.Width = image.Width;
                boundRect.X = 0;
            }
            Console.WriteLine(image.Height);
            Console.WriteLine(boundRect.Y);
            Console.WriteLine("Cropping image to max height");
            boundRect.Y = 0;
            boundRect.Height = image.Height;
            try
            {
              return new Mat(image, boundRect);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't crop, will continue uncropped: " + ex.Message);
                return image;
            }
            
        }
    }
}
