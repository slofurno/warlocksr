using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warlocks.Game
{
  public class SObject : IProcessable
  {
    public double x { get; set; }
    public double y { get; set; }
    public double velX { get; set; }
    public double velY { get; set; }


    public void Process(WGame game)
    {
      x += velX;
      y += velY;
    }

    public string ToJson()
    {
      return "{\"x\":" + this.x + ",\"y\":" + this.y + ",\"vx\":" + this.velX + ",\"vy\":" + this.velY + "}";
    }

  }
}
