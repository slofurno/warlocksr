using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warlocks.Game
{
  public class NObject : IProcessable
  {
    public double x;
    public double y;
    public double velX;
    public double velY;
    private int ix;
    private int iy;
    private double velX1;
    private double velY1;
    public int intX { get { return (int)this.x; } }
    public int intY { get { return (int)this.y; } }


    public NObject(int x, int y, double velX, double velY)
    {

      this.x = x;
      this.y = y;

      double angle = RNG.next(Math.PI * 2);

      this.velX = Math.Cos(angle) * 1.5 + velX;
      this.velX = Math.Sin(angle) * 1.5 + velY;


    }

    public string ToJson()
    {
      return "{\"x\":" + this.x + ",\"y\":" + this.y + "}";
    }

    public void Process(WGame game)
    {

      bool doExplode = false;

      velY += .1;

      y += velY;
      x += velX;

      int ix = (int)x;
      int iy = (int)y;


      if (!game.leveldata.inside(ix, iy))
      {
        doExplode = true;

      }

      if (game.leveldata.getPixel(ix, iy) == PIXEL.dirt)
      {
        game.leveldata.setPixel2(ix, iy, PIXEL.blood);
        doExplode = true;


      }
      else if (game.leveldata.getPixel(ix, iy) != PIXEL.empty)
      {

        doExplode = true;

      }





      if (doExplode)
      {
        game.bloodlist.Remove(this);
      }


    }

  }
}
