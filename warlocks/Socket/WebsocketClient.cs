using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace warlocks.Socket
{
  class WebsocketClient
  {
    private TcpClient _client;

    public WebsocketClient(TcpClient client)
    {
      _client = client;
    }

    public async Task<Websocket> UpgradeAsync()
    {
      var rw = _client.GetStream();
      

      var reader = new StreamReader(rw);
      var writer = new StreamWriter(rw);

      string next;
      var headers = new Dictionary<string, string>();
      var lines = new List<string>();
      var count = 0;

      while ((next = await reader.ReadLineAsync()) != null && next != "")
      {
        if (count > 0)
        {
          var header = next.Split(':');
          headers[header[0]] = header[1].Trim();
        }
        count++;
      }

      var guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
      var key = headers["Sec-WebSocket-Key"];

      var s = key + guid;
      var b = Encoding.UTF8.GetBytes(s);
      string challengeresponse = "";

      using (SHA1Managed sha1 = new SHA1Managed())
      {
        var hash = sha1.ComputeHash(b);
        challengeresponse = Convert.ToBase64String(hash);
      }

      writer.Write("HTTP/1.1 101 Switching Protocols\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Accept: " + challengeresponse + "\r\n\r\n");
      writer.Flush();

      return new Websocket(_client,rw);
    }
  }
}
