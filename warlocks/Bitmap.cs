using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Net.Http;

namespace warlocks
{
    public class BMAP
    {

        Bitmap _bitmap;
        int[] _intarray;
        public bool ready;
        int _height=0;
        int _width=0;
        int counter = 0;

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
            this.ready = false;

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
                    return 1;

                }
            }

            //Debug.WriteLine("how many : " + counter);

            return -1;

        }



        internal void setPixels(int iposx, int iposy, int digx, int digy, int color, WarlockGame game)
        {

            var temp = new List<pixel>();

            for (int i = digx-7; i < digx+7; i++)
            {
                for (int j = digy-7; j < digy+7; j++)
                {
                    if (setPixel(i, j, color)>0){

                        temp.Add(new pixel(i, j, 0));

                    }

                }


            }

            if (temp.Count > 0)
            {

                game.sendPixels(temp);
            }
               
        }
    }

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