using CustomOsc.Models;

namespace CustomOsc.Global
{
  public static class GlobalSetting
  {
    public static bool IsSet { get; set;} = false;
    public static SettingData Config { get; set; }
  }
}
