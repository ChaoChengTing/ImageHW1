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

            int rows = source.Height;
            int cols = source.Width;

            byte[] bColor = new byte[rows * cols];
            byte[] gColor = new byte[rows * cols];
            byte[] rColor = new byte[rows * cols];
            int[] bGrade = new int[256];
            int[] gGrade = new int[256];
            int[] rGrade = new int[256];
            int[] bNew = new int[256];
            int[] gNew = new int[256];
            int[] rNew = new int[256];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    bColor[y * cols + x] = (byte)source.Data[y, x, 0];  //B
                    gColor[y * cols + x] = (byte)source.Data[y, x, 1];  //G
                    rColor[y * cols + x] = (byte)source.Data[y, x, 2];  //R
                }
            }

            for (int i = 0; i < 256; i++)
            {
                bGrade[i] = 0;
                gGrade[i] = 0;
                rGrade[i] = 0;
                for (int j = 0; j < rows * cols; j++)
                {
                    if (i == (int)bColor[j])
                    {
                        bGrade[i]++;
                    }
                    if (i == (int)gColor[j])
                    {
                        gGrade[i]++;
                    }
                    if (i == (int)rColor[j])
                    {
                        rGrade[i]++;
                    }
                }
            }

            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    bGrade[i] = bGrade[i - 1] + bGrade[i];
                    if (bGrade[i - 1] != bGrade[i])
                    {
                        bNew[i] = (byte)(255 * bGrade[i] / (rows * cols));
                    }
                    gGrade[i] = gGrade[i - 1] + gGrade[i];
                    if (gGrade[i - 1] != gGrade[i])
                    {
                        gNew[i] = (byte)(255 * gGrade[i] / (rows * cols));
                    }
                    rGrade[i] = rGrade[i - 1] + rGrade[i];
                    if (rGrade[i - 1] != rGrade[i])
                    {
                        rNew[i] = (byte)(255 * rGrade[i] / (rows * cols));
                    }
                }
            }

            for (int i = 0; i < 256; i++)
            {
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        if (bColor[y * cols + x] == i)
                        {
                            result.Data[y, x, 0] = (byte)bNew[i];
                        }
                        if (gColor[y * cols + x] == i)
                        {
                            result.Data[y, x, 1] = (byte)gNew[i];
                        }
                        if (rColor[y * cols + x] == i)
                        {
                            result.Data[y, x, 2] = (byte)rNew[i];
                        }
                    }
                }
            }
            return result;
        }

        public static Image<Bgr, Byte> imageBlending(Image<Bgr, Byte> source, Image<Bgr, Byte> source2, double Threshold)
        {
            Image<Bgr, Byte> result = new Image<Bgr, Byte>(source.Width, source.Height);

            int rows = source.Height;
            int cols = source.Width;

            byte r, g, b;  //source
            byte r2, g2, b2;  //source2
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    b = (byte)source.Data[y, x, 0];  //B from source
                    g = (byte)source.Data[y, x, 1];  //G from source
                    r = (byte)source.Data[y, x, 2];  //R from source

                    b2 = (byte)source2.Data[y, x, 0];  //B from source2
                    g2 = (byte)source2.Data[y, x, 1];  //G from source2
                    r2 = (byte)source2.Data[y, x, 2];  //R from source2

                    result.Data[y, x, 0] = (byte)(b * Threshold + b2 * (1 - Threshold));
                    result.Data[y, x, 1] = (byte)(g * Threshold + g2 * (1 - Threshold));
                    result.Data[y, x, 2] = (byte)(r * Threshold + r2 * (1 - Threshold));
                }
            }

            return result;
        }

        public static Image<Bgr, Byte> Rotating(Image<Bgr, Byte> source, double theta)
        {
            Image<Bgr, Byte> result = new Image<Bgr, Byte>(source.Width, source.Height);

            return result;
        }
    }
}