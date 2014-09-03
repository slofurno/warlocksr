using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.AspNet.SignalR;


namespace warlocks
{
    public class WarlocksHub : Hub
    {

        private readonly WarlockGame _game;
        public WarlocksHub() : this(Warlocks.warlockgame) { }
        public WarlocksHub(WarlockGame game)
        {
            _game = game;
        }

        public void Hello()
        {
            Clients.All.hello();
        }

        public override Task OnConnected()
        {

            _game.AddPlayer(Context.ConnectionId);

            return base.OnConnected();
        }

        public void SendInput(Command command)
        {
            _game.ProcessCommand(Context.ConnectionId, command);

        }

        public void KeyDown(int id, int key)
        {


        }
    }
}