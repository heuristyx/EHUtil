using System;
using Evergreen;
using Everhood.Battle;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EHUtil;

public class Main
{
  public static bool invincible = false;
  public static float currentPitch = 1f;
  public static float lastCheckpoint = 0f;
  public static int enemyHpAtCheckpoint = -1;
  public static bool returnToCheckpointOnHit = false;

  public static object lastCr;
  public static AudioSource lastAudioSource;
  public static float lastSongLength;

  internal static void Init()
  {
    BattleAPI.OnTakeDamage += (object sender, BattleAPI.DamageEventArgs args) =>
    {
      if (invincible) args.damage = 0;
      if (returnToCheckpointOnHit) ReturnToCheckpoint();
    };

    BattleAPI.OnBattleLeave += (object sender, EventArgs args) =>
    {
      currentPitch = 1f;
      lastCheckpoint = 0f;
      lastCr = null;
      lastAudioSource = null;
      lastSongLength = 0f;
      enemyHpAtCheckpoint = -1;
    };

    ChartAPI.OnChartStart += (object sender, EventArgs args) =>
    {
      lastCr = sender;
      lastAudioSource = CompatAPI.ChartReader.GetAudioSource(lastCr);
      lastAudioSource.pitch = currentPitch;
      lastSongLength = lastAudioSource.clip.length;
    };
  }

  internal static void ToggleInvincibility()
  {
    invincible = !invincible;
    TextDrawing.DrawToConsole($"Toggled invincibility {(invincible ? "on" : "off")}");
  }

  internal static void JumpInChart(float distance)
  {
    if (lastCr == null) return;
    var songPos = CompatAPI.ChartReader.GetSongPosition(lastCr);
    var newSongPos = Mathf.Clamp(songPos + distance, 0.0f, float.MaxValue);
    CompatAPI.ChartReader.JumpPosChange(lastCr, newSongPos);
    TextDrawing.DrawToConsole($"Jumped to {newSongPos:F1}");
  }

  internal static void ReturnToCheckpoint()
  {
    var be = GameObject.FindObjectOfType<BattleEnemy>();
    if (lastCr != null)
    {
      CompatAPI.ChartReader.JumpPosChange(lastCr, lastCheckpoint);
      var notes = GameObject.FindObjectsOfType<ProjectileColorAccessibility>();
      foreach (var note in notes) note.gameObject.SetActive(false);
      TextDrawing.DrawToConsole("Moved to checkpoint");
    }
    if (be != null)
    {
      if (enemyHpAtCheckpoint >= 0) be.currentHp = enemyHpAtCheckpoint;
      else be.currentHp = be.startHp;
    }
  }

  internal static void SetPitch(float newPitch)
  {
    currentPitch = Mathf.Clamp(newPitch, 0.1f, 10f);
    CompatAPI.ChartReader.GetAudioSource(lastCr).pitch = currentPitch;
    if (lastCr != null) TextDrawing.DrawToConsole($"Set chart speed to {currentPitch:F1}");
  }

  internal static void QuickRestart()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    if (SceneManager.GetActiveScene().buildIndex == 15) // Incinerator
    {
      EHUtil.CoroutineController.SkipToIncineratorPartTwo();
    }
  }

  internal static void SetCheckpoint()
  {
    var be = GameObject.FindObjectOfType<BattleEnemy>();
    if (lastCr != null)
    {
      var bp = GameObject.FindObjectOfType<BattlePlayer>();
      if (bp != null)
      {
        var checkpointBar = GameObject.Instantiate(Assets.checkpointBar);
        checkpointBar.transform.position = bp.transform.position;
        checkpointBar.AddComponent<CheckpointMove>();
      }
      lastCheckpoint = lastAudioSource.time;
    }
    if (be != null) enemyHpAtCheckpoint = be.currentHp;
    TextDrawing.DrawToConsole($"Set checkpoint at {lastAudioSource.time:F1}");
  }

  internal static void ToggleRCPH()
  {
    returnToCheckpointOnHit = !returnToCheckpointOnHit;
    TextDrawing.DrawToConsole($"Toggled return to checkpoint on hit {(returnToCheckpointOnHit ? "on" : "off")}");
  }

  internal static void DumpChart()
  {
    if (lastCr != null)
    {
      if (!Evergreen.Evergreen.IsBaseGame) TextDrawing.DrawToConsole("Chart dumping not available for custom battles.");
      else
      {
        ChartEncoder.Encode(lastCr as Everhood.Chart.ChartReader);
        TextDrawing.DrawToConsole($"Wrote chart to {(lastCr as Everhood.Chart.ChartReader).chart.songName}.chart");
      }
    }
  }
}