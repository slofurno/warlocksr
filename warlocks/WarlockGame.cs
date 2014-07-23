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

        public WarlockGame()
        {
            this.newinputs = new ConcurrentQueue<PlayerInput>();
            this.players = new List<Player>();

            this.hubcontext = GlobalHost.ConnectionManager.GetHubContext<WarlocksHub>();

            WebApiApplication.physicsTimer = new Timer(20); //was 40
            WebApiApplication.physicsTimer.Enabled = true;
            WebApiApplication.physicsTimer.Elapsed += new ElapsedEventHandler(Update);

        }

        public void Update(object source, ElapsedEventArgs e)
        {
            //foreach (PlayerInput q in newinputs)
            //for (var i = newinputs.Count-1; i>= 0; i--)
            while (newinputs.Count>0)
            {
                PlayerInput currentelement;
                bool success = newinputs.TryDequeue(out currentelement);

                if (success)
                {
                    var temp = players.Find(x => x.id == currentelement.id);

                    //var temp = players.Find(x => x.id == q.id);

                    if (temp != null)
                    {
                        temp.Update(currentelement);
                        //newinputs.RemoveAt(i);

                    }
                }

            }



            hubcontext.Clients.All.updateState(players.ToArray());


        }

        public int AddPlayer()
        {
            Player tempplayer = new Player();
            players.Add(tempplayer);

            return tempplayer.id;


        }

        public void ProcessInput(PlayerInput input)
        {
            //newinputs.Add(input);
            newinputs.Enqueue(input);
            Debug.WriteLine(input.id + " has the q key pressed : " + input.q);

        }


    }
}