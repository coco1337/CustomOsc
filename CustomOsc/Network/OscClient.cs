using System.Net;
using System.Net.Sockets;

namespace CustomOsc.Network;

internal sealed class OscClient
{
  private const string IP_ADDR = "127.0.0.1";
  private const int SEND_PORT = 9000;
  private IPEndPoint destination;

  private Socket udpSocket;

  private byte[] byteArray;

  private int writePos = 19;

  private void ResetWritePos() => writePos = 19;

  internal bool IsConnected => this.udpSocket.Connected;

  public void Init()
  {
    if (!ConnectToServer()) return;

    this.byteArray = GC.AllocateArray<byte>(4096, true);

    // /avatar/parameters/
    this.byteArray[0] = 0x2F;
    this.byteArray[1] = 0x61;
    this.byteArray[2] = 0x76;
    this.byteArray[3] = 0x61;
    this.byteArray[4] = 0x74;
    this.byteArray[5] = 0x61;
    this.byteArray[6] = 0x72;
    this.byteArray[7] = 0x2F;
    this.byteArray[8] = 0x70;
    this.byteArray[9] = 0x61;
    this.byteArray[10] = 0x72;
    this.byteArray[11] = 0x61;
    this.byteArray[12] = 0x6D;
    this.byteArray[13] = 0x65;
    this.byteArray[14] = 0x74;
    this.byteArray[15] = 0x65;
    this.byteArray[16] = 0x72;
    this.byteArray[17] = 0x73;
    this.byteArray[18] = 0x2F;
  }

  public bool ConnectToServer()
  {
    try
    {
      this.udpSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
      var ipAddr = IPAddress.Parse(IP_ADDR);
      this.destination = new IPEndPoint(ipAddr, SEND_PORT);

      this.udpSocket.Connect(this.destination);

      return true;
    }
    catch (Exception exception)
    {
      Console.WriteLine(exception.ToString());
      return false;
    }
  }

  public void Send(string address, int val)
  {
    Write(address);
    AddZero(1);

    this.byteArray[writePos++] = 0x2C;
    this.byteArray[writePos++] = 0x69;
    AddZero(2);
    Write(val);

    Send(new ReadOnlySpan<byte>(this.byteArray, 0, writePos));
    ResetWritePos();
  }

  public void Send(string address, bool b)
  {
    Write(address);
    AddZero(4);
    this.byteArray[writePos++] = 0x2C;
    if (b) this.byteArray[writePos++] = 0x54;
    else this.byteArray[writePos++] = 0x46;
    this.byteArray[writePos++] = 0x00;
    this.byteArray[writePos++] = 0x00;

    Send(new ReadOnlySpan<byte>(this.byteArray, 0, writePos));
    ResetWritePos();
  }

  // send data via socket
  private void Send(ReadOnlySpan<byte> sendData)
  {
    try
    {
      var oscMessage = new OscCore.OscMessage("/avatar/parameters/test", 1).ToByteArray();
      var oscMessage2 = new OscCore.OscMessage("/avatar/parameters/test", 10000).ToByteArray();
      var oscMessage3 = new OscCore.OscMessage("/avatar/parameters/test", 1000000000).ToByteArray();
      this.udpSocket.SendTo(sendData, this.destination);  
    } 
    catch(Exception exception)
    {
      
    }
  }

  #region Write
  // TODO : matching with OscTypes
  private void Write(int data)
  {
    this.byteArray[writePos++] = (byte)(data >> 24);
    this.byteArray[writePos++] = (byte)(data >> 16);
    this.byteArray[writePos++] = (byte)(data >> 8);
    this.byteArray[writePos++] = (byte)data;
  }

  private void Write(string data)
  {
    foreach (var ch in data)
    {
      this.byteArray[writePos++] = (byte)ch;
    }

    var alignedLength = (data.Length + 3) & ~3;
    if (alignedLength == data.Length) alignedLength += 4;

    for (; writePos < alignedLength; ++writePos)
    {
      this.byteArray[writePos] = 0;
    }
  }

  private void AddZero(int count)
  {
    for (int i = 0; i < count; ++i)
      this.byteArray[writePos++] = 0x00;
  }
  #endregion
} 

/*
internal static class OscTag
{
  internal static readonly byte[] OscTypes =
  {
    0x2C, 0x69, 0x20, // ",i " int32
    0x2C, 0x66, 0x20, // ",f " float32
    0x2C, 0x74, 0x20, // ",s " string
    0x2C, 0x62, 0x20, // ",b " blob
    0x2C, 0x64, 0x20, // ",d " int64
    0x2C, 0x68, 0x20, // ",h " float64
    0x2C, 0x72, 0x20, // ",r " color32
    0x2C, 0x6D, 0x20, // ",m " midi
    0x2C, 0x63, 0x20, // ",c " char
    0x2C, 0x54, 0x20, // ",T " true
    0x2C, 0x46, 0x20, // ",F " false
    0x2C, 0x4E, 0x20, // ",N " Nil
    0x2C, 0x49, 0x20, // ",I " Infinitum
    0x2C, 0x66, 0x66, // ",ff" Vector2
    0x2C, 0x66, 0x66, 0x66, // ",fff" Vector3
  };
}
*/