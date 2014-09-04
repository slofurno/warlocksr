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
        int _height;
        int _width;
        int counter = 0;

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


    }
}