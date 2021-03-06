﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace warlocks.Game
{
  public class Ninjarope
  {

    public bool attached { get; set; }
    public int length { get; set; }
    public bool isout;
    public double x;
    public double y;
    public double velX;
    public double velY;

    public Ninjarope()
    {
      this.attached = false;
      this.isout = false;
    }

    public string ToJson()
    {
      return "{\"x\":" + this.x + ",\"y\":" + this.y + ",\"isout\":" + this.isout.ToString().ToLower() + "}";
    }

    public void process(Worm owner, WGame game)
    {

      if (isout)
      {

        x += velX;
        y += velY;

        int ix = (int)x;
        int iy = (int)y;

        double forceX, forceY;
        double diffX = (x - owner.position.X);
        double diffY = (y - owner.position.Y);

        forceX = (diffX * 1.3);
        forceY = (diffY * 1.3);

        double curLen = Math.Sqrt(diffX * diffX + diffY * diffY);

        double springforce = 0;

        if (curLen > length)
        {
          springforce = (curLen - length);
        }

        double angle = Math.Atan2(diffY, diffX);


        forceX = Math.Cos(angle) * springforce;
        forceY = Math.Sin(angle) * springforce;


        if (ix <= 0
        || ix >= game.leveldata.width - 1
        || iy <= 0
        || iy >= game.leveldata.height - 1
        || game.CheckLocation(ix, iy))
        {
          if (!attached)
          {
            length = (int)(curLen * .7);
            attached = true;
          }

          velX = 0;
          velY = 0;
        }

        else
        {
          attached = false;
        }

        if (attached)
        {
          // curLen can't be 0
          if (curLen > length)
          {
            owner.velocity.X += forceX / curLen;
            owner.velocity.Y += forceY / curLen;
          }
        }
        else
        {
          //velY += .05; //gravity
          if (curLen > length)
          {
            //velX -= forceX / curLen;
            //velY -= forceY / curLen;
          }
        }
      }
    }


  }
}
