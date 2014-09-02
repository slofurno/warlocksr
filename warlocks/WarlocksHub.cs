using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace warlocks
{
    public class WarlocksHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void StartPlayer()
        {

            Clients.Caller.setId(Warlocks.warlockgame.AddPlayer());

        }

        public void SendInput(PlayerInput someinput)
        {
            Warlocks.warlockgame.ProcessInput(someinput);

        }

        public void KeyDown(int id, int key)
        {


        }
    }
}