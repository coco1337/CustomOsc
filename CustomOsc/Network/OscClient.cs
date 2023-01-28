using System.Net.Sockets;

namespace CustomOsc.Network;

internal sealed class OscClient
{
  private const string IP_ADDR = "127.0.0.1";
  private const int SEND_PORT = 9000;

  private UdpClient udpClient;

  private byte[] byteArray;
  private int writePos;

  private void ResetWritePos() => writePos = 0;

  internal bool IsConnected => this.udpClient.Client.Connected;

  public void Init()
  {
    udpClient = new UdpClient(IP_ADDR, SEND_PORT);

    byteArray = GC.AllocateArray<byte>(4096, true);

    // var bytes = new OscCore.OscMessage($"/avatar/parameters/Cap", this.test).ToByteArray();
  }

  public void Send(string address, int val)
  {
    Write(address);
    this.byteArray[writePos++] = 0x2C;
    this.byteArray[writePos++] = 0x69;
    this.byteArray[writePos++] = 0x20;
    Write(val);

    this.udpClient.Send(new ReadOnlySpan<byte>(this.byteArray, 0, writePos));
    ResetWritePos();
  }

  public void Send(string address, bool b)
  {
    Write(address);
    this.byteArray[writePos++] = 0x2C;
    if (b) this.byteArray[writePos++] = 0x54;
    else this.byteArray[writePos++] = 0x46;
    this.byteArray[writePos++] = 0x20;

    this.udpClient.Send(new ReadOnlySpan<byte>(this.byteArray, 0, writePos));
    ResetWritePos();
  }

  private void Write(int data)
  {
    this.byteArray[writePos++] = (byte)(data >> 24);
    this.byteArray[writePos++] = (byte)(data >> 16);
    this.byteArray[writePos++] = (byte)(data >> 8);
    this.byteArray[writePos++] = (byte)data;
  }

  private void Write(string data)
  {
    foreach (var c in data)
    {
      this.byteArray[writePos++] = (byte)c;
    }

    var alignedLength = (data.Length + 3) & ~3;
    if (alignedLength == data.Length) alignedLength += 4;

    for (; writePos < alignedLength; ++writePos)
    {
      this.byteArray[writePos] = 0;
    }
  }
} 

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