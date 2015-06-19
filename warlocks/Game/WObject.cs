using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warlocks.Game
{
  public class WObject : IProcessable
  {

    public double x;
    public double y;
    public double velX;
    public double velY;
    public int id;
    int ownerIdx;
    int curFrame;
    int timeLeft;
    public int firedbyid;

    public int intX { get { return (int)this.x; } }
    public int intY { get { return (int)this.y; } }

    public string ToJson()
    {
      return "{\"x\":" + this.x + ",\"y\":" + this.y + "}";
    }

    public void Process(WGame game)
    {

      bool doExplode = false;

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
        game.leveldata.setPixels(ix, iy, 3, 0, game);
        //game.leveldata.setPixel2(ix, iy, PIXEL.empty);
        doExplode = true;
      }
      else if (game.leveldata.getPixel(ix, iy) != PIXEL.empty)
      {

        doExplode = true;

      }

      foreach (var worm in game.wormList)
      {
        if ((worm.id != this.firedbyid) && Worm.checkForSpecWormHit(game, ix, iy, 6, worm))
        {
          for (var i = 0; i < 32; i++)
          {
            game.bloodlist.Add(new NObject(ix, iy, velX / 3, velY / 3));
          }

          doExplode = true;
        }
      }

      if (doExplode)
      {
        game.wormobjects.free(this);
      }

    }

  }
}
