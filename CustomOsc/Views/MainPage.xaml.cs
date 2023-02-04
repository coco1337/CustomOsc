using CustomOsc.Global;
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
	private DateTime cacheDate;

	private const int RECV_PORT = 9001;

	private bool isInitialized = false;
	
	private bool test = false;

	public MainPage()
	{
		InitializeComponent();
	}

  private async void OnConnectClicked(object sender, EventArgs e)
  {
		if (isInitialized) return;
		this.oscClient.Init();

		if (this.oscClient.IsConnected)
			ConnectStatus.Text = "Connected";
		// recv loop
		this.udpRecvThread = new Thread(() => UdpListener());
		this.udpRecvThread.Start();

		SendInitTime();

		var timer = new System.Timers.Timer();
		timer.Interval = this.updateDuration;
		timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerElapsed);
		timer.Start();

		isInitialized = true;

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

	// only send on first time
	private void SendInitTime()
	{
		this.cacheDate = DateTime.Now;
    this.oscClient.Send(GlobalSetting.Config.Watch.Year, this.cacheDate.Year);
    this.oscClient.Send(GlobalSetting.Config.Watch.Month, this.cacheDate.Month);
    this.oscClient.Send(GlobalSetting.Config.Watch.Day, this.cacheDate.Day);
    this.oscClient.Send(GlobalSetting.Config.Watch.Hour, this.cacheDate.Hour);
    this.oscClient.Send(GlobalSetting.Config.Watch.Minute, this.cacheDate.Minute);
    this.oscClient.Send(GlobalSetting.Config.Watch.Second, this.cacheDate.Second);
  }

	private void TimerElapsed(object sender, ElapsedEventArgs e)
	{
		if (!this.oscClient.IsConnected) return;

		// check global setting
		if (!GlobalSetting.IsSet) return;
		
		var currentTime = DateTime.Now;
		if (this.cacheDate.Year != currentTime.Year) this.oscClient.Send(GlobalSetting.Config.Watch.Year, currentTime.Year);
		if (this.cacheDate.Month != currentTime.Month) this.oscClient.Send(GlobalSetting.Config.Watch.Month, currentTime.Month);
		if (this.cacheDate.Day != currentTime.Day) this.oscClient.Send(GlobalSetting.Config.Watch.Day, currentTime.Day);
		if (this.cacheDate.Hour != currentTime.Hour) this.oscClient.Send(GlobalSetting.Config.Watch.Hour, currentTime.Hour);
		if (this.cacheDate.Minute != currentTime.Minute) this.oscClient.Send(GlobalSetting.Config.Watch.Minute, currentTime.Minute);
		if (this.cacheDate.Second != currentTime.Second) this.oscClient.Send(GlobalSetting.Config.Watch.Second, currentTime.Second);
		this.test = !this.test;
	}
}

