using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warlocks.Socket;

namespace warlocks.Game
{
  public class WGame
  {

    private ConcurrentQueue<Websocket> _connections;

    public WGame(ConcurrentQueue<Websocket> connections)
    {
      _connections = connections;
    }

    public void Run()
    {
      var sw = new Stopwatch();
      sw.Start();

      const double dt = 1000D / 60;
      double t = 0;
      double accumulator = 0;

      while (true)
      {
        var elapsed = sw.Elapsed.TotalMilliseconds;
        sw.Restart();
        accumulator += elapsed;

        while (accumulator >= dt)
        {
          accumulator -= dt;
          t += dt;
          var en = _connections.GetEnumerator();
          
          while (en.MoveNext()){
            if (en.Current.Available() > 0)
            {
              Console.WriteLine(en.Current.ReadFrame());
            }
          }
        }


      }
    }

  }
}
