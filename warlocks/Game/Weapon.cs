using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;

namespace warlocks.Game
{
  public class Weapon
  {

    public void fire(WGame game, Worm owner, Vector2 position, Vector2 direction, int id)
    {
      WObject obj = game.wormobjects.newObjectsReuse();

      obj.x = position.X;
      obj.y = position.Y;

      obj.velX = (5 * direction.X);
      obj.velY = (5 * direction.Y);
      
      obj.firedbyid = id;
    }
  }

}