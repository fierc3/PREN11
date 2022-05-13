using Camera;
using OpenCvSharp;
using PiController;
using System;

namespace QrCodeDetection
{
    public class Detector
    {

        class Code
        {
            public string Value { get; set; }
            public RotatedRect Area { get; set; }
        }

        int previousX = 0;

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
            return cropped;
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
                //Crop image to 25% - 75%, center focused
                Mat cropped = CropWidthToCenter(gray);
                gray.Dispose();
                var detector = new QRCodeDetector();
                Point2f[] points = null;
                var result = detector.DetectAndDecode(cropped, out points);
                cropped.Dispose();
                if (result.Length > 1)
                {
                    var currentX = Cv2.BoundingRect(points).X;
                    if (previousX > currentX){
                        //is a duplicate
                        Console.WriteLine("found suspected dupe, because previous x > current x: " + result);
                    }
                    else
                    {
                        Console.WriteLine("result: " + result);
                        qrRectangle = Cv2.BoundingRect(points);
                        finalResult = result;
                    }
                    previousX = currentX;
                }

                if (image.Empty())
                    continue;
            }

            if(qrRectangle.HasValue)
            {
                //Try to crop around plant
               var croppedImage = CropImageForPlant(image, qrRectangle.Value);
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
            try
            {
                Utils.SaveImageAsUniqueFile(image.ToBytes());
                Mat gray = new Mat();
                Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

                var iRect = gray.BoundingRect();
                iRect.Width = boundRect.Width * 2;
                iRect.X = (image.Width / 100 * 25) + boundRect.X - (boundRect.Width / 2);

                var cropped = new Mat(image, iRect);
                //image.Dispose();
                Utils.SaveImageAsUniqueFile(cropped.ToBytes());
                return cropped;
            }
            catch (Exception ex) {
                Console.WriteLine("Failed to crop: " + ex.Message);
                return image;
            }

        }
    }
}
