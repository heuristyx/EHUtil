using Doozy.Engine.Extensions;
using Evergreen;
using TMPro;
using UnityEngine;
using static Evergreen.TextDrawing;
using static EHUtilConfig.Config;

namespace EHUtil;

public class EHUtilUI : MonoBehaviour
{
  private GameObject SpeedMultiplier;
  private GameObject InvincibilityLabel;
  private GameObject ChartPosLabel;
  private GameObject DebugControlsLabel;
  private GameObject TASPlaybackLabel;
  public GameObject DebugUI;

  private void Awake()
  {
    SpeedMultiplier = DrawText("", TextDrawing.TextAlignmentOptions.BottomRight, 72);
    SpeedMultiplier.SetActive(false);
    SpeedMultiplier.transform.SetParent(this.gameObject.transform);

    InvincibilityLabel = DrawText("Invincible", TextDrawing.TextAlignmentOptions.BottomRight, 48);
    InvincibilityLabel.SetActive(false);
    InvincibilityLabel.transform.SetParent(this.gameObject.transform);
    InvincibilityLabel.transform.localPosition = new Vector3(0f, 100f, 0f);

    DebugUI = new GameObject("Debug UI");
    DebugUI.transform.SetParent(this.gameObject.transform);
    DebugUI.AddComponent<RectTransform>().FullScreen(true);

    TASPlaybackLabel = DrawText("TAS Playback", TextDrawing.TextAlignmentOptions.BottomRight, 48);
    TASPlaybackLabel.SetActive(false);
    TASPlaybackLabel.transform.SetParent(this.gameObject.transform);
    TASPlaybackLabel.transform.localPosition = new Vector3(0f, 200f, 0f);

    ChartPosLabel = DrawText("", TextDrawing.TextAlignmentOptions.Right);
    ChartPosLabel.transform.SetParent(DebugUI.transform);
    ChartPosLabel.transform.localPosition = new Vector3(0f, 200f, 0f);

    string debugControls = "<b><size=125%>Debug controls</size></b>\n";
    debugControls += $"Show/hide this menu <space=2em> <color=#00FF00>{debugMenuKey.Value}</color>\n";
    debugControls += $"Toggle invincibility <space=2em> <color=#00FF00>{invincibilityKey.Value}</color>\n";
    debugControls += $"Move in chart <space=2em> <color=#00FF00>{chartJumpBackKey.Value}</color>/<color=#00FF00>{chartJumpAheadKey.Value}</color>\n";
    debugControls += $"Change chart speed <space=2em> <color=#00FF00>{chartSpeedDownKey.Value}</color>/<color=#00FF00>{chartSpeedUpKey.Value}</color>\n";
    debugControls += $"Set/move to checkpoint <space=2em> <color=#00FF00>{setCheckpointKey.Value}</color>/<color=#00FF00>{moveToCheckpointKey.Value}</color>\n";
    debugControls += $" Toggle return to checkpoint on hit (RCPH) <space=2em> <color=#00FF00>{toggleRCPHKey.Value}</color>\n";
    debugControls += $"Toggle TAS mode/TAS playback <space=2em> <color=#00FF00>{toggleTASModeKey.Value}</color>/<color=#00FF00>{toggleTASPlaybackKey.Value}</color>\n";
    debugControls += $"<color=#AAAAAA>(In TAS mode)</color> Advance timestep <space=2em> <color=#00FF00>{advanceTASTimestepKey.Value}</color>\n";
    debugControls += $"Quick restart <space=2em> <color=#00FF00>{quickRestartKey.Value}</color>\n";
    debugControls += $"Dump current chart <space=2em> <color=#00FF00>{dumpChartKey.Value}</color>";

    DebugControlsLabel = DrawText(debugControls, TextDrawing.TextAlignmentOptions.Right);
    DebugControlsLabel.transform.SetParent(DebugUI.transform);
    DebugControlsLabel.transform.localPosition = new Vector3(0, -100f, 0);
  }

  private void Update()
  {
    var stm = SpeedMultiplier.GetComponent<TextMeshProUGUI>();

    if (Main.currentPitch == 1f && SpeedMultiplier.activeSelf) SpeedMultiplier.SetActive(false);
    else if (Main.currentPitch != 1f && !SpeedMultiplier.activeSelf) SpeedMultiplier.SetActive(true);

    stm.text = $"x{Main.currentPitch:F1}";

    stm.color = Main.currentPitch switch
    {
      >= 1.95f => Color.red,
      >= 1.45f and < 1.95f => new Color(1f, 0.5f, 0f),
      >= 1.05f and < 1.45f => Color.yellow,
      >= 0.95f and < 1.05f => Color.white,
      >= 0.55f and < 0.95f => new Color(0.75f, 1f, 0.75f),
      < 0.55f => new Color(0.5f, 1f, 0.5f),
      _ => Color.white
    };

    if (Main.invincible && !InvincibilityLabel.activeSelf) InvincibilityLabel.SetActive(true);
    else if (!Main.invincible && InvincibilityLabel.activeSelf) InvincibilityLabel.SetActive(false);

    if (Evergreen.Evergreen.DebugMode)
    {
      if (Main.lastCr != null)
      {
        string chartPos = $"Chart pos: {Main.lastAudioSource.time:F1}/{Main.lastSongLength:F1}\nCheckpoint pos: {Main.lastCheckpoint:F1} (RCPH {(Main.returnToCheckpointOnHit ? "on" : "off")})";
        ChartPosLabel.GetComponent<TextMeshProUGUI>().text = chartPos;
      }
    }

    if (EHUtil.TASController.replayMode && !TASPlaybackLabel.activeSelf) TASPlaybackLabel.SetActive(true);
    else if (!EHUtil.TASController.replayMode && TASPlaybackLabel.activeSelf) TASPlaybackLabel.SetActive(false);
  }
}