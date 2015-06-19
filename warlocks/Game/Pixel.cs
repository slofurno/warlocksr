using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warlocks.Game
{
  public enum PIXEL { empty=0, dirt, rock, blood };


  public class Pixel
  {
    public int X { get; set; }
    public int Y { get; set; }
    public int color { get; set; }

    public Pixel(int x, int y, int color)
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
