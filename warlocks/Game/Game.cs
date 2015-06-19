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
    private List<Pixel>[] _history;

    public ObjectList<WObject> wormobjects;
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
      _history = new List<Pixel>[32];

      for (int i = 0; i < 32; i++)
			{
        _history[i] = new List<Pixel>();
			}

       

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
      int frame = 0;

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

            if (en.Current.Available() == 0)
            {
              continue;
            }

            var last = en.Current.LastUpdate;

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

            var missed = new List<Pixel>();

            if ((frame - last) > 31)
            {
              missed = leveldata._alldirtypixels;
            }
            else
            {
               for (var i = last; i < frame; i++)
            {
              missed.AddRange(_history[i & 31]);
            }
            }
           
            
            var worms = "[" + string.Join(",", wormList.Select(x => x.ToJson()).ToArray()) + "]";
            var blood = "[" + string.Join(",", bloodlist.Select(x => x.ToJson()).ToArray()) + "]";
            var objects = "[" + string.Join(",", wormobjects.getList().Select(x => x.ToJson()).ToArray()) + "]";
            var pixels = "[" + string.Join(",", missed.Select(x => x.ToJson())) + "]";
            var update = "{\"worms\":" + worms + ",\"blood\":" + blood + ",\"objects\":" + objects + ",\"pixels\":" + pixels + "}";

            en.Current.LastUpdate = frame;
            en.Current.Write(update);
          }

          wormobjects.processNew();
          wormobjects.ProcessAll(this);

          for (var i = bloodlist.Count - 1; i >= 0; i--)
          {
            bloodlist[i].Process(this);

          }

          var dirtypixels = leveldata.getDirtyPixels();
          var index = frame & 31;
          _history[index].Clear();
          _history[index].AddRange(dirtypixels);

          runtime.Stop();
          Console.WriteLine(runtime.Elapsed.TotalMilliseconds);
          ++frame;

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
