using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Security.Cryptography;

using warlocks.Socket;
using warlocks.Game;

namespace warlocks
{
  class Program
  {

    public static ConcurrentQueue<Websocket> _connections = new ConcurrentQueue<Websocket>();

    static void Main(string[] args)
    {

      /*
      var testlevel = File.ReadAllBytes("person.png");
      var t = Convert.ToBase64String(testlevel);
      using (var writer = File.CreateText("person.txt"))
      {
        writer.Write(t);
      }
      */

      ServicePointManager.UseNagleAlgorithm = false;
      ServicePointManager.DefaultConnectionLimit = int.MaxValue;

      Task.Run(() => RunServer());

      var game = new WGame(_connections);

      game.Run();
    
    }

    static async Task RunServer()
    {
      var server = new WebsocketListener(1616);
      server.Start();
      int nextid = 0;

      while (true)
      {
        var client = await server.AcceptWebsocketClientAsync();
        AcceptWebsocket(client, nextid);
        ++nextid;

      }
    }

    static async Task AcceptWebsocket(WebsocketClient client, int nextid)
    {
      var ws = await client.UpgradeAsync();
      ws.Id = nextid;

      _connections.Enqueue(ws);

    }










    static async Task HandleConnection(Stream rw)
    {

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

      var testmsg = "hey buddy!";
      var bt = Encoding.UTF8.GetBytes(testmsg);
      var len = testmsg.Length;

      var testbytes = new byte[len + 2]; //{ (byte)129, (byte)testmsg.Length, bt };
      testbytes[0] = 129;
      testbytes[1] = (byte)len;
      Array.Copy(bt, 0, testbytes, 2, len);

      rw.Write(testbytes, 0, testbytes.Length);
      rw.Flush();
      Debug.WriteLine("done");

    }


  }
}
