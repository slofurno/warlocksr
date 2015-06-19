using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warlocks.Game
{
  public class Vector2
  {
    double x;
    double y;
    double length;



    public double X
    {
      get { return x; }
      set { x = value; ComputeLength(); }
    }

    public double Y
    {
      get { return y; }
      set { y = value; ComputeLength(); }
    }

    public double len
    {
      get { return length; }
    }

    public Vector2(double X, double Y)
    {
      x = X;
      y = Y;

      ComputeLength();
    }

    public Vector2()
    {
      x = 0;
      y = 0;
      ComputeLength();

    }

    public string ToJson()
    {
      return "{\"X\":" + this.X + ",\"Y\":" + this.Y + "}";
    }


    void ComputeLength()
    {
      length = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
    }

    public Vector2 Normalize()
    {

      if (length == 0)
      {
        x = 0;
        y = 0;

      }
      else
      {

        x = x / length;
        y = y / length;
        ComputeLength();
      }

      return this;

    }

    public static Vector2 operator *(Vector2 v1, double x)
    {
      return new Vector2(v1.X * x, v1.Y * x);
    }
    public static Vector2 operator *(Vector2 v1, int x)
    {
      return new Vector2(v1.X * x, v1.Y * x);
    }

    public static Vector2 operator +(Vector2 v1, Vector2 v2)
    {
      return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
    }

  }
}
