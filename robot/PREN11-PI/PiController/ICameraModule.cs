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

        public bool Read( Mat image);
        //public void ShowImage(Mat image);
        public void Release();
      
}
}
