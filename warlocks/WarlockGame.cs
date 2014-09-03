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
        private List<Player> _playerlist;
        private List<AIController> _ailist;
        

        public WarlockGame()
        {
            this.newinputs = new ConcurrentQueue<PlayerInput>();
            this.players = new List<Player>();
            this.projectiles = new List<Projectile>();
            this.hubcontext = GlobalHost.ConnectionManager.GetHubContext<WarlocksHub>();

            _playerdictionary = new Dictionary<string, Player>();
            _playerlist = new List<Player>();
            _ailist = new List<AIController>();

            WebApiApplication.physicsTimer = new Timer(20); //was 40
            WebApiApplication.physicsTimer.Enabled = true;
            WebApiApplication.physicsTimer.Elapsed += new ElapsedEventHandler(Update);

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

            

            var len = _ailist.Count;

            if (len < 200)
            {
                AddAI();
                len++;
            }

            for (var i = 0; i<len; i++){
                _ailist[i].Think();
            }

            hubcontext.Clients.All.updateState(_playerlist.ToArray());
        }

        public void AddPlayer(string connectionid)
        {

            var temp = new Player();

            _playerdictionary[connectionid] = temp;

            _playerlist.Add(temp);


        }

        public void AddAI()
        {
            var v = new Vector2(RNG.next(1000), RNG.next(1000));
            var temp = new Player(v);
            _playerlist.Add(temp);
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
            this.position = new Vector2(500,500);
            this.view = new Vector2();
            this.id = nextid;
            nextid++;

        }
        public Player(Vector2 position)
        {
            this.position = position;
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

    public class AIController
    {
        static Random _r = new Random();
        private Player _player;

        public AIController(Player player)
        {
            _player = player;

        }

        public void Think()
        {
            //random for now

            var v1 = new Vector2(RNG.next(-5, 5), RNG.next(-5, 5));
            var v2 = new Vector2(RNG.next(1), RNG.next(1));
            
            var tempcommand = new Command(v2, v1);

            _player.Update(tempcommand);

        }

    }
}