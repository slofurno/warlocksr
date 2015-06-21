using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warlocks.Game
{

  public static class Buttons
  {
    
    public const int UP = 1;
    public const int DOWN = 2;
    public const int LEFT = 4;
    public const int RIGHT = 8;
    public const int SHOOT = 16;
    public const int JUMP = 32;
    public const int ROPE = 64;
    public const int DIG = Buttons.LEFT | Buttons.RIGHT;
    public const int SWITCH = 128;

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
