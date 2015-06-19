using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warlocks.Game
{

  public class Buttons
  {
    
    public static readonly int UP = 1;
    public static readonly int DOWN = 2;
    public static readonly int LEFT = 4;
    public static readonly int RIGHT = 8;
    public static readonly int SHOOT = 16;
    public static readonly int JUMP = 32;
    public static readonly int ROPE = 64;
    public static readonly int DIG = Buttons.LEFT | Buttons.RIGHT;

  }

  public class CommandResult
  {
    public Command Command { get; set; }
    public bool Ok { get; set; }
  }

  public class Command
  {


    public Vector2 View { get; set; }
    public int Buttons { get; set; }

    public static CommandResult TryParse(string ps)
    {
      var s = ps.Split('|');
      double x;
      double y;
      int buttons;

      if (s.Length == 3&&double.TryParse(s[0],out x)&&double.TryParse(s[1], out y)&&int.TryParse(s[2],out buttons))
      {
        var view = new Vector2(x,y);
        var command = new Command() { Buttons = buttons, View = view };
        return new CommandResult() { Command = command, Ok = true };
      }
      else
      {
        return new CommandResult() { Command = null, Ok = false };

      }

    }
  }
}
