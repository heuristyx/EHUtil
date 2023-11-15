using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using Evergreen;
using Everhood;
using UnityEngine.SceneManagement;
using System;
using Everhood.Battle;
using Doozy.Engine.Extensions;
using System.Collections.Generic;

using static EHUtilConfig.Config;
using System.Collections;
using Fungus;
using System.Linq;

namespace EHUtil;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency("com.heuristyx.plugins.evergreen")]
public class EHUtil : BaseUnityPlugin
{
  public static PluginInfo PluginInfo { get; private set; }

  public const string Guid = "com.heuristyx.plugins.ehutil";
  public const string Name = "EHUtil";
  public const string Version = "1.0.0";

  public static bool invincible = false;
  public static float currentPitch = 1f;
  public static float lastCheckpoint = 0f;
  public static int enemyHpAtCheckpoint = -1;
  public static bool returnToCheckpointOnHit = false;

  internal static TASModeController TASController;

  public static object lastCr;
  public static AudioSource lastAudioSource;
  public static float lastSongLength;

  private EHUtilUI UI;

  internal static ManualLogSource Log;

  private void Awake()
  {
    Log = BepInEx.Logging.Logger.CreateLogSource("EHUtil");

    PluginInfo = Info;

    RegisterConfig(this);

    Assets.Init();
    Commands.Init();

    Locking.AddLock(new Locking.Lock(
      Guid, "main", Locking.LockType.All
    ));

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

    TASController = this.gameObject.AddComponent<TASModeController>();

    var UIgo = new GameObject("EHUtil UI");
    UIgo.transform.SetParent(TextDrawing.GetCanvas().transform);
    UI = UIgo.AddComponent<EHUtilUI>();
    UIgo.AddComponent<RectTransform>().FullScreen(true);

    Evergreen.Evergreen.OnToggleDebugMode += (self, state) => { UI.DebugUI.gameObject.SetActive(state); };

    Evergreen.Evergreen.ToggleDebugMode(false);
  }

  private float JumpInChart(float distance)
  {
    var songPos = CompatAPI.ChartReader.GetSongPosition(lastCr);
    var newSongPos = Mathf.Clamp(songPos + distance, 0.0f, float.MaxValue);
    CompatAPI.ChartReader.JumpPosChange(lastCr, newSongPos);
    return newSongPos;
  }

  private void ReturnToCheckpoint()
  {
    var be = FindObjectOfType<BattleEnemy>();
    if (lastCr != null)
    {

      CompatAPI.ChartReader.JumpPosChange(lastCr, lastCheckpoint);
      var notes = FindObjectsOfType<ProjectileColorAccessibility>();
      foreach (var note in notes) note.gameObject.SetActive(false);
    }
    if (be != null)
    {
      if (enemyHpAtCheckpoint >= 0) be.currentHp = enemyHpAtCheckpoint;
      else be.currentHp = be.startHp;
    }
  }

  private void SetPitch(float newPitch)
  {
    currentPitch = Mathf.Clamp(newPitch, 0.1f, 10f);
    CompatAPI.ChartReader.GetAudioSource(lastCr).pitch = currentPitch;
  }

  private void Update()
  {
    if (Evergreen.Evergreen.IsBaseGame)
    {
      if (SceneManager.GetActiveScene().name == "IntroMenu" && skipIntro.Value)
        SceneManager.LoadScene("MainMenu");
    }

    if (TextDrawing.inputField.isFocused) return;

    if (Input.GetKeyDown(debugMenuKey.Value))
    {
      Evergreen.Evergreen.ToggleDebugMode(!Evergreen.Evergreen.DebugMode);
    }
    if (Input.GetKeyDown(invincibilityKey.Value))
    {
      invincible = !invincible;
      TextDrawing.DrawToConsole($"Toggled invincibility {(invincible ? "on" : "off")}");
    }
    if (Input.GetKeyDown(chartJumpAheadKey.Value) && lastCr != null)
    {
      var newPos = JumpInChart(chartJumpIncrement.Value);
      TextDrawing.DrawToConsole($"Jumped to {newPos:F1}");
    }
    if (Input.GetKeyDown(chartJumpBackKey.Value) && lastCr != null)
    {
      var newPos = JumpInChart(-chartJumpIncrement.Value);
      TextDrawing.DrawToConsole($"Jumped to {newPos:F1}");
    }
    if (Input.GetKeyDown(chartSpeedDownKey.Value))
    {
      SetPitch(currentPitch - chartSpeedIncrement.Value);
      if (lastCr != null) TextDrawing.DrawToConsole($"Set chart speed to {currentPitch:F1}");
    }
    if (Input.GetKeyDown(chartSpeedUpKey.Value))
    {
      SetPitch(currentPitch + chartSpeedIncrement.Value);
      if (lastCr != null) TextDrawing.DrawToConsole($"Set chart speed to {currentPitch:F1}");
    }
    if (Input.GetKeyDown(quickRestartKey.Value))
    {
      // BattlePauseController bpc = FindObjectOfType<BattlePauseController>();
      // if (bpc != null) bpc.Retry();
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      if (SceneManager.GetActiveScene().buildIndex == 15) // Incinerator
      {
        StartCoroutine(SkipToIncineratorPartTwo());
      }
    }
    if (Input.GetKeyDown(setCheckpointKey.Value))
    {
      var be = FindObjectOfType<BattleEnemy>();
      if (lastCr != null)
      {
        var bp = FindObjectOfType<BattlePlayer>();
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
    if (Input.GetKeyDown(moveToCheckpointKey.Value))
    {
      ReturnToCheckpoint();
      TextDrawing.DrawToConsole("Moved to checkpoint");
    }
    if (Input.GetKeyDown(toggleRCPHKey.Value))
    {
      returnToCheckpointOnHit = !returnToCheckpointOnHit;
      TextDrawing.DrawToConsole($"Toggled return to checkpoint on hit {(returnToCheckpointOnHit ? "on" : "off")}");
    }
    if (Input.GetKeyDown(toggleTASModeKey.Value))
    {
      TASController.ToggleTASMode();
    }
    if (Input.GetKeyDown(toggleTASPlaybackKey.Value))
    {
      TASController.PlaybackRecording();
    }
    if (Input.GetKeyDown(dumpChartKey.Value))
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

  IEnumerator SkipToIncineratorPartTwo()
  {
    yield return new WaitForSeconds(.1f);
    var list = GameObject.FindObjectsOfType<Flowchart>().First(f => f.gameObject.name == "EventFlowChart").FindBlock("Start").CommandList;
    list.RemoveRange(13, 8); // Remove Red looking around in the cutscene
    yield return new WaitForSeconds(.5f);
    var readers = GameObject.FindObjectsOfType<Everhood.Chart.ChartReader>();
    foreach (var reader in readers)
      if (reader.gameObject.name == "ChartEditor Incinerator part 2") reader.StartChartReader();
  }
}
