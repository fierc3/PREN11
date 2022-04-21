using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraTEst
{
    class FaceFeatureDetection
    {

        class FaceFeature
        {
            public Rect Face { get; set; }
            //public Rect[] Eyes { get; set; }
        }

        List<FaceFeature> features = new List<FaceFeature>();

        VideoCapture videoCapture;
        CascadeClassifier face_cascade;
        CascadeClassifier eyes_cascade;

        public void Init()
        {
            //Initialise the video capture module
            videoCapture = new VideoCapture(1);
            videoCapture.Set(3, 640); //Set the frame width
            videoCapture.Set(4, 480); //Set the frame height

            //Define the face and eyes classifies using Haar-cascade xml
            //Download location: https://github.com/opencv/opencv/tree/master/data/haarcascades
            face_cascade = new CascadeClassifier("C:/Users/MikePullen/source/repos/CameraTEst/CameraTEst/haarcascades/haarcascade_frontalface_default.xml");
        }
        public void Release()
        {
            videoCapture.Release();
            Cv2.DestroyAllWindows();
        }

        private Mat GrabFrame()
        {
            Mat image = new Mat();
            //Capture frame by frame
            videoCapture.Read(image);
            return image;
        }

        private Mat ConvertGrayScale(Mat image)
        {
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            return gray;
        }

        private Rect[] DetectFaces(Mat image)
        {
            Rect[] faces = face_cascade.DetectMultiScale(image, 1.3, 5);
            return faces;
        }

        private void MarkFeatures(Mat image)
        {
            foreach (FaceFeature feature in features)
            {
                Cv2.Rectangle(image, feature.Face, new Scalar(0, 255, 0), thickness: 1);
            }
        }

        public void DetectFeatures()
        {
            Mat image;
            while (true)
            {
                //Grab the current frame
                image = GrabFrame();
                //Convert to gray scale to improve the image processing
                Mat gray = ConvertGrayScale(image);

                //Detect faces using Cascase classifier
                Rect[] faces = DetectFaces(gray);
                if (image.Empty())
                    continue;
                //Loop through detected faces
                foreach (var item in faces)
                {
                    //Get the region of interest where you can find facial features
                    Mat face_roi = gray[item];

                    //Record the facial features in a list
                    features.Add(new FaceFeature()
                    {
                        Face = item,
                    });
                }
                //Mark the detected feature on the original frame
                MarkFeatures(image);
                Cv2.ImShow("frame", image);
                if (Cv2.WaitKey(1) == (int)ConsoleKey.Enter)
                    break;
            }
        }
    }
}
