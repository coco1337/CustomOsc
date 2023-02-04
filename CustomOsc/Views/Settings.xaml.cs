using CustomOsc.Global;
using CustomOsc.Models;
using Newtonsoft.Json;

namespace CustomOsc;

public partial class Settings : ContentPage
{
  public Settings()
  {
    InitializeComponent();

    LoadSettingData();
  }

  private void RefreshDatas()
  {
    if (GlobalSetting.Config.Watch != null)
    {
      ParamYear.Text = GlobalSetting.Config.Watch.Year;
      ParamMonth.Text = GlobalSetting.Config.Watch.Month;
      ParamDay.Text = GlobalSetting.Config.Watch.Day;
      ParamHour.Text = GlobalSetting.Config.Watch.Hour;
      ParamMinute.Text = GlobalSetting.Config.Watch.Minute;
      ParamSecond.Text = GlobalSetting.Config.Watch.Second;
    }
    else
      GlobalSetting.Config.Watch = new();
  }

  private void LoadSettingData()
  {
    try
    {
      var settingFile = System.IO.Path.Combine(FileSystem.AppDataDirectory, "SettingData.json");
      using var fs = System.IO.File.OpenRead(settingFile);
      using var reader = new StreamReader(fs);

      var jsonString = reader.ReadToEnd();

      var watchParamsData = JsonConvert.DeserializeObject<SettingData>(jsonString);

      GlobalSetting.Config = watchParamsData;

      GlobalSetting.IsSet = true;

      RefreshDatas();
    }
    catch 
    {
      // TODO : handle Exception, cannot find setting data
    }
  }
  
  private void SaveBtn_Clicked(object sender, EventArgs e)
  {
    GlobalSetting.Config.Watch.Year = ParamYear.Text;
    GlobalSetting.Config.Watch.Month = ParamMonth.Text;
    GlobalSetting.Config.Watch.Day = ParamDay.Text;
    GlobalSetting.Config.Watch.Hour = ParamHour.Text;
    GlobalSetting.Config.Watch.Minute = ParamMinute.Text;
    GlobalSetting.Config.Watch.Second = ParamSecond.Text;

    var settingFile = System.IO.Path.Combine(FileSystem.AppDataDirectory, "SettingData.json");
    using var fs = System.IO.File.OpenWrite(settingFile);
    using var writer = new StreamWriter(fs);

    var jsonString = JsonConvert.SerializeObject(GlobalSetting.Config);

    writer.Write(jsonString);
  }

  private void LoadBtn_Clicked(object sender, EventArgs e)
  {
    try
    {
      var settingFile = System.IO.Path.Combine(FileSystem.AppDataDirectory, "SettingData.json");
      var jsonString = System.IO.File.ReadAllText(settingFile);

      GlobalSetting.Config = JsonConvert.DeserializeObject<SettingData>(jsonString);
      RefreshDatas();
    }
    catch (Exception exception)
    {
      // TODO : handle Exception
    }
  }
}