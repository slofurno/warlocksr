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

namespace warlocks
{
    public class WarlockGame
    {
        public ConcurrentQueue<PlayerInput> newinputs;
        public List<Player> players;
        public IHubContext hubcontext;
        public List<Projectile> projectiles;
        //private readonly static Lazy<WarlockGame> _instance = new Lazy<WarlockGame>(() => new WarlockGame());
        private Dictionary<string, Player> _playerdictionary;

        public WarlockGame()
        {
            this.newinputs = new ConcurrentQueue<PlayerInput>();
            this.players = new List<Player>();
            this.projectiles = new List<Projectile>();
            this.hubcontext = GlobalHost.ConnectionManager.GetHubContext<WarlocksHub>();

            _playerdictionary = new Dictionary<string, Player>();

            WebApiApplication.physicsTimer = new Timer(20); //was 40
            WebApiApplication.physicsTimer.Enabled = true;
            WebApiApplication.physicsTimer.Elapsed += new ElapsedEventHandler(Update);

        }

        public void Update(object source, ElapsedEventArgs e)
        {
            //foreach (PlayerInput q in newinputs)
            //for (var i = newinputs.Count-1; i>= 0; i--)

            var packetarray = new List<Player>();

            foreach (var pair in _playerdictionary)
            {
                Debug.WriteLine(pair.Value);
                packetarray.Add(pair.Value);
            }

            //hubcontext.Clients.All.updateProjectiles(projectiles.ToArray());
            hubcontext.Clients.All.updateState(packetarray.ToArray());


        }

        public void AddPlayer(string connectionid)
        {
            _playerdictionary[connectionid] = new Player();


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

                player.Update(command);

            }
            else
            {
                Debug.WriteLine("CANT FIND KEY");
            }
            
            

        }

        


    }

    public class Command
    {

        public Vector2 view { get; set; }
        public Vector2 velocity { get; set; }

        public Command(Vector2 view, Vector2 vel)
        {
            this.view = view;
            this.velocity = vel;

        }


    }

    public class Player
    {
        private static int nextid = 0;
        public Vector2 position { get; set; }
        public Vector2 view { get; set; }
        public int id { get; set; }

        public Player()
        {
            this.position = new Vector2();
            this.view = new Vector2();
            this.id = nextid;
            nextid++;

        }

        public void Update(Command command)
        {

            this.position = this.position + command.velocity;
            this.view = command.view;
            this.view.Normalize();


        }

    }
}