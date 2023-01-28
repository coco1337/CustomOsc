using CustomOsc.Models;

namespace CustomOsc;

public partial class Settings : ContentPage
{
	private ListView listView = new();

	public Settings()
	{
		InitializeComponent();

		this.listView.SetBinding(ItemsView.ItemsSourceProperty, "AvatarParameters");
		this.listView.BeginRefresh();
	}
}