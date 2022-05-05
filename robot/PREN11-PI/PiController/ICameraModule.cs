using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera
{
    public interface ICameraModule
    {
        public void Init();
        //public Mat Capture(bool save);

        public byte[] Read();
        //public void ShowImage(Mat image);
        public void Release();
      
}
}
