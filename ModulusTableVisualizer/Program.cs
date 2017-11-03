using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace ModulusTableVisualizer
{
    class Program
    {
        public static byte[] ImageBuffer;
        public static int Width;
        public static int Height;

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the size of the table that you would like to generate: ");
            int size = int.Parse(Console.ReadLine());
            Width = size;
            Height = size;
            ImageBuffer = new byte[Width * Height * 4];
            int[,] Table = GenTable(size);

            //creates the image where the color shifts through the whole spectrum as value increases
            byte[] NextPix;
            for(int y = 0; y < size; y++)
            {
                for(int x = 0; x < size; x++)
                {
                    NextPix = CalcColorFullSpectrum(size, Table[x, y]);
                    WritePixel(x, y, NextPix[0], NextPix[1], NextPix[2]);
                }
            }
            SaveArrayAsImage(ImageBuffer, "FullSpectrum-" + size);

            //creates the image where the grayscale shifts through whole spectrum as value increases
            byte Color;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Color = CalcColorBW(size, Table[x, y]);
                    WritePixel(x, y, Color, Color, Color);
                }
            }
            SaveArrayAsImage(ImageBuffer, "BlackAndWhite-" + size);

            //creates the image where the grayscale shifts through whole spectrum as digit count increases increases
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Color = CalcColorBWDigitCount(size, Table[x, y]);
                    WritePixel(x, y, Color, Color, Color);
                }
            }
            SaveArrayAsImage(ImageBuffer, "BlackAndWhiteDigitCount-" + size);
            Console.ReadLine();
        }

        private static void PrintTable(int[,] Table)
        {
            for(int y = 0; y < Table.GetLength(0); y++)
            {
                for(int x = 0; x < Table.GetLength(1); x++)
                {
                    Console.Write(Table[x, y].ToString("000") + ", ");
                }
                Console.WriteLine();
            }
        }

        private static byte CalcColorBWDigitCount(int MaxVal, int Val)
        {
            int max = MaxVal.ToString().Length;
            int ConvFact = 128 / max;
            return (byte)(Val * ConvFact + 64);
        }

        private static byte CalcColorBW(int MaxVal, int Val)
        {
            double ConvFact = 256 / MaxVal;
            return (byte)(Val * ConvFact);
        }

        private static byte[] CalcColorFullSpectrum(int MaxVal, int Val)
        {
            byte[] Pix = new byte[3];

            double ConvFact = (double)(256 * 3) / (double)MaxVal;
            int i = (int)(Val * ConvFact);

            if(i < 256)
            {
                Pix[2] = (byte)(i % 256);
            }
            else if(i < 512)
            {
                Pix[1] = (byte)(i % 256);
            }
            else
            {
                Pix[0] = (byte)(i % 256);
            }

            return Pix;
        }

        private static int[,] GenTable(int size)
        {
            int[,] Table = new int[size, size];
            for(int y = 1; y <= size; y++)
            {
                for(int x = 1; x <= size; x++)
                {
                    Table[x - 1, y - 1] = (x * y) % (size + 1);
                }
            }

            return Table;
        }

        private static int GetOffset(int xPos, int yPos)
        {
            int offset = ((Width * 4) * yPos + (xPos * 4));

            return offset;
        }

        private static void WritePixel(int xPos, int yPos, byte r, byte g, byte b)
        {
            int offset = GetOffset(xPos, yPos);
            ImageBuffer[offset] = b;
            ImageBuffer[offset + 1] = g;
            ImageBuffer[offset + 2] = r;
            ImageBuffer[offset + 3] = 255;
        }

        private static void SaveArrayAsImage(byte[] ImageBuffer, string fileName)
        {
            unsafe
            {
                fixed (byte* ptr = ImageBuffer)
                {
                    using (Bitmap image = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppRgb, new IntPtr(ptr)))
                    {
                        image.Save(@"C:\Users\Seth Dolin\Desktop\" + fileName + ".png");
                    }
                }
            }
        }
    }
}
