using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Timers;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Concurrent;
using System.IO;

namespace warlocks
{
    public class WarlockGame
    {
        public ConcurrentQueue<PlayerInput> newinputs;
        public List<Worm> players;
        public IHubContext hubcontext;
        public List<NObject> bloodlist;
        //private readonly static Lazy<WarlockGame> _instance = new Lazy<WarlockGame>(() => new WarlockGame());
        private Dictionary<string, Worm> _playerdictionary;
        public List<Worm> wormlist;
        private List<AIController> _ailist;
        private BMAP _leveldata;
        public bool leveldataready = false;
        public ObjectList<WObject> wormobjects;

        

        public BMAP leveldata { get { return _leveldata; } }
        

        public WarlockGame()
        {
            this.newinputs = new ConcurrentQueue<PlayerInput>();
            this.players = new List<Worm>();
            this.bloodlist = new List<NObject>();
            this.hubcontext = GlobalHost.ConnectionManager.GetHubContext<WarlocksHub>();
            this.wormobjects = new ObjectList<WObject>(()=>new WObject());

            _playerdictionary = new Dictionary<string, Worm>();
            wormlist = new List<Worm>();
            _ailist = new List<AIController>();

            WebApiApplication.physicsTimer = new Timer(20); //was 40
            WebApiApplication.physicsTimer.Enabled = true;
            WebApiApplication.physicsTimer.Elapsed += new ElapsedEventHandler(Update);
            _leveldata = new BMAP("D:/data/source/warlocks/warlocks/testlevel.bmp");

            //string txtPath = Path.Combine(Environment.CurrentDirectory, "testlevel.bmp");

            //_leveldata = new BMAP(txtPath);

        }

        public void Update(object source, ElapsedEventArgs e)
        {
            //foreach (PlayerInput q in newinputs)
            //for (var i = newinputs.Count-1; i>= 0; i--)
            /*
            var packetarray = new List<Player>();

            foreach (var pair in _playerdictionary)
            {
                Debug.WriteLine(pair.Value);
                packetarray.Add(pair.Value);
            }

            //hubcontext.Clients.All.updateProjectiles(projectiles.ToArray());
            hubcontext.Clients.All.updateState(packetarray.ToArray());

            */

            /*
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            */

            wormobjects.processNew();
            /*
            var temp = wormobjects.getList();

            for (int i = temp.Count - 1; i >= 0; i--)
            {

                temp[i].process(this);

            }
            */


            wormobjects.ProcessAll(this);

            //bloodlist.ForEach(b=>b.process(this));

            for (var i = bloodlist.Count - 1; i >= 0; i--)
            {
                bloodlist[i].Process(this);

            }


            hubcontext.Clients.All.updateState(wormlist.ToArray());
            hubcontext.Clients.All.updateObjects(this.wormobjects.ToArray());
            hubcontext.Clients.All.updateBlood(this.bloodlist.ToArray());

            sendPixels();

            /*
            stopwatch.Stop();
            double elap = ((double)stopwatch.ElapsedTicks/Stopwatch.Frequency);
            Debug.WriteLine("elpsed time : " + elap.ToString() + " seconds");
             * */

        }

        public void AddPlayer(string connectionid)
        {

            var temp = new Worm(this);

            _playerdictionary[connectionid] = temp;

            wormlist.Add(temp);


        }

        public void AddAI()
        {
            var v = new Vector2(RNG.next(1000), RNG.next(1000));
            var temp = new Worm(v, this);
            wormlist.Add(temp);
            _ailist.Add(new AIController(temp));

        }

        public void ProcessInput(PlayerInput input, double time)
        {
            //newinputs.Add(input);
            newinputs.Enqueue(input);
            //Debug.WriteLine(input.id + " has the q key pressed : " + input.q);

        }

        public void ProcessCommand(string connectionid, Command command)
        {

            //Debug.WriteLine("SOMETHING HAPPENING HERE");

            

            if (_playerdictionary.ContainsKey(connectionid)){
                var player = _playerdictionary[connectionid];

                command.velocity.Normalize();

                command.velocity = command.velocity * 2;

                player.Update(this, command);

            }
            else
            {
                Debug.WriteLine("CANT FIND KEY");
            }

           

        }

        public bool CheckLocation(Vector2 position)
        {

            if (_leveldata.getColor(position) == 0)
            {
                return true;

            }
            return false;
        }

        public bool CheckLocation(int x, int y)
        {

            //int temp = _leveldata.getColor(x, y);

            if (_leveldata.getColor(x, y) > 0)
            {
                return true;

            }
            return false;
        }





        public void sendPixels()
        {

            

            if (leveldataready)
            {

                

                if (leveldata.dirtypixellength > 0)
                {

                    Debug.WriteLine("updating some pixles");

                    //leveldata.dirtycounter = 0;
                    var pixelarray = leveldata.getDirtyPixels();
                    

                    hubcontext.Clients.All.updatePixels(pixelarray);
                }
            }

            

        }
    }

    public class Command
    {

        public Vector2 view { get; set; }
        public Vector2 velocity { get; set; }
        public int[] buttons { get; set; }

        public Command(Vector2 view, Vector2 vel, int[] buttons)
        {
            this.view = view;
            this.velocity = vel;
            this.buttons = buttons;

        }


    }

    

    public class AIController
    {
        static Random _r = new Random();
        private Worm _player;

        public AIController(Worm player)
        {
            _player = player;

        }

        public void Think()
        {
            //random for now

            var v1 = new Vector2(RNG.next(-5, 5), RNG.next(-5, 5));
            var v2 = new Vector2(RNG.next(1), RNG.next(1));
            
            //var tempcommand = new Command(v2, v1);

            //_player.Update(tempcommand);

        }

    }
}