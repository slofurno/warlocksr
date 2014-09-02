using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows;
using System.Diagnostics;

namespace warlocks
{
    public static class Warlocks
    {
        public static WarlockGame warlockgame = new WarlockGame();


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
        private Player owner;
        public Vector2 velocity;

        public Projectile(Vector2 Position, Vector2 Direction, Player Owner)
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

        public Vector2(double X, double Y)
		{
			x = X;
			y = Y;
			
			ComputeLength();
		}


        void ComputeLength()
        {
            length = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        void Normalize()
        {

            x = x / length;
            y = y / length;
            ComputeLength();
        }

        public static Vector2 operator *(Vector2 v1, double x)
        {
            return new Vector2(v1.X * x, v1.Y * x);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

    }


    public class Player
    {

        public static int nextid = 0;
        public int id;
        public float x;
        public float y;
        public double rotation;
        public float velocity;
        public double angularvelocity;
        private double turnrate = 0.5;
        private float velocityx = 0;
        private float velocityy = 0;
        private float traveldistance;
        private float movespeed = 3;
        private double desiredrotation;
        private int selectedability;
        private int castability;
        private WarlockGame warlockgame;

        public Player(WarlockGame game)
        {

            this.id = nextid;
            nextid++;
            this.x = 50;
            this.y = 50;
            this.rotation = 0;
            this.velocity = 0;
            this.angularvelocity = 0;
            this.traveldistance = 0;
            this.desiredrotation = 0;
            this.selectedability = 0;
            this.castability = 0;
            this.warlockgame = game;
        }

        public void TurnTowards(Vector2 point)
        {


        }

        public void MoveTowards(Vector2 point)
        {
            double temp;


            temp = Math.Atan2((point.Y - this.y), (point.X - this.x));





            this.angularvelocity = temp - this.rotation;

            if (this.angularvelocity > Math.PI)
            {
                this.angularvelocity = this.angularvelocity - 2 * Math.PI;
            }
            else if (this.angularvelocity < -Math.PI)
            {
                this.angularvelocity = this.angularvelocity + 2 * Math.PI;
            }

            this.velocityx = (float)point.X - this.x;
            this.velocityy = (float)point.Y - this.y;

            this.traveldistance = (float)Math.Sqrt(Math.Pow((point.X - this.x), 2) + Math.Pow((point.Y - this.y), 2));

            this.desiredrotation = temp;

        }

        public void ProcessInputs(PlayerInput input)
        {

            if (input.q > 0)
            {

                this.selectedability = 1;

            }

            if (input.lmb > 0)
            {
                //turn towards point, walk towards point
                //cast fireball
                if (this.selectedability > 0)
                {

                    this.castability = 1;
                    MoveTowards(new Vector2(input.mousex, input.mousey));

                }

            }

            if (input.rmb > 0)
            {

                //cancel all moves
                this.castability = 0;
               
                double temp;


                temp = Math.Atan2((input.mousey - this.y), (input.mousex - this.x));





                this.angularvelocity = temp - this.rotation;

                Debug.WriteLine("temp  " + temp + "  rot + " + this.rotation);

                if (this.angularvelocity > Math.PI)
                {
                    this.angularvelocity = this.angularvelocity - 2 * Math.PI;
                }
                else if (this.angularvelocity < -Math.PI)
                {
                    this.angularvelocity = this.angularvelocity + 2 * Math.PI;
                }

                this.velocityx = input.mousex - this.x;
                this.velocityy = input.mousey - this.y;

                this.traveldistance = (float)Math.Sqrt(Math.Pow((input.mousex - this.x), 2) + Math.Pow((input.mousey - this.y), 2));

                this.desiredrotation = temp;

            }



        }

        public void Update()
        {
            


           

            if (this.traveldistance > 0)
            {
                this.x += (float)Math.Cos(this.desiredrotation) * this.movespeed;
                this.y += (float)Math.Sin(this.desiredrotation) * this.movespeed;
                this.traveldistance -= this.movespeed;
            }
         

            
            if(this.angularvelocity >= this.turnrate)
            {
                this.rotation += this.turnrate;
                this.angularvelocity -= this.turnrate;

            }
            else if (this.angularvelocity < -this.turnrate)
            {
                this.rotation += -this.turnrate;
                this.angularvelocity -= -this.turnrate;
            }
            else
            {
                this.rotation += this.angularvelocity;
                this.angularvelocity = 0;
            }


            if (this.castability>0 && this.angularvelocity==0)
            {
                //cast fireball
                this.warlockgame.projectiles.Add(new Projectile(new Vector2(this.x, this.y), new Vector2(Math.Cos(this.rotation), Math.Sin(this.rotation)), this));
                this.selectedability = 0;
                this.castability = 0;
            }

            this.rotation %= (2*Math.PI);
       

        }


    }

    public class Ability
    {
        public string name;
        


    }

    public class PlayerInput
    {

        public int id { get; set; }
        public float mousex { get; set; }
        public float mousey { get; set; }
        public int rmb { get; set; }
        public int lmb { get; set; }
        public int q { get; set; }
        public int w { get; set; }
        public int e { get; set; }
        public int r { get; set; }

        public PlayerInput()
        {
            this.id = -1;
            this.mousex = 0;
            this.mousey = 0;
            this.rmb = 0;
            this.lmb = 0;
            this.q = 0;
            this.w = 0;
            this.e = 0;
            this.r = 0;
        }

    }
}