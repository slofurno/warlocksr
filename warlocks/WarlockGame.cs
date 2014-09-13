﻿using System;
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
        public List<Projectile> projectiles;
        //private readonly static Lazy<WarlockGame> _instance = new Lazy<WarlockGame>(() => new WarlockGame());
        private Dictionary<string, Worm> _playerdictionary;
        private List<Worm> _playerlist;
        private List<AIController> _ailist;
        private BMAP _leveldata;

        public ObjectList wormobjects;

        

        public BMAP leveldata { get { return _leveldata; } }
        

        public WarlockGame()
        {
            this.newinputs = new ConcurrentQueue<PlayerInput>();
            this.players = new List<Worm>();
            this.projectiles = new List<Projectile>();
            this.hubcontext = GlobalHost.ConnectionManager.GetHubContext<WarlocksHub>();
            this.wormobjects = new ObjectList();

            _playerdictionary = new Dictionary<string, Worm>();
            _playerlist = new List<Worm>();
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

            

            var len = _ailist.Count;

            if (len < 200)
            {
                //AddAI();
                len++;
            }

            for (var i = 0; i<len; i++){
                //_ailist[i].Think();
            }


            wormobjects.ProcessAll(this);


            hubcontext.Clients.All.updateState(_playerlist.ToArray());
            hubcontext.Clients.All.updateObjects(this.wormobjects.ToArray());
        }

        public void AddPlayer(string connectionid)
        {

            var temp = new Worm(this);

            _playerdictionary[connectionid] = temp;

            _playerlist.Add(temp);


        }

        public void AddAI()
        {
            var v = new Vector2(RNG.next(1000), RNG.next(1000));
            var temp = new Worm(v, this);
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

            var stopwatch = new Stopwatch();
            stopwatch.Start();

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

            stopwatch.Stop();
            var elap = stopwatch.ElapsedTicks;
            //Debug.WriteLine("elpsed time : " + elap.ToString());

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

            if (_leveldata.getColor(x, y) > 0)
            {
                return true;

            }
            return false;
        }





        internal void sendPixels(List<pixel> temp)
        {

            Debug.WriteLine("setting some pixles");

            hubcontext.Clients.All.updatePixels(temp.ToArray());

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