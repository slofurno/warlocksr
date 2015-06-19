using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows;
using System.Diagnostics;

namespace warlocks.Game
{

  public static class RNG
  {
    static Random _r = new Random();

    public static double next(double max)
    {

      return (max * _r.NextDouble());
    }

    public static double next(double min, double max)
    {

      return (((max - min) * _r.NextDouble()) + min);
    }

  }


  public class Projectile
  {
    private static int nextid = 0;
    public int id;

    public float _velocity;
    private float radius;
    private float range;
    public Vector2 direction;
    public Vector2 position;
    private Worm owner;
    public Vector2 velocity;

    public Projectile(Vector2 Position, Vector2 Direction, Worm Owner)
    {
      this.id = nextid;
      nextid++;
      this._velocity = 6;
      this.radius = 30;
      this.range = 300;
      this.direction = Direction;
      this.position = Position;
      this.owner = Owner;
      this.velocity = this.direction * this._velocity;
    }

  }


}