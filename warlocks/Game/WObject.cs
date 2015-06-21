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
    public int timeLeft { get; set; }
    int ownerIdx;
    int curFrame;
    public int firedbyid;


    public int intX { get { return (int)this.x; } }
    public int intY { get { return (int)this.y; } }

    public string ToJson()
    {
      return "{\"x\":" + this.x + ",\"y\":" + this.y + "}";
    }

    public void blowUpObject(WGame game)
    {

      int iy = (int)this.y;
      int ix = (int)this.x;

      var w = Common.Weapons[this.id];

      for (var i = 0; i < w.splinterAmount; i++)
      {
        double angle = RNG.next(Math.PI * 2);

        var s = new SObject() { 
          x = this.x, 
          y = this.y,
          velX = Math.Cos(angle), 
          velY = Math.Sin(angle)
        };

        game._splinters.Add(s);

      }

      var radius = w.explosionRadius;

      for (int i = -radius; i < radius; i++)
      {
        for (int j = -radius; j < radius; j++)
        {
          if (i * i + j * j <= radius * radius && game.leveldata.getPixel(ix + i, iy + j) != PIXEL.rock)
          {
            game.leveldata.setPixel(ix + i, iy + j, 0);
          }
        }
      }

      foreach (var worm in game.wormList)
      {
        if (w.explosionRadius * w.explosionRadius > (worm.position.X - this.x) * (worm.position.X - this.x) + (worm.position.Y - this.y) * (worm.position.Y - this.y))
        {
          for (var i = 0; i < 32; i++)
          {
            game.bloodlist.Add(new NObject((int)worm.position.X, (int)worm.position.Y, this.velX / 3, this.velY / 3));
          }
        }
      }


    

      //game.leveldata.setPixels(ix, iy, w.explosionRadius, 0);

    }

    public void Process(WGame game)
    {
      int iter = 2;

      while (--iter >= 0)
      {

        var w = Common.Weapons[this.id];
        bool doExplode = false;
        bool bounced = false;

        int inewX = (int)(x + velX);
        int inewY = (int)(y + velY);

        int ix = (int)x;
        int iy = (int)y;

        if (w.Bounce > 0)
        {
          if (!game.leveldata.inside(inewX, iy)
          || game.leveldata.getPixel(inewX, iy) != PIXEL.empty)
          {
            if (w.Bounce != 100)
            {
              velX = -velX * w.Bounce / 100;
              velY = (velY * 4) / 5;
            }
            else
            {
              velX = -velX;
            }

            bounced = true;
          }

          if (!game.leveldata.inside(ix, inewY)
          || game.leveldata.getPixel(ix, inewY) != PIXEL.empty)
          {
            if (w.Bounce != 100)
            {
              velY = -velY * w.Bounce / 100;
              velX = (velX * 4) / 5; // TODO: Read from EXE
            }
            else
            {
              velY = -velY;

            }

            bounced = true;
          }


        }

        if (!game.leveldata.inside(inewX, inewY)
          || game.leveldata.getPixel(inewX, inewY) != PIXEL.empty)
        {
          if (w.Bounce == 0)
          {
            doExplode = true;
          }

        }
        else
        {
          velY += w.gravity;
        }


        foreach (var worm in game.wormList)
        {
          if ((worm.id != this.firedbyid) && Worm.checkForSpecWormHit(game, inewX, inewY, 6, worm))
          {
            for (var i = 0; i < 32; i++)
            {
              game.bloodlist.Add(new NObject(inewX, inewY, velX / 3, velY / 3));
            }

            doExplode = true;
          }
        }

        if (w.timeToExplosion > 0)
        {
          --timeLeft;
          if (timeLeft <= 0)
          {
            doExplode = true;
          }
        }

        y += velY;
        x += velX;

        if (doExplode)
        {
          blowUpObject(game);
          game.wormobjects.free(this);
        }

      }

    }

  }
}
