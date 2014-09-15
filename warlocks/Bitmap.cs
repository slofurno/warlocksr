using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Net.Http;
using System.Collections.Concurrent;

namespace warlocks
{
    public class BMAP
    {

        Bitmap _bitmap;
        int[] _intarray;
        public bool ready = false;
        int _height=0;
        int _width=0;
        int counter = 0;

        public int dirtycounter = 0;
        private ConcurrentQueue<pixel> _dirtypixels;
        public pixel[] dirtypixels { get {

            return _dirtypixels.ToArray();

        } }

        public int dirtypixellength { get { return dirtycounter; } }

        public int height
        {
            get
            {
                return _height;
            }
        }

        public int width
        {
            get
            {
                return _width;
            }
        }

        public BMAP(string name)
        {

            _dirtypixels = new ConcurrentQueue<pixel>();

            try
            {
                System.Net.WebRequest request =
                    System.Net.WebRequest.Create(
                    "http://warlocksr.azurewebsites.net/testlevel.bmp");
                System.Net.WebResponse response = request.GetResponse();
                System.IO.Stream responseStream =
                    response.GetResponseStream();
                _bitmap = new Bitmap(responseStream);
                

            }
            catch (System.Net.WebException)
            {
                Debug.WriteLine("this diddnt work...");
            }


            
            //_bitmap = new Bitmap(name);
            

            Thread newThread = new Thread(this.processBitmap);

            newThread.Start();

            //processBitmap();

        }

        public void processBitmap()
        {

            int height = _bitmap.Height;
            int width = _bitmap.Width;

            _height = height;
            _width = width;

            _intarray = new int[height*width];

            Color pixelcolor;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    pixelcolor = _bitmap.GetPixel(j, i);

                    if ((pixelcolor.B==255)&&(pixelcolor.R==255)&&(pixelcolor.G==255))
                    {

                        _intarray[i * width + j] = 0;

                    }
                    else
                    {
                        _intarray[i * width + j] = 1;
                    }

                    counter++;

                }


            }

            this.ready = true;

        }

        public int getColor(Vector2 pos)
        {

            int x = (int)pos.X;
            int y = (int)pos.Y;

            var result = 0;

            if (this.ready)
            {

                if (x < 0 || y < 0 || x >= _width || y >= _height)
                {
                    Debug.WriteLine("out of bounds" );

                }
                else
                {
                    result = _intarray[y * _width + x];

                    Debug.WriteLine("number : " + result);
                }
            }

            Debug.WriteLine("how many : " + counter);

            return result;

        }


        public int getColor(int x, int y)
        {

           

            var result = 0;

            if (this.ready)
            {

                if (x < 0 || y < 0 || x >= _width || y >= _height)
                {
                    Debug.WriteLine("out of bounds");

                }
                else
                {
                    result = _intarray[y * _width + x];

                    //Debug.WriteLine("number : " + result);
                }
            }

            //Debug.WriteLine("how many : " + counter);

            return result;

        }

        public int setPixel(int x, int y, int color)
        {



            

            if (this.ready)
            {



                if (x < 0 || y < 0 || x >= _width || y >= _height)
                {
                    Debug.WriteLine("out of bounds");
                    

                }
                else if (_intarray[y * _width + x] == color)
                {

                    return 0;
                    //Debug.WriteLine("number : " + result);
                }
                else
                {
                    _intarray[y * _width + x] = color;

                    _dirtypixels.Enqueue(new pixel(x, y, color));

                    dirtycounter++;

                    return 1;

                }
            }

            //Debug.WriteLine("how many : " + counter);

            return -1;

        }



        public void setPixels(int iposx, int iposy, int digx, int digy, int color, WarlockGame game)
        {

            //var temp = new List<pixel>();

            game.leveldataready = true;

            for (int i = digx-7; i < digx+7; i++)
            {
                for (int j = digy-7; j < digy+7; j++)
                {
                    setPixel(i, j, color);

                        //temp.Add(new pixel(i, j, 0));

                    

                }


            }
            /*
            if (temp.Count > 0)
            {

                game.sendPixels(temp);
            }
             * */

            

        }

        public PIXEL getPixel(int x, int y)
        {

            if (this.ready)
            {

                if (x < 0 || y < 0 || x >= _width || y >= _height)
                {
                    Debug.WriteLine("out of bounds");

                    


                }
                else
                {
                    return (PIXEL)_intarray[y * _width + x];

                    //Debug.WriteLine("number : " + result);
                }
            }

            return PIXEL.empty;

        }

        public void setPixel2(int x, int y, PIXEL pixel)
        {

            int color = (int)pixel;

            if (this.ready)
            {

                if (x < 0 || y < 0 || x >= _width || y >= _height)
                {
                    Debug.WriteLine("out of bounds");


                }
                else if (_intarray[y * _width + x] == color)
                {

                    return;
                    //Debug.WriteLine("number : " + result);
                }
                else
                {
                    _intarray[y * _width + x] = color;

                    _dirtypixels.Enqueue(new pixel(x, y, color));

                    return;

                }
            }



        }

        public bool inside(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
            {
                return false;


            }

            return true;
        }
    }


    public static class PIXEL2
    {
        public const int empty = 0;
        public const int dirt = 1;
        public const int rock = 2;
        public const int Right = 3;

    }

    public enum PIXEL {empty, dirt, rock, blood };
  

    public class pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int color { get; set; }

        

        public pixel(int x, int y, int color)
        {
            this.X = x;
            this.Y = y;
            this.color = color;
        }


    }
}