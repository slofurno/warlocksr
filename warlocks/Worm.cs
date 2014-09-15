using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace warlocks
{

    struct Point
    {
        public int x, y;

        public Point(int x, int y){
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
        private WarlockGame _world;
        public Vector2 velocity { get; set; }
        private int[] reacts = new int[4];
        private Weapon tempweapon = new Weapon();

        public int weapondelayleft { get; set; }

        public Worm(WarlockGame world)
        {
            this.position = new Vector2(600, 300);
            this.view = new Vector2();
            this.id = nextid;
            nextid++;
            velocity = new Vector2();
            _world = world;

        }
        public Worm(Vector2 position, WarlockGame world)
        {
            this.position = position;
            this.view = new Vector2();
            this.id = nextid;
            nextid++;
            velocity = new Vector2();
            _world = world;

        }

        public static class RF
        {
            public const int Down = 0;
            public const int Left = 1;
            public const int Up = 2;
            public const int Right = 3;
            
        }

        

        static Point[,] colPoints = new Point[4,7]
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

        public void processPhysics(WarlockGame game)
        {
        //Common& common = *game.common;

            int velX = (int)this.velocity.X;
            int velY = (int)this.velocity.Y;

            double x = this.position.X;
            double y = this.position.Y;

            if(reacts[RF.Up] > 0)
            {
                velX = (velX/2);
            }
       
            if(velX > 0)
            {
                    if(reacts[RF.Left] > 0)
                    {
                            if(velX > 3)
                            {
                                    
                                    velX = -velX / 3;
                            }
                            else
                                    velX = 0;
                    }
            }
            else if(velX < 0)
            {
                    if(reacts[RF.Right]>0)
                    {
                            if(velX < -3)
                            {
                                  
                                    velX = -velX / 3;
                            }
                            else
                                    velX = 0;
                    }
            }
       
            if(velY > 0)
            {
                    if(reacts[RF.Up] > 0)
                    {
                            if(velY > 3)
                            {
                                  
                                    velY = -velY / 3;
                            }
                            else
                                    velY = 0;
                    }
            }
            else if(velY < 0)
            {
                    if(reacts[RF.Down]>0)
                    {
                            if(velY < -3)
                            {
                                    
                                    velY = -velY / 3;
                            }
                            else
                                    velY = 0;
                    }
            }
       
            if(reacts[RF.Up] == 0)
            {
                    velY += 1; //GRAVITY
            }
       
            if(velX >= 0)
            {
                    if(reacts[RF.Left] < 2)
                            x += velX;
            }
            else
            {
                    if(reacts[RF.Right] < 2)
                            x += velX;
            }
       
            if(velY >= 0)
            {
                    if(reacts[RF.Up] < 2)
                            y += velY;
            }
            else
            {
                    if(reacts[RF.Down] < 2)
                            y += velY;
            }


            this.velocity.X = velX;
            this.velocity.Y = velY;
            this.position.X = x;
            this.position.Y = y;

            


        }


        public void calculateReactionForce(WarlockGame game, int newX, int newY, int dir)
        {
        



            reacts[dir] = 0;
       
            // newX should be x + velX at the first call
       
            for(int i = 0; i < colPointCount[dir]; ++i)
            {
                int colX = newX + colPoints[dir,i].x;
                int colY = newY + colPoints[dir,i].y;
               
            

                    //if(game.CheckLocation(colX, colY))
                if (game.leveldata.getPixel(colX, colY) != PIXEL.empty)
                {
                        ++reacts[dir];
                }
            }
        }



        void processMovement(WarlockGame game, Command command)
        {
       
            var movable = true;

            if(movable)
            {
                bool left = (command.velocity.X < 0);
                bool right = (command.velocity.X > 0);
                bool jump = (command.velocity.Y < 0);
                bool dig = (command.buttons[0]>0);

                if (left) {
                
                    if (this.velocity.X >= -2)
                    {
                        this.velocity.X -= 2;


                    }

                }

                if (right)
                {

                    if (this.velocity.X <= 2)
                    {
                        this.velocity.X += 2;


                    }

                }


                //dig time
                if (dig){
                    int digx = (int)(this.position.X + this.view.X * 12);
                    int digy = (int)(this.position.Y + this.view.Y * 12);

                    Debug.WriteLine(this.view.X * 12 + ", " + this.view.Y * 12);

                    int iposx = (int)this.position.X;
                    int iposy = (int)this.position.Y;

                    game.leveldata.setPixels(digx, digy, 7, 0, game);



                }
               
                //this.velocity = this.velocity + command.velocity;
               

                if(jump)
		        {
			        //ninjarope.out = false;
			        //ninjarope.attached = false;
			
			        if((reacts[RF.Up] > 0)
			        && (ableToJump))
			        {
				        this.velocity.Y -= 8;
				        ableToJump = false;
			        }
		        }
		        else
			        ableToJump = true;
                    
               
                
            }
        }



        public void Update(WarlockGame game, Command command)
        {

            var newposition = new Vector2();
            var newvelocity = new Vector2();

            if (command.view.len > 0)
            {
                this.view = command.view;
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

            if (command.buttons[4]>0 && weapondelayleft<=0){
         
                fire(game);
            
            }


            processPhysics(game);

            processMovement(game, command);


 




        }

        private void processWeapons(WarlockGame game)
        {
            if (this.weapondelayleft > 0)
            {
                --this.weapondelayleft;
            }
        }


        public bool ableToJump { get; set; }

        public void fire(WarlockGame game)
        {

            this.weapondelayleft = 5;

            tempweapon.fire(game, this, this.position, this.view, this.id);

        }


        public static bool checkForSpecWormHit(WarlockGame game, int x, int y, double radius, Worm worm)
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