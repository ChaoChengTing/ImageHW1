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
            int[,] grayColor = new int[256, 3]; //[灰階的數字(0~255),每一個灰階有幾個像素,前兩項相乘(用來算平均)]
            byte gray = 0;
            int rows = source.Height;
            int cols = source.Width;
            int totalPixel = rows * cols;
            float meanAverage = 0, biggerAverage = 0, smallerAverage = 0;
            float tempG = 0, finalG = 1, n0 = 0, n1 = 0, I0 = 0, I1 = 0;
            float w0 = 0, w1 = 0;

            byte t = 0, finalT = 0;
            byte r, g, b;

            for (int i = 0; i < 256; i++) //初始化二維陣列值
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
                        if (gray == grayColor[i, 0]) grayColor[i, 1]++; //判斷每一個灰階有幾個像素
                    }
                    meanAverage += gray; //灰階總數
                }
            }
            meanAverage = meanAverage / (rows * cols); //整體平均灰階數

            for (int i = 0; i < 256; i++)
            {
                grayColor[i, 2] = grayColor[i, 0] * grayColor[i, 1];
            }

            for (t = 0; t < 255; t++) //閥值(Threshold) t從0開始到255去做比較 取出最好的t值
            {
                n0 = 0; //小於閥值的總像素
                n1 = 0; //大於閥值的總像素
                I0 = 0; //小於閥值的總像素*灰階值(算平均用)
                I1 = 0; //大於閥值的總像素*灰階值(算平均用)
                for (int i = 0; i < 256; i++)
                {
                    if (grayColor[i, 0] < t) //小於閥值的部分
                    {
                        n0 += grayColor[i, 1];
                        I0 += grayColor[i, 2];
                    }
                    else //大於等於閥值的部分
                    {
                        n1 += grayColor[i, 1];
                        I1 += grayColor[i, 2];
                    }
                }
                if (n0 == 0 || n1 == 0) continue; //避免分母為0
                else
                {
                    smallerAverage = I0 / n0; //小於閥值的總平均
                    biggerAverage = I1 / n1; //大於閥值的總平均
                    w0 = n0 / totalPixel; //算出整張圖片<t的部分占多少
                    w1 = n1 / totalPixel; //算出整張圖片>t的部分占多少
                    tempG = (int)(w0 * w1 * Math.Abs((biggerAverage - meanAverage) * (smallerAverage - meanAverage))); //公式
                    if (tempG > finalG) //tempG的值越高表示Threshold值越好
                    {
                        finalG = tempG;
                        finalT = t;
                    }
                }
            }

            for (int y = 0; y < rows; y++) //利用利用上面計算出的t計算二值化
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
            int pixels = rows * cols;
            
            int[] bGrade = new int[256];  //統計所有色階值的數量(0~rows*cols)
            int[] gGrade = new int[256];
            int[] rGrade = new int[256];
            int bLastValue = 0;  //儲存上一次的Grade總和
            int gLastValue = 0;
            int rLastValue = 0;
            int bMinCDF = 255;  //儲存圖片中最少的色階
            int gMinCDF = 255;
            int rMinCDF = 255;
            byte[] bNew = new byte[256];  //儲存原色階轉化後新的值(0~255)
            byte[] gNew = new byte[256];
            byte[] rNew = new byte[256];
            
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    bGrade[source.Data[y, x, 0]]++;  //統計每個色階的像素量
                    gGrade[source.Data[y, x, 1]]++;
                    rGrade[source.Data[y, x, 2]]++;
                }
            }

            bMinCDF = bGrade[0];  //儲存最小CDF
            gMinCDF = gGrade[0];
            rMinCDF = rGrade[0];

            for (int i = 0; i < 256; i++)
            {
                bGrade[i] = bLastValue + bGrade[i];  //將這個色階數量轉為CDF
                gGrade[i] = gLastValue + gGrade[i];
                rGrade[i] = rLastValue + rGrade[i];
                if (bGrade[i] != bLastValue) //此圖片中若有這個色階
                {
                    bLastValue = bGrade[i];  //更新CDF的色階總數
                    bNew[i] = (byte)(255 * (bLastValue - bMinCDF) / (pixels - bMinCDF));  //計算新的色階
                }
                if (gGrade[i] != gLastValue)
                {
                    gLastValue = gGrade[i];
                    gNew[i] = (byte)(255 * (gLastValue - gMinCDF) / (pixels - gMinCDF));
                }
                if (rGrade[i] != rLastValue)
                {
                    rLastValue = rGrade[i];
                    rNew[i] = (byte)(255 * (rLastValue - rMinCDF) / (pixels - rMinCDF));
                }
            }
            
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    result.Data[y, x, 0] = bNew[source.Data[y, x, 0]];  //將原色階轉化為新色階輸出到結果
                    result.Data[y, x, 1] = gNew[source.Data[y, x, 1]];
                    result.Data[y, x, 2] = rNew[source.Data[y, x, 2]];
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

                    result.Data[y, x, 0] = (byte)(b * Threshold + b2 * (1 - Threshold));  //將第一張圖與第二張圖照比例分配輸出結果
                    result.Data[y, x, 1] = (byte)(g * Threshold + g2 * (1 - Threshold));
                    result.Data[y, x, 2] = (byte)(r * Threshold + r2 * (1 - Threshold));
                }
            }

            return result;
        }

        public static Image<Bgr, Byte> Rotating(Image<Bgr, Byte> source, double theta)
        {
            Image<Bgr, Byte> result = new Image<Bgr, Byte>(source.Width, source.Height);
            int rows = source.Height;
            int cols = source.Width;
            int newX = 0;
            int newY = 0;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int centerX = x - cols / 2;  //切換為以中間為基準的座標
                    int centerY = y - rows / 2;
                    double angle = -theta / 180 * Math.PI;  //將新圖所旋轉的角度回推原圖所旋轉的弧度

                    newX = (int)(Math.Cos(angle) * centerX - Math.Sin(angle) * centerY) + cols / 2;  //計算旋轉前的座標
                    newY = (int)(Math.Sin(angle) * centerX + Math.Cos(angle) * centerY) + rows / 2;

                    if (newX >= 0 && newX < cols && newY >= 0 && newY < rows)  //若輸出的圖還在原圖範圍中
                    {
                        result.Data[y, x, 0] = source.Data[newY, newX, 0];  //在新座標輸出原座標資料
                        result.Data[y, x, 1] = source.Data[newY, newX, 1];
                        result.Data[y, x, 2] = source.Data[newY, newX, 2];
                    }
                }
            }
            return result;
        }
    }
}