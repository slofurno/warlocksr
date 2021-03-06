﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace warlocks.Game
{

  struct Point
  {
    public int x, y;

    public Point(int x, int y)
    {
      this.x = x;
      this.y = y;
    }
  };

  public class Worm
  {
    private static int nextid = 0;
    public Vector2 position { get; set; }
    public Vector2 view { get; set; }
    public int id { get; set; }
    private WGame _world;
    public Vector2 velocity { get; set; }
    private int[] reacts = new int[4];
    private Weapon tempweapon = new Weapon() { Bounce = 3 };
    public Ninjarope ninjarope { get; set; }
    private bool ableToRope { get; set; }
    public int activeWeapon { get; set; }
    private bool canSwitch = true;

    public int weapondelayleft { get; set; }

    public Worm(WGame world, int id)
    {
      this.position = new Vector2(600, 300);
      this.view = new Vector2();
      this.id = id;
      velocity = new Vector2();
      _world = world;
      this.ninjarope = new Ninjarope();


    }
    public Worm(Vector2 position, WGame world)
    {
      this.position = position;
      this.view = new Vector2();
      this.id = nextid;
      nextid++;
      velocity = new Vector2();
      _world = world;
      this.ninjarope = new Ninjarope();
    }

    public string ToJson()
    {
      return "{\"position\":" + this.position.ToJson() +
        ",\"ninjarope\":" + this.ninjarope.ToJson() +
        ",\"view\":" + this.view.ToJson() +
        ",\"id\":" + this.id + "}";
    }

    public static class RF
    {
      public const int Down = 0;
      public const int Left = 1;
      public const int Up = 2;
      public const int Right = 3;

    }



    static Point[,] colPoints = new Point[4, 7]
        {
                { //DOWN reaction points
                        new Point(-1, -4),
                        new Point(0, -4),
                        new Point(-1, -4),
                        new Point(0, 0),
                        new Point(0, 0),
                        new Point(0, 0),
                        new Point(0, 0)
                },
                { //LEFT reaction points
                        new Point(1, -3),
                        new Point(1, -2),
                        new Point(1, -1),
                        new Point(1, 0),
                        new Point(1, 1),
                        new Point(1, 2),
                        new Point(1, 3)
                        
                },
                { //UP reaction points
                        new Point(-1, 4),
                        new Point(0, 4),
                        new Point(1, 4),
                        new Point(0, 0),
                        new Point(0, 0),
                        new Point(0, 0),
                        new Point(0, 0)
           
                },
                { //RIGHT reaction points
                        new Point(-1, -3),
                        new Point(-1, -2),
                        new Point(-1, -1),
                        new Point(-1, 0),
                        new Point(-1, 1),
                        new Point(-1, 2),
                        new Point(-1, 3)
                }
               
        };

    static int[] colPointCount = new int[4]
        {
                3,
                7,
                3,
                7
        };

    public void processPhysics(WGame game)
    {
      //Common& common = *game.common;

      double velX = this.velocity.X;
      double velY = this.velocity.Y;

      double x = this.position.X;
      double y = this.position.Y;

      if (reacts[RF.Up] > 0)
      {
        velX = (velX / 2);
      }

      if (velX > 0)
      {
        if (reacts[RF.Left] > 0)
        {
          if (velX > 3)
          {

            velX = -velX / 3;
          }
          else
            velX = 0;
        }
      }
      else if (velX < 0)
      {
        if (reacts[RF.Right] > 0)
        {
          if (velX < -3)
          {

            velX = -velX / 3;
          }
          else
            velX = 0;
        }
      }

      if (velY > 0)
      {
        if (reacts[RF.Up] > 0)
        {
          if (velY > 3)
          {

            velY = -velY / 3;
          }
          else
            velY = 0;
        }
      }
      else if (velY < 0)
      {
        if (reacts[RF.Down] > 0)
        {
          if (velY < -3)
          {

            velY = -velY / 3;
          }
          else
            velY = 0;
        }
      }

      if (reacts[RF.Up] == 0)
      {
        velY += .2; //GRAVITY
      }

      if (velX >= 0)
      {
        if (reacts[RF.Left] < 2)
          x += velX;
      }
      else
      {
        if (reacts[RF.Right] < 2)
          x += velX;
      }

      if (velY >= 0)
      {
        if (reacts[RF.Up] < 2)
          y += velY;
      }
      else
      {
        if (reacts[RF.Down] < 2)
          y += velY;
      }


      this.velocity.X = velX;
      this.velocity.Y = velY;
      this.position.X = x;
      this.position.Y = y;




    }



    public void calculateReactionForce(WGame game, int newX, int newY, int dir)
    {

      reacts[dir] = 0;

      // newX should be x + velX at the first call

      for (int i = 0; i < colPointCount[dir]; ++i)
      {
        int colX = newX + colPoints[dir, i].x;
        int colY = newY + colPoints[dir, i].y;

        if (game.leveldata.getPixel(colX, colY) != PIXEL.empty)
        {
          ++reacts[dir];
        }
      }
    }


    void processMovement(WGame game, Command command)
    {

      var movable = true;

      if (movable)
      {
        bool left = (command.Buttons & Buttons.LEFT)==Buttons.LEFT;
        bool right = (command.Buttons & Buttons.RIGHT) == Buttons.RIGHT;
        bool jump = (command.Buttons & Buttons.JUMP) ==Buttons.JUMP;
        bool dig = (command.Buttons & Buttons.DIG ) == Buttons.DIG;
        bool ropeshoot = (command.Buttons & Buttons.ROPE) ==Buttons.ROPE;
        bool up = (command.Buttons & Buttons.UP) ==Buttons.UP;
        bool down = (command.Buttons & Buttons.DOWN) == Buttons.DOWN;

        if (left)
        {

          if (this.velocity.X >= -1.5)
          {
            this.velocity.X -= 1.5;
          }
        }

        if (right)
        {

          if (this.velocity.X <= 1.5)
          {
            this.velocity.X += 1.5;

          }
        }

        if (dig)
        {
          int digx = (int)(this.position.X + this.view.X * 12);
          int digy = (int)(this.position.Y + this.view.Y * 12);

          int iposx = (int)this.position.X;
          int iposy = (int)this.position.Y;

          game.leveldata.setPixels(digx, digy, 7, 0);
        }

        if (jump)
        {
          ninjarope.isout = false;
          ninjarope.attached = false;

          if ((reacts[RF.Up] > 0)
          && (ableToJump))
          {
            this.velocity.Y -= 4;
            ableToJump = false;
          }
        }
        else
          ableToJump = true;

        if (ninjarope.isout)
        {
          if (up)
            ninjarope.length -= 1;
          if (down)
            ninjarope.length += 1;

          if (ninjarope.length < 1)
            ninjarope.length = 1;

          if (ninjarope.length > 600)
            ninjarope.length = 600;

        }

        if (ropeshoot)
        {

          if (this.ableToRope)
          {

            this.ableToRope = false;

            ninjarope.isout = true;
            ninjarope.attached = false;

            ninjarope.x = this.position.X;
            ninjarope.y = this.position.Y;

            ninjarope.velX = this.view.X * 4;
            ninjarope.velY = this.view.Y * 4;

            ninjarope.length = 20;
          }

        }
        else
        {
          this.ableToRope = true;
        }

      }
    }



    public void Update(WGame game, Command command)
    {

      var newposition = new Vector2();
      var newvelocity = new Vector2();

      if (command.View.len > 0)
      {
        this.view = command.View;
        this.view.Normalize();
      }

      newposition = this.position + this.velocity;

      int iNextX = (int)newposition.X;
      int iNextY = (int)newposition.Y;

      for (int i = 0; i < 4; i++)
      {
        calculateReactionForce(game, iNextX, iNextY, i);

        // Yes, Liero does this in every iteration. Keep it this way.


        if (iNextX < 4)
        {
          reacts[RF.Right] += 5;
        }
        else if (iNextX > game.leveldata.width - 5)
        {
          reacts[RF.Left] += 5;
        }

        if (iNextY < 5)
        {
          reacts[RF.Down] += 5;
        }
        else
        {

          if (iNextY > game.leveldata.height - 6)
          {
            reacts[RF.Up] += 5;
          }
        }
      }

      if (reacts[RF.Down] < 2)
      {
        if (reacts[RF.Up] > 0)
        {
          if (reacts[RF.Left] > 0 || reacts[RF.Right] > 0)
          {
            //Low or none push down,
            //Push up and
            //Push left or right

            position.Y -= 1;
            newposition.Y = position.Y + velocity.Y;
            iNextY = (int)newposition.Y;

            calculateReactionForce(game, iNextX, iNextY, RF.Left);
            calculateReactionForce(game, iNextX, iNextY, RF.Right);
          }
        }
      }

      if (reacts[RF.Up] < 2)
      {
        if (reacts[RF.Down] > 0)
        {
          if (reacts[RF.Left] > 0 || reacts[RF.Right] > 0)
          {
            //Low or none push up,
            //Push down and
            //Push left or right

            position.Y += 1;
            newposition.Y = position.Y + velocity.Y;
            iNextY = (int)newposition.Y;

            calculateReactionForce(game, iNextX, iNextY, RF.Left);
            calculateReactionForce(game, iNextX, iNextY, RF.Right);
          }
        }
      }

      processWeapons(game);


      if ((command.Buttons & Buttons.SWITCH) == Buttons.SWITCH)
      {
        if (canSwitch)
        {
          activeWeapon++;
          activeWeapon %= Common.Weapons.Length;
          canSwitch = false;
        }
      }
      else
      {
        canSwitch = true;
      }

      if ((command.Buttons & Buttons.SHOOT) == Buttons.SHOOT && weapondelayleft <= 0)
      {
        fire(game);
      }

      processPhysics(game);
      processMovement(game, command);
      ninjarope.process(this, game);

    }

    private void processWeapons(WGame game)
    {
      if (this.weapondelayleft > 0)
      {
        --this.weapondelayleft;
      }
    }


    public bool ableToJump { get; set; }

    public void fire(WGame game)
    {

      this.weapondelayleft = 5;
      var w = Common.Weapons[activeWeapon];

      w.fire(game, this, this.position, this.view);

    }


    public static bool checkForSpecWormHit(WGame game, int x, int y, double radius, Worm worm)
    {
      double wormx = worm.position.X;
      double wormy = worm.position.Y;

      if (Math.Sqrt((wormx - x) * (wormx - x) + (wormy - y) * (wormy - y)) <= radius)
      {

        return true;


      }


      return false;

    }





  }



}