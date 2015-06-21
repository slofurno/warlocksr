using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;

namespace warlocks.Game
{


  public static class Common
  {
    public static Weapon[] Weapons { get; set; }

    static Common()
    {
      Weapons = new Weapon[]{
        new Weapon(){ 
          Bounce=60, 
          timeToExplosion=200, 
          Id=0, 
          explosionRadius=20,
          gravity=.05,
          splinterAmount=100,
          velocity=2
        },
        new Weapon(){
          Bounce=0,
          Id=1, 
          explosionRadius=3, 
          gravity=0,
          velocity=5
        },
        new Weapon(){ 
          Bounce=0, 
          timeToExplosion=40, 
          Id=2, 
          explosionRadius=20,
          gravity=.02,
          splinterAmount=6,
          velocity=4
        },

      };
    }

  }

  public class Weapon
  {

    public int Bounce { get; set; }
    public int Splinters { get; set; }
    public int Id { get; set; }
    public int timeToExplosion { get; set; }
    public int explosionRadius { get; set; }
    public double gravity { get; set; }
    public int splinterAmount { get; set; }
    public double velocity { get; set; }

    public void fire(WGame game, Worm owner, Vector2 position, Vector2 direction)
    {
      WObject obj = game.wormobjects.newObjectsReuse();

      obj.x = position.X;
      obj.y = position.Y;

      obj.velX = (this.velocity * direction.X);
      obj.velY = (this.velocity * direction.Y);

      obj.firedbyid = owner.id;
      obj.id = this.Id;
      obj.timeLeft = this.timeToExplosion;
    }
  }

}