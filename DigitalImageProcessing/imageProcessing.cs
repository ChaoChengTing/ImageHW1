using System;

using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;

namespace DigitalImageProcessing
{
    public static class imageProcessing
    {
        public static Image<Bgr, Byte> ConvertToGray(Image<Bgr, Byte> source)  //灰階 function //Gray = R*0.299 + G*0.587 + B*0.114
        {
            Image<Bgr, Byte> result = new Image<Bgr, Byte>(source.Width, source.Height);

            int rows = source.Height;
            int cols = source.Width;

            byte r, g, b;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    b = (byte)source.Data[y, x, 0];  //B
                    g = (byte)source.Data[y, x, 1];  //G
                    r = (byte)source.Data[y, x, 2];  //R

                    byte grayColor = (byte)(b * 0.114 + g * 0.587 + r * 0.299);
                    result.Data[y, x, 0] = grayColor;
                    result.Data[y, x, 1] = grayColor;
                    result.Data[y, x, 2] = grayColor;
                }
            }

            return result;
        }

        public static Image<Bgr, Byte> ConvertToMirror(Image<Bgr, Byte> source)
        {
            Image<Bgr, Byte> result = new Image<Bgr, Byte>(source.Width, source.Height);

            int rows = source.Height;
            int cols = source.Width;

            byte r, g, b;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    b = (byte)source.Data[y, x, 0];  //B
                    g = (byte)source.Data[y, x, 1];  //G
                    r = (byte)source.Data[y, x, 2];  //R

                    result.Data[y, (cols - x - 1), 0] = b;
                    result.Data[y, (cols - x - 1), 1] = g;
                    result.Data[y, (cols - x - 1), 2] = r;
                }
            }
            return result;
        }

        public static Image<Bgr, Byte> ConvertToOtsu(Image<Bgr, Byte> source)
        {
            Image<Bgr, Byte> result = new Image<Bgr, Byte>(source.Width, source.Height);
            byte grayColor = 0;

            int rows = source.Height;
            int cols = source.Width;

            byte r, g, b;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    b = (byte)source.Data[y, x, 0];  //B
                    g = (byte)source.Data[y, x, 1];  //G
                    r = (byte)source.Data[y, x, 2];  //R
                    grayColor = (byte)(b * 0.114 + g * 0.587 + r * 0.299);
                    //average += (int)grayColor;
                }
            }
            /*average = average / (rows * cols);
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    b = (byte)source.Data[y, x, 0];  //B
                    g = (byte)source.Data[y, x, 1];  //G
                    r = (byte)source.Data[y, x, 2];  //R
                    grayColor = (byte)(b * 0.114 + g * 0.587 + r * 0.299);
                    if (grayColor <= average) grayColor = 0;
                    else grayColor = 255;
                    result.Data[y, x, 0] = grayColor;
                    result.Data[y, x, 1] = grayColor;
                    result.Data[y, x, 2] = grayColor;
                }
            }*/
            return result;
        }

        public static Image<Bgr, Byte> HistogramEqualization(Image<Bgr, Byte> source)
        {
            Image<Bgr, Byte> result = new Image<Bgr, Byte>(source.Width, source.Height);

            return result;
        }

        public static Image<Bgr, Byte> imageBlending(Image<Bgr, Byte> source, Image<Bgr, Byte> source2, double Threshold)
        {
            Image<Bgr, Byte> result = new Image<Bgr, Byte>(source.Width, source.Height);

            return result;
        }

        public static Image<Bgr, Byte> Rotating(Image<Bgr, Byte> source, double theta)
        {
            Image<Bgr, Byte> result = new Image<Bgr, Byte>(source.Width, source.Height);

            return result;
        }
    }
}