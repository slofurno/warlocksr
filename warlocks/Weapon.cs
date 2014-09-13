using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace warlocks
{
    public class Weapon
    {


        public void fire(WarlockGame game, Worm owner, Vector2 position, Vector2 direction)
        {

            WObject obj = game.wormobjects.newObjectsReuse();

            obj.x = position.X;
            obj.y = position.Y;

            obj.velX = (3*direction.X);
            obj.velY = (3*direction.Y);
        }

    }

    public class WObject
    {

        public double x;
        public double y;
	    public double velX;
        public double velY;
        public int id;
	    int ownerIdx;
	    int curFrame;
	    int timeLeft;

        public int intX { get { return (int)this.x; } }
        public int intY { get { return (int)this.y; } }

        public void process(WarlockGame game)
        {

            y += velY;
            x += velX;


        }


    }

    public class ObjectList
    {
        private List<WObject> _objectlist;

        public ObjectList()
        {
            _objectlist = new List<WObject>();

        }


        public WObject newObjectsReuse()
        {

            var obj = new WObject();

            _objectlist.Add(obj);

            return obj;

        }

        public void ProcessAll(WarlockGame game)
        {

            for (int i = 0; i < _objectlist.Count; i++) {

                _objectlist[i].process(game);
            
            }

        }

        public object ToArray()
        {
            return this._objectlist.ToArray();
        }
    }




}