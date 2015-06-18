using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace warlocks.Socket
{
  class WebsocketListener
  {
    private TcpListener _tcplistener;

    public WebsocketListener(int port)
    {
      _tcplistener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
    }

    public void Start()
    {
      _tcplistener.Start();
    }

    public async Task<WebsocketClient> AcceptWebsocketClientAsync()
    {
      var client = await _tcplistener.AcceptTcpClientAsync();

      return new WebsocketClient(client);
    }

  }
}
