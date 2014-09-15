using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;

namespace warlocks
{
    public class Weapon
    {


        public void fire(WarlockGame game, Worm owner, Vector2 position, Vector2 direction, int id)
        {

            WObject obj = game.wormobjects.newObjectsReuse();

            obj.x = position.X;
            obj.y = position.Y;

            obj.velX = (3*direction.X);
            obj.velY = (3*direction.Y);

            //obj.firedbyid = id;
        }

    }

    public interface IProcessable
    {
        void Process(WarlockGame game);
    }

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

            double angle = RNG.next(Math.PI*2);

            this.velX = Math.Cos(angle) * 1.5 + velX;
            this.velX = Math.Sin(angle) * 1.5 + velY;


        }

        

        public void Process(WarlockGame game)
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
            else if (game.leveldata.getColor(ix, iy) == (int)PIXEL.rock)
            {
                doExplode = true;

            }


           


            if (doExplode)
            {
                game.bloodlist.Remove(this);
            }


        }

    }


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
        int firedbyid;

        public int intX { get { return (int)this.x; } }
        public int intY { get { return (int)this.y; } }

        public void Process(WarlockGame game)
        {

            bool doExplode = false;

            y += velY;
            x += velX;

            int ix = (int) x;
            int iy = (int) y;

            if (!game.leveldata.inside(ix, iy))
            {
                doExplode = true;

            }


            if (game.leveldata.getPixel(ix, iy) == PIXEL.empty)
            {
                game.leveldata.setPixel2(ix, iy, PIXEL.dirt);
                doExplode = true;

            }


            
            game.wormlist.ForEach(worm=>{

                if (Worm.checkForSpecWormHit(game, ix, iy, 1.5, worm))
                {

                    //create some bloods
                    for (var i = 0; i < 32; i++)
                    {
                        game.bloodlist.Add(new NObject(ix, iy, velX/3, velY/3));
                    }

                        doExplode = true;
                }
            
            
            });


            if (doExplode)
            {
                game.wormobjects.free(this);
            }
            

        }

        private bool checkForSpecWormHit5(WarlockGame game, int x, int y, int radius, Worm worm)
        {
            double wormx = worm.position.X;
            double wormy = worm.position.Y;

            if (Math.Sqrt((wormx - x)*(wormx - x) + (wormy - y)*(wormy - y)) <= radius)
            {

                return true;


            }


            return false;
            
        }



        
    }

    public class ObjectList<T> where T : IProcessable
    {
        private ConcurrentQueue<T> _objectqueue;
        private List<T> _objectlist;
        private Func<T> _objectcreator;

        public ObjectList(Func<T> func)
        {
            _objectqueue = new ConcurrentQueue<T>();
            _objectlist = new List<T>();
            _objectcreator = func;

        }


        public T newObjectsReuse()
        {

            var obj = this.CreateInstance();

            _objectqueue.Enqueue(obj);

            return obj;

        }

        public void processNew()
        {

            T temp;

            while (_objectqueue.TryDequeue(out temp))
            {
                _objectlist.Add(temp);

            }

        }

        public List<T> getList()
        {
            return _objectlist;
        }

        public void free(T obj)
        {

            _objectlist.Remove(obj);
        }

        public void ProcessAll(WarlockGame game)
        {

            for (int i = _objectlist.Count-1; i >=0; i--)
            {

                _objectlist[i].Process(game);
            
            }

        }

        public T CreateInstance()
        {

            return _objectcreator();

        }

        public object ToArray()
        {
            return this._objectlist.ToArray();
        }
    }




}