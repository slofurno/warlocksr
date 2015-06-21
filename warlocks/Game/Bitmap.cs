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

    public string json { get; set; }

    public int dirtycounter = 0;
    public List<Pixel> _alldirtypixels;
    private ConcurrentQueue<Pixel> _dirtypixels;
    public Pixel[] dirtypixels
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

      _dirtypixels = new ConcurrentQueue<Pixel>();
      _alldirtypixels = new List<Pixel>();

      var img = Image.FromFile("level1.png");
      _bitmap = new Bitmap(img);
      /*
      using (var fs = File.OpenRead(name))
      {
        _bitmap = new Bitmap(fs);
      }
       * */

      this.processBitmap();
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

          if (pixelcolor.A == 0)
          {
            _intarray[i * width + j] = 0;
          }
          else if (pixelcolor.A == 255)
          {
            _intarray[i * width + j] = 2;
          }
          else
          {
            _intarray[i * width + j] = 1;
          }
          

          counter++;
        }
      }

      this.json = "[" + string.Join(",", _intarray) + "]";        //,_intarray.Select(x=>x.ToString()).ToArray()

      this.ready = true;

    }


    public int getColor(int x, int y)
    {
      var result = 0;

      if (this.ready)
      {
        //TODO:i think somewhere there are some bounds issues dropping fps
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

      if (x < 0 || y < 0 || x >= _width || y >= _height)
      {

      }
      else if (_intarray[y * _width + x] == color)
      {
        return 0;
      }
      else
      {
        _intarray[y * _width + x] = color;
        _dirtypixels.Enqueue(new Pixel(x, y, color));
        dirtycounter++;

        return 1;
      }
      return -1;

    }



    public void setPixels(int digx, int digy, int radius, int color)
    {

      for (int i = -radius; i < radius; i++)
      {
        for (int j = -radius; j < radius; j++)
        {

          if (i * i + j * j <= radius * radius)
          {
            setPixel(digx + i, digy + j, color);

          }

        }
      }


    }

    public PIXEL getPixel(int x, int y)
    {

        if (x < 0 || y < 0 || x >= _width || y >= _height)
        {
          Debug.WriteLine("out of bounds");
          return PIXEL.rock;

        }
        else
        {
          var i = _intarray[y * _width + x];
          var pixel = (PIXEL)i;
          return pixel;
          //return (PIXEL)_intarray[y * _width + x];
        }

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

          _dirtypixels.Enqueue(new Pixel(x, y, color));

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

    public Pixel[] getDirtyPixels()
    {

      List<Pixel> templist = new List<Pixel>();

      Pixel temppixel;

      while (_dirtypixels.TryDequeue(out temppixel))
      {
        templist.Add(temppixel);
      }

      _alldirtypixels.AddRange(templist);

      return templist.ToArray();
    }
  }



}