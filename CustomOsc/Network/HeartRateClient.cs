using System.Net.Http.Headers;
using System.Net.WebSockets;

namespace CustomOsc.Network;

internal class HeartRateClient
{
  private const string PULSOID_URL = "https://api.stromno.com/v1/api/public/rpc";
  private const string TOKEN = "";
  private const string UUID = "";
  private HttpClient client = new();

  public async Task StartClient()
  {
    //var uri = new Uri($"{PULSOID_URL}{TOKEN}");
    //using var ws = new ClientWebSocket();
    //await ws.ConnectAsync(uri, CancellationToken.None);

    //var received = new byte[4096];
    //while(ws.State == WebSocketState.Open)
    //{
    //  var result = await ws.ReceiveAsync(new ArraySegment<byte>(received), CancellationToken.None);
    //  Console.WriteLine(result);
    //}

    using var request = new HttpRequestMessage(HttpMethod.Post, PULSOID_URL);

    var reqString = "{\"id\":\"" + UUID + "\",\"method\":\"getWidget\",\"jsonrpc\":\"2.0\",\"params\":{\"widget_id\":\"" + TOKEN + "\"}}";
    request.Content = new StringContent(reqString, new MediaTypeHeaderValue("application/json"));

    using var response = await client.SendAsync(request);

    response.EnsureSuccessStatusCode();

    var responseString = await response.Content.ReadAsStringAsync();

    var t = "asdf";
  }
}