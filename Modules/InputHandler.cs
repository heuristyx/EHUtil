using Evergreen;
using UnityEngine;

using static EHUtilConfig.Config;

namespace EHUtil;

public static class InputHandler
{
  public static void HandleInput()
  {
    if (TextDrawing.inputField.isFocused) return;

    if (Input.GetKeyDown(debugMenuKey.Value))
      Evergreen.Evergreen.ToggleDebugMode(!Evergreen.Evergreen.DebugMode);

    if (Input.GetKeyDown(invincibilityKey.Value))
      Main.ToggleInvincibility();

    if (Input.GetKeyDown(chartJumpAheadKey.Value))
      Main.JumpInChart(chartJumpIncrement.Value);

    if (Input.GetKeyDown(chartJumpBackKey.Value))
      Main.JumpInChart(-chartJumpIncrement.Value);

    if (Input.GetKeyDown(chartSpeedDownKey.Value))
      Main.SetPitch(Main.currentPitch - chartSpeedIncrement.Value);

    if (Input.GetKeyDown(chartSpeedUpKey.Value))
      Main.SetPitch(Main.currentPitch + chartSpeedIncrement.Value);

    if (Input.GetKeyDown(quickRestartKey.Value))
      Main.QuickRestart();

    if (Input.GetKeyDown(setCheckpointKey.Value))
      Main.SetCheckpoint();

    if (Input.GetKeyDown(moveToCheckpointKey.Value))
      Main.ReturnToCheckpoint();

    if (Input.GetKeyDown(toggleRCPHKey.Value))
      Main.ToggleRCPH();

    if (Input.GetKeyDown(toggleTASModeKey.Value))
      EHUtil.TASController.ToggleTASMode();

    if (Input.GetKeyDown(toggleTASPlaybackKey.Value))
      EHUtil.TASController.PlaybackRecording();

    if (Input.GetKeyDown(dumpChartKey.Value))
      Main.DumpChart();
  }
}