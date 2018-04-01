﻿using System;

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
            int[,] grayColor = new int[256, 3];
            byte gray = 0;
            int rows = source.Height;
            int cols = source.Width;
            int totalPixel = rows * cols;
            float meanAverage = 0, biggerAverage = 0, smallerAverage = 0;
            float tempG = 0, finalG = 1, n0 = 0, n1 = 0, I0 = 0, I1 = 0;
            float w0 = 0, w1 = 0;

            byte t = 0, finalT = 0;
            byte r, g, b;

            for (int i = 0; i < 256; i++)
            {
                grayColor[i, 0] = i;
                grayColor[i, 1] = 0;
            }

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    b = (byte)source.Data[y, x, 0];  //B
                    g = (byte)source.Data[y, x, 1];  //G
                    r = (byte)source.Data[y, x, 2];  //R
                    gray = (byte)(b * 0.114 + g * 0.587 + r * 0.299);
                    for (int i = 0; i < 256; i++)
                    {
                        if (gray == grayColor[i, 0]) grayColor[i, 1]++;
                    }
                    meanAverage += gray; //灰階總數
                }
            }
            meanAverage = meanAverage / (rows * cols); //整體平均灰階數

            for (int i = 0; i < 256; i++)
            {
                grayColor[i, 2] = grayColor[i, 0] * grayColor[i, 1];
            }

            for (t = 0; t < 255; t++)
            {
                n0 = 0;
                n1 = 0;
                I0 = 0;
                I1 = 0;
                for (int i = 0; i < 256; i++)
                {
                    if (grayColor[i, 0] < t)
                    {
                        n0 += grayColor[i, 1];
                        I0 += grayColor[i, 2];
                    }
                    else
                    {
                        n1 += grayColor[i, 1];
                        I1 += grayColor[i, 2];
                    }
                }
                if (n0 == 0 || n1 == 0) continue;
                else
                {
                    smallerAverage = I0 / n0;
                    biggerAverage = I1 / n1;
                    w0 = n0 / totalPixel;
                    w1 = n1 / totalPixel;
                    tempG = (int)(w0 * w1 * Math.Abs((biggerAverage - meanAverage) * (smallerAverage - meanAverage)));
                    if (tempG > finalG)
                    {
                        finalG = tempG;
                        finalT = t;
                    }
                }
            }

            for (int y = 0; y < rows; y++) //利用平均灰階計算二值化
            {
                for (int x = 0; x < cols; x++)
                {
                    b = (byte)source.Data[y, x, 0];  //B
                    g = (byte)source.Data[y, x, 1];  //G
                    r = (byte)source.Data[y, x, 2];  //R
                    gray = (byte)(b * 0.114 + g * 0.587 + r * 0.299);
                    if (gray <= finalT) gray = 0;
                    else gray = 255;
                    result.Data[y, x, 0] = gray;
                    result.Data[y, x, 1] = gray;
                    result.Data[y, x, 2] = gray;
                }
            }

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