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

    Dictionary<Color, int> _paletteLookup;
    HashSet<Color> _existingColors;
    PIXEL[] _levelData;
    int[] _colorData;

    public bool ready = false;
    int _colorCount = 0;

    int _height = 0;
    int _width = 0;
    int counter = 0;

    public string json { get; set; }
    public string _palette { get; set; }

    public int dirtycounter = 0;
    public List<Pixel> _alldirtypixels;
    private List<Pixel> _dirtypixels;
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

      

      _dirtypixels = new List<Pixel>();
      _alldirtypixels = new List<Pixel>();
      _existingColors = new HashSet<Color>();
      _paletteLookup = new Dictionary<Color, int>();

      using (var img = Image.FromFile("level1.png"))
      {
        var bitmap = new Bitmap(img);
        processBitmap(bitmap);
      }
      /*
      using (var fs = File.OpenRead(name))
      {
        _bitmap = new Bitmap(fs);
      }
       * */

    }

    public void processBitmap(Bitmap bitmap)
    {

      int height = bitmap.Height;
      int width = bitmap.Width;

      _height = height;
      _width = width;

      _levelData = new PIXEL[height * width];
      _colorData = new int[height * width];

      Color pixelcolor;
      int key;

      for (int j = 0; j < height; j++)
      {
        for (int i = 0; i < width; i++)
        {

          pixelcolor = bitmap.GetPixel(i, j);

          if (!_paletteLookup.TryGetValue(pixelcolor, out key))
          {
            _paletteLookup.Add(pixelcolor, _colorCount);
            key = _colorCount;
            _colorCount++;
          }

          _colorData[j * width + i] = key;

          if (pixelcolor.A == 0)
          {
            _levelData[j * width + i] = 0;
          }
          else if (pixelcolor.A == 255)
          {
            _levelData[j * width + i] = PIXEL.rock;
          }
          else
          {
            _levelData[j * width + i] = PIXEL.dirt;
          }


          counter++;
        }
      }

      var palette = _paletteLookup.Keys.Select(x => new Tuple<int, Color>(_paletteLookup[x], x)).OrderBy(x => x.Item1).Select(x => x.Item2).ToArray();

      _palette = "[" + String.Join(",", palette.Select(x => "{\"r\":" + x.R + ",\"g\":" + x.G + ",\"b\":" + x.B + ",\"a\":" + x.A + "}").ToArray()) + "]";

      this.json = "[" + string.Join(",", _colorData) + "]";        //,_intarray.Select(x=>x.ToString()).ToArray()

      this.ready = true;

    }

    public void setPixels(int digx, int digy, int radius, PIXEL pixel)
    {
      for (int i = -radius; i < radius; i++)
      {
        for (int j = -radius; j < radius; j++)
        {
          if (i * i + j * j <= radius * radius)
          {
            setPixel(digx + i, digy + j, pixel);
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
        return _levelData[y * _width + x];
      }
    }

    public void setPixel(int x, int y, PIXEL pixel)
    {

      if (x < 0 || y < 0 || x >= _width || y >= _height)
      {
        Debug.WriteLine("out of bounds");
      }
      else if (_levelData[y * _width + x] != pixel)
      {
        _levelData[y * _width + x] = pixel;
        _dirtypixels.Add(new Pixel(x, y, pixel));
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
      var p = _dirtypixels.ToArray();

      _dirtypixels.Clear();
      _alldirtypixels.AddRange(p);

      return p;
    }


  }
}