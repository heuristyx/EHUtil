using BepInEx;
using BepInEx.Configuration;

namespace EHUtilConfig;

internal class Config
{
  public static ConfigEntry<bool> skipIntro;

  public static void RegisterConfig(BaseUnityPlugin plugin)
  {
    skipIntro = plugin.Config.Bind(
      "General", "Skip intro", true, "Skip directly to main menu on game launch."
    );
  }
}