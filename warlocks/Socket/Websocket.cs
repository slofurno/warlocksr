using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace warlocks.Socket
{
  public class Websocket
  {

    private Stream _rw;
    private TcpClient _client;

    //TODO:move these
    public int Id { get; set; }
    public int LastUpdate { get; set; }

    public Websocket(TcpClient client, Stream rw)
    {
      _client = client;
      _rw = rw;
    }

    public void Write(string s)
    {
      const int headersize = 10;
      var bytes = Encoding.UTF8.GetBytes(s);
      var len = bytes.Length;

      var buffer = new byte[len + headersize];
      buffer[0] = 129;
      buffer[1] = 127;

      buffer[2] = 0;
      buffer[3] = 0;
      buffer[4] = 0;
      buffer[5] = 0;

      buffer[6] = 0;
      buffer[7] = (byte)((len >> 16) & 255);
      buffer[8] = (byte)((len >> 8) & 255);
      buffer[9] = (byte)(len & 255);

      Array.Copy(bytes, 0, buffer, headersize, len);

      _rw.Write(buffer, 0, buffer.Length);
      _rw.Flush();

    }

    public string ReadFrame()
    {
      int read;

      var header = new byte[6];
      read = _rw.Read(header, 0, 6);

      if (read != 6)
      {
        var wtf = "m8";
      }

      var opcode = header[0] & 15;
	    var length = header[1] & 127;

	    if (opcode == 8) {
		    //closing
	    }

      var body = new byte[4096];
      read = _rw.Read(body, 0, length);

      if (read != length)
      {
        var wtf = "m8";
      }

      for (var i = 0; i < length; i++)
      {
        body[i] ^= header[2 + (i & 3)];
      }

      return Encoding.UTF8.GetString(body,0,length);
    }

    public int Available()
    {
      return _client.Available;
    }
  }
}
