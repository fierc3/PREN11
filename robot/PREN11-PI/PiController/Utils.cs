using System;
using System.IO;

namespace PiController
{
    public class Utils
    {
        public static void SaveImageAsUniqueFile(byte[] bytes)
        {
            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            File.WriteAllBytesAsync("run-" + milliseconds + ".jpg", bytes);
        }
    }
}
