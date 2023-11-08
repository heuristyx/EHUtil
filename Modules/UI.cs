using Doozy.Engine.Extensions;
using Evergreen;
using TMPro;
using UnityEngine;
using static Evergreen.TextDrawing;

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

    string debugControls = "-=Debug controls=-\n";
    debugControls += "F1 - Show/hide this menu\n";
    debugControls += "F2 - Toggle invincibility\n";
    debugControls += "T/Y - Move in chart\n";
    debugControls += "G/H - Change chart speed\n";
    debugControls += "B - Set checkpoint, N - Move to checkpoint\n";
    debugControls += "M - Toggle return to checkpoint on hit (RCPH)\n";
    debugControls += "L - Toggle TAS mode, ; - TAS Playback\n";
    debugControls += "(In TAS mode) . - Advance timestep\n";
    debugControls += "R - Quick restart\n";
    debugControls += "F4 - Dump current chart";

    DebugControlsLabel = DrawText(debugControls, TextDrawing.TextAlignmentOptions.Right);
    DebugControlsLabel.transform.SetParent(DebugUI.transform);
    DebugControlsLabel.transform.localPosition = new Vector3(0, -200f, 0);
  }

  private void Update()
  {
    var stm = SpeedMultiplier.GetComponent<TextMeshProUGUI>();

    if (EHUtil.currentPitch == 1f && SpeedMultiplier.activeSelf) SpeedMultiplier.SetActive(false);
    else if (EHUtil.currentPitch != 1f && !SpeedMultiplier.activeSelf) SpeedMultiplier.SetActive(true);

    stm.text = $"x{EHUtil.currentPitch:F1}";

    stm.color = EHUtil.currentPitch switch
    {
      >= 1.95f => Color.red,
      >= 1.45f and < 1.95f => new Color(1f, 0.5f, 0f),
      >= 1.05f and < 1.45f => Color.yellow,
      >= 0.95f and < 1.05f => Color.white,
      >= 0.55f and < 0.95f => new Color(0.75f, 1f, 0.75f),
      < 0.55f => new Color(0.5f, 1f, 0.5f),
      _ => Color.white
    };

    if (EHUtil.invincible && !InvincibilityLabel.activeSelf) InvincibilityLabel.SetActive(true);
    else if (!EHUtil.invincible && InvincibilityLabel.activeSelf) InvincibilityLabel.SetActive(false);

    if (EHUtil.debugMode)
    {
      if (EHUtil.lastCr != null)
      {
        string chartPos = $"Chart pos: {EHUtil.lastAudioSource.time:F1}/{EHUtil.lastSongLength:F1}\nCheckpoint pos: {EHUtil.lastCheckpoint:F1} (RCPH {(EHUtil.returnToCheckpointOnHit ? "on" : "off")})";
        ChartPosLabel.GetComponent<TextMeshProUGUI>().text = chartPos;
      }
    }

    if (EHUtil.TASController.replayMode && !TASPlaybackLabel.activeSelf) TASPlaybackLabel.SetActive(true);
    else if (!EHUtil.TASController.replayMode && TASPlaybackLabel.activeSelf) TASPlaybackLabel.SetActive(false);
  }
}