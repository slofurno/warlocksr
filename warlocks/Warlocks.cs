using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace warlocks
{
    public static class Warlocks
    {
        public static WarlockGame warlockgame = new WarlockGame();


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
        private double turnrate = 0.1;
        private float velocityx = 0;
        private float velocityy = 0;
        private float traveldistance;
        private float movespeed = 2;


        public Player()
        {

            this.id = nextid;
            nextid++;
            this.x = 50;
            this.y = 50;
            this.rotation = 0;
            this.velocity = 0;
            this.angularvelocity = 0;
            this.traveldistance = 0;
            

        }

        public void Update(PlayerInput input)
        {
            if (input.rmb > 0)
            {
                //this.x = input.mousex;
                //this.y = input.mousey;
                double temp;

       
                temp = Math.Atan2((input.mousey - this.y), (input.mousex - this.x));





                this.angularvelocity = temp - this.rotation;

                if (this.angularvelocity > Math.PI)
                {
                    this.angularvelocity = this.angularvelocity - 2*Math.PI;
                }
                else if (this.angularvelocity < -Math.PI)
                {
                    this.angularvelocity = this.angularvelocity + 2 * Math.PI;
                }

                this.velocityx = input.mousex - this.x;
                this.velocityy = input.mousey - this.y;

                this.traveldistance = (float)Math.Sqrt(Math.Pow((input.mousex - this.x), 2) + Math.Pow((input.mousey - this.y), 2));

               
            }


            if (Math.Abs(this.angularvelocity)<.05)
            {

                if (this.traveldistance > 0)
                {
                    this.x += (float)Math.Cos(this.rotation) * this.movespeed;
                    this.y += (float)Math.Sin(this.rotation) * this.movespeed;
                    this.traveldistance -= this.movespeed;
                }
            }
            else{

            
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

                    if (this.traveldistance > 0)
                    {
                        this.x += (float)Math.Cos(this.rotation) * (1 / 2) * this.movespeed;
                        this.y += (float)Math.Sin(this.rotation) * (1 / 2) * this.movespeed;
                        this.traveldistance -= (1 / 2) * this.movespeed;
                    }
                }
        }


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