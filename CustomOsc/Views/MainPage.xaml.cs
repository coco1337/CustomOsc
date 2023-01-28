using CustomOsc.Network;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace CustomOsc;

public partial class MainPage : ContentPage
{
	private readonly ConcurrentQueue<string> recvQueue = new();
	private readonly ConcurrentQueue<byte[]> sendQueue = new();

	private OscClient oscClient = new();
	private Thread udpRecvThread = null;
	private int updateDuration = 1000;

	private const int SEND_PORT = 9000;
	private const int RECV_PORT = 9001;

	int count = 0;

	private bool test = false;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

  private async void OnConnectClicked(object sender, EventArgs e)
  {
		this.oscClient.Init();

		if (this.oscClient.IsConnected)
			ConnectStatus.Text = "Connected";
		// recv loop
		this.udpRecvThread = new Thread(() => UdpListener());
		this.udpRecvThread.Start();

		var timer = new System.Timers.Timer();
		timer.Interval = this.updateDuration;
		timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerElapsed);
		timer.Start();

		while (true)
		{
			await Task.Delay(TimeSpan.FromMilliseconds(this.updateDuration));
			if (this.recvQueue.IsEmpty)
				continue;

			while (!this.recvQueue.IsEmpty)
			{
				if (this.recvQueue.TryDequeue(out var result))
					Log.Text = result;
			}
		}
  }

	private async void UdpListener()
	{
		using var receiver = new UdpClient(RECV_PORT);

		while (true)
		{
			var receivedResults = await receiver.ReceiveAsync();

			var str = Encoding.Default.GetString(receivedResults.Buffer);
			if (str.StartsWith("/avatar/change"))
				this.recvQueue.Enqueue(str);
		}
	}

	private void TimerElapsed(object sender, ElapsedEventArgs e)
	{
		this.oscClient.Send("/avatar/parameters/GlassesToggle", this.test);
		this.test = !this.test;
	}
}

