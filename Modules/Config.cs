using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace EHUtilConfig;

internal class Config
{
  public static ConfigEntry<bool> skipIntro;
  public static ConfigEntry<float> chartJumpIncrement;
  public static ConfigEntry<float> chartSpeedIncrement;

  // Keybinds
  public static ConfigEntry<KeyCode> debugMenuKey;
  public static ConfigEntry<KeyCode> invincibilityKey;
  public static ConfigEntry<KeyCode> chartJumpAheadKey;
  public static ConfigEntry<KeyCode> chartJumpBackKey;
  public static ConfigEntry<KeyCode> chartSpeedUpKey;
  public static ConfigEntry<KeyCode> chartSpeedDownKey;
  public static ConfigEntry<KeyCode> quickRestartKey;
  public static ConfigEntry<KeyCode> setCheckpointKey;
  public static ConfigEntry<KeyCode> moveToCheckpointKey;
  public static ConfigEntry<KeyCode> toggleRCPHKey;
  public static ConfigEntry<KeyCode> toggleTASModeKey;
  public static ConfigEntry<KeyCode> advanceTASTimestepKey;
  public static ConfigEntry<KeyCode> toggleTASPlaybackKey;
  public static ConfigEntry<KeyCode> dumpChartKey;

  public static void RegisterConfig(BaseUnityPlugin plugin)
  {
    skipIntro = plugin.Config.Bind("General", "Skip intro", true, "Skip directly to main menu on game launch.");
    chartJumpIncrement = plugin.Config.Bind("General", "Chart jump increment", 5f, "Amount of time skipped when jumping around in a chart (in seconds).");
    chartSpeedIncrement = plugin.Config.Bind("General", "Chart speed increment", .1f, "Increment factor when changing chart speed.");

    debugMenuKey = plugin.Config.Bind("Keybinds", "Toggle debug menu", KeyCode.F1, "Keybind to open the debug UI.");
    invincibilityKey = plugin.Config.Bind("Keybinds", "Toggle invincibility", KeyCode.F2, "Keybind to toggle invincibility.");
    chartJumpAheadKey = plugin.Config.Bind("Keybinds", "Jump ahead in chart", KeyCode.Y, "Keybind to jump ahead in a chart.");
    chartJumpBackKey = plugin.Config.Bind("Keybinds", "Jump back in chart", KeyCode.T, "Keybind to jump back in a chart.");
    chartSpeedUpKey = plugin.Config.Bind("Keybinds", "Chart speed up", KeyCode.H, "Keybind to increase the chart speed.");
    chartSpeedDownKey = plugin.Config.Bind("Keybinds", "Chart speed down", KeyCode.G, "Keybind to decrease the chart speed.");
    quickRestartKey = plugin.Config.Bind("Keybinds", "Quick restart", KeyCode.R, "Keybind to restart a battle.");
    setCheckpointKey = plugin.Config.Bind("Keybinds", "Set checkpoint", KeyCode.B, "Keybind to set a checkpoint.");
    moveToCheckpointKey = plugin.Config.Bind("Keybinds", "Move to checkpoint", KeyCode.N, "Keybind to move to a checkpoint.");
    toggleRCPHKey = plugin.Config.Bind("Keybinds", "Toggle RCPH", KeyCode.M, "Keybind to toggle returning to checkpoint automatically on hit.");
    toggleTASModeKey = plugin.Config.Bind("Keybinds", "Toggle TAS Mode", KeyCode.L, "Keybind to toggle TAS Mode.");
    advanceTASTimestepKey = plugin.Config.Bind("Keybinds", "Advance timestep", KeyCode.Period, "Keybind to advance a timestep in TAS Mode.");
    toggleTASPlaybackKey = plugin.Config.Bind("Keybinds", "Toggle TAS Playback", KeyCode.Semicolon, "Keybind to start or stop TAS playback.");
    dumpChartKey = plugin.Config.Bind("Keybinds", "Dump chart", KeyCode.F4, "Keybind to dump chart to a .chart file.");
  }
}