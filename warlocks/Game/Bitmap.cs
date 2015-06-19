using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

namespace warlocks.Game
{
  public class BMAP
  {

    Bitmap _bitmap;
    int[] _intarray;
    public bool ready = false;
    int _height = 0;
    int _width = 0;
    int counter = 0;

    public int dirtycounter = 0;
    public List<pixel> _alldirtypixels;
    private ConcurrentQueue<pixel> _dirtypixels;
    public pixel[] dirtypixels
    {
      get
      {

        return _dirtypixels.ToArray();

      }
    }

    public int dirtypixellength { get { return _dirtypixels.Count; } }

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
      _alldirtypixels = new List<pixel>();

      using (var fs = File.OpenRead(name))
      {
        _bitmap = new Bitmap(fs);
      }

      this.processBitmap();

      //Thread newThread = new Thread(this.processBitmap);
      //newThread.Start();


    }

    public void processBitmap()
    {

      int height = _bitmap.Height;
      int width = _bitmap.Width;

      _height = height;
      _width = width;

      _intarray = new int[height * width];

      Color pixelcolor;

      for (int i = 0; i < height; i++)
      {
        for (int j = 0; j < width; j++)
        {

          pixelcolor = _bitmap.GetPixel(j, i);

          if ((pixelcolor.B == 255) && (pixelcolor.R == 255) && (pixelcolor.G == 255))
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
          Debug.WriteLine("out of bounds");

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



    public void setPixels(int digx, int digy, int radius, int color, WGame game)
    {

      //var temp = new List<pixel>();

      game.leveldataready = true;

      for (int i = -radius; i < radius; i++)
      {
        for (int j = -radius; j < radius; j++)
        {


          if (i * i + j * j <= radius * radius)
          {
            setPixel(digx + i, digy + j, color);

          }



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

      return PIXEL.dirt;

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

    public pixel[] getDirtyPixels()
    {

      List<pixel> templist = new List<pixel>();

      pixel temppixel;

      while (_dirtypixels.TryDequeue(out temppixel))
      {
        templist.Add(temppixel);
      }

      _alldirtypixels.AddRange(templist);

      return templist.ToArray();
    }
  }


  public static class PIXEL2
  {
    public const int empty = 0;
    public const int dirt = 1;
    public const int rock = 2;
    public const int Right = 3;

  }

  public enum PIXEL { empty, dirt, rock, blood };


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

    public string ToJson()
    {
      return "{\"X\":" + this.X + ",\"Y\":" + this.Y + ",\"color\":" + this.color + "}";
    }


  }
}