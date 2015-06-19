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

    public List<NObject> bloodlist;

    private Dictionary<int, Worm> _playerdictionary;
    private BMAP _leveldata;

    public ObjectList<WObject> wormobjects;
    public bool leveldataready = false;
    private ConcurrentQueue<Websocket> _connections;

    public BMAP leveldata { get { return _leveldata; } }

    public IEnumerable<Worm> wormList
    {
      get
      {
        return _playerdictionary.Values;
      }
    }


    public WGame(ConcurrentQueue<Websocket> connections)
    {
      _connections = connections;
      _playerdictionary = new Dictionary<int, Worm>();
      _leveldata = new BMAP("testlevel.bmp");
      bloodlist = new List<NObject>();
      this.wormobjects = new ObjectList<WObject>(() => new WObject());

    }

    public void Update()
    {
      wormobjects.processNew();
      wormobjects.ProcessAll(this);

      for (var i = bloodlist.Count - 1; i >= 0; i--)
      {
        bloodlist[i].Process(this);
      }


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
          var runtime = new Stopwatch();
          runtime.Start();
          accumulator -= dt;
          t += dt;
          var en = _connections.GetEnumerator();
          
          while (en.MoveNext()){
            while (en.Current.Available() > 0)
            {
              var next = en.Current.ReadFrame();
              var command = Command.TryParse(next);
              if (command.Ok)
              {
                Worm player;

                if (!_playerdictionary.TryGetValue(en.Current.Id, out player))
                {
                  player = new Worm(this, en.Current.Id);
                  _playerdictionary.Add(en.Current.Id, player);
                }
                player.Update(this, command.Command);
              }

            }

            var worms = "[" + string.Join(",", wormList.Select(x => x.ToJson()).ToArray()) + "]";
            var blood = "[" + string.Join(",", bloodlist.Select(x => x.ToJson()).ToArray()) + "]";
            var objects = "[" + string.Join(",", wormobjects.getList().Select(x => x.ToJson()).ToArray()) + "]";
            var pixels = "[" + string.Join(",", leveldata.getDirtyPixels().Select(x => x.ToJson())) + "]";
            var update = "{\"worms\":" + worms + ",\"blood\":" + blood + ",\"objects\":" + objects + ",\"pixels\":" + pixels + "}";

            en.Current.Write(update);
          }

          wormobjects.processNew();
          wormobjects.ProcessAll(this);

          for (var i = bloodlist.Count - 1; i >= 0; i--)
          {
            bloodlist[i].Process(this);

          }

          runtime.Stop();
          Console.WriteLine(runtime.Elapsed.TotalMilliseconds);

        }
      }
    }

    public bool CheckLocation(int x, int y)
    {
      if (_leveldata.getColor(x, y) > 0)
      {
        return true;

      }
      return false;
    }



  }
}
