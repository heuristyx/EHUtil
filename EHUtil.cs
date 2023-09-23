using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using Evergreen;
using Everhood;
using UnityEngine.SceneManagement;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Reflection;
using Everhood.Battle;
using System.Collections.Generic;
using Doozy.Engine.Extensions;

namespace EHUtil;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency("com.heuristyx.plugins.evergreen")]
public class EHUtil : BaseUnityPlugin {
  public static PluginInfo PluginInfo { get; private set; }

  public const string Guid = "com.heuristyx.plugins.ehutil";
  public const string Name = "EHUtil";
  public const string Version = "1.0.0";

  public static bool invincible = false;
  public static float currentPitch = 1f;
  public static float lastCheckpoint = 0f;
  public static int enemyHpAtCheckpoint = -1;
  public static bool returnToCheckpointOnHit = false;
  public static bool debugMode = false;

  internal static TASModeController TASController;

  public static Everhood.Chart.ChartReader lastCr;
  public static float lastSongLength;

  private EHUtilUI UI;

  internal static ManualLogSource Log;

  private void Awake() {
    Log = BepInEx.Logging.Logger.CreateLogSource("EHUtil");

    PluginInfo = Info;

    Assets.Init();

    Locking.AddLock(new Locking.Lock(
      Guid, "main", Locking.LockType.All
    ));

    BattleAPI.OnTakeDamage += (object sender, BattleAPI.DamageEventArgs args) => {
      if (invincible) args.damage = 0;
      if (returnToCheckpointOnHit) ReturnToCheckpoint();
    };

    BattleAPI.OnBattleLeave += (object sender, EventArgs args) => {
      SetPitch(1f);
      lastCheckpoint = 0f;
      lastCr = null;
      lastSongLength = 0f;
      enemyHpAtCheckpoint = -1;
    };

    ChartAPI.OnChartStart += (object sender, EventArgs args) => {
      var cr = (Everhood.Chart.ChartReader)sender;
      lastCr = cr;
      lastCr.audioSource.pitch = currentPitch;
      lastSongLength = lastCr.audioSource.clip.length;
    };

    IL.Everhood.Chart.ChartReader.ChartReaderBehaviour += HookChartReaderBehaviour;
    IL.Everhood.Chart.ChartReader.JumpPosChange += HookJumpPosChange;

    TASController = this.gameObject.AddComponent<TASModeController>();

    var UIgo = new GameObject("EHUtil UI");
    UIgo.transform.SetParent(TextDrawing.GetCanvas().transform);
    UI = UIgo.AddComponent<EHUtilUI>();
    UIgo.AddComponent<RectTransform>().FullScreen(true);

    ToggleDebugMode(false);
  }

  private void HookJumpPosChange(ILContext il) {
    var c = new ILCursor(il);

    var m_setSongPosition = typeof(Everhood.Chart.ChartReader).GetMethod("set__songposition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    c.TryGotoNext(MoveType.After, i => i.MatchCall(m_setSongPosition));
    c.Emit(OpCodes.Ldarg_0);
    c.EmitDelegate<Action<Everhood.Chart.ChartReader>>((lastCr) => {
      lastCr._songposition = lastCr.audioSource.time;
    });
  }

  private void HookChartReaderBehaviour(ILContext il) {
    var c = new ILCursor(il);

    var m_setSongPosition = typeof(Everhood.Chart.ChartReader).GetMethod("set__songposition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    c.TryGotoNext(MoveType.After, i => i.MatchCall(m_setSongPosition));
    c.Emit(OpCodes.Ldarg_0);
    c.EmitDelegate<Action<Everhood.Chart.ChartReader>>((lastCr) => {
      if (lastCr.audioSource.pitch != currentPitch) lastCr.audioSource.pitch = currentPitch;
      lastCr._songposition = lastCr.audioSource.time;
    });
  }

  public void ToggleDebugMode(bool state) {
    debugMode = state;
    TextDrawing.ToggleConsole(state);

    UI.gameObject.SetActive(state);
  }

  private void JumpInChart(float distance) {
    if (lastCr != null) {
      lastCr.JumpPosChange(Mathf.Clamp(lastCr._songposition + distance, 0.0f, float.MaxValue));
    }
  }

  private void ReturnToCheckpoint() {
    var be = FindObjectOfType<BattleEnemy>();
    if (lastCr != null) {
      lastCr.JumpPosChange(lastCheckpoint);
      var notes = FindObjectsOfType<ProjectileColorAccessibility>();
      foreach (var note in notes) note.gameObject.SetActive(false);
    }
    if (be != null) {
      if (enemyHpAtCheckpoint >= 0) be.currentHp = enemyHpAtCheckpoint;
      else be.currentHp = be.startHp;
    }
  }

  private void SetPitch(float newPitch) {
    currentPitch = Mathf.Clamp(newPitch, 0.1f, 10f);
  }

  private void Update() {
    if (SceneManager.GetActiveScene().name == "IntroMenu") {
      SceneManager.LoadScene("MainMenu");
    }
    if (Input.GetKeyDown(KeyCode.F2)) {
      invincible = !invincible;
      TextDrawing.DrawToConsole($"Toggled invincibility {(invincible ? "on" : "off")}");
    }
    if (Input.GetKeyDown(KeyCode.Y)) {
      JumpInChart(5f);
      TextDrawing.DrawToConsole("Jumped 5 seconds ahead");
    }
    if (Input.GetKeyDown(KeyCode.T)) {
      JumpInChart(-5f);
      TextDrawing.DrawToConsole("Jumped 5 seconds back");
    }
    if (Input.GetKeyDown(KeyCode.G)) {
      SetPitch(currentPitch - 0.1f);
      if (lastCr != null && debugMode)TextDrawing.DrawToConsole($"Set timescale to {currentPitch:F1}");
    }
    if (Input.GetKeyDown(KeyCode.H)) {
      SetPitch(currentPitch + 0.1f);
      if (lastCr != null && debugMode)TextDrawing.DrawToConsole($"Set timescale to {currentPitch:F1}");
    }
    if (Input.GetKeyDown(KeyCode.R)) {
      BattlePauseController bpc = FindObjectOfType<BattlePauseController>();
      if (bpc != null) bpc.Retry();
    }
    if (Input.GetKeyDown(KeyCode.F1)) {
      ToggleDebugMode(!debugMode);
    }
    if (Input.GetKeyDown(KeyCode.B)) {
      var be = FindObjectOfType<BattleEnemy>();
      if (lastCr != null) {
        var bp = FindObjectOfType<BattlePlayer>();
        if (bp != null) {
          var checkpointBar = GameObject.Instantiate(Assets.checkpointBar);
          checkpointBar.transform.position = bp.transform.position;
          checkpointBar.AddComponent<CheckpointMove>();
        }
        lastCheckpoint = lastCr.audioSource.time;
      }
      if (be != null) enemyHpAtCheckpoint = be.currentHp;
      TextDrawing.DrawToConsole($"Set checkpoint at {lastCr.audioSource.time:F1}");
    }
    if (Input.GetKeyDown(KeyCode.N)) {
      ReturnToCheckpoint();
      TextDrawing.DrawToConsole("Moved to checkpoint");
    }
    if (Input.GetKeyDown(KeyCode.M)) {
      returnToCheckpointOnHit = !returnToCheckpointOnHit;
      TextDrawing.DrawToConsole($"Toggled return to checkpoint on hit {(returnToCheckpointOnHit ? "on" : "off")}");
    }
    if (Input.GetKeyDown(KeyCode.L)) {
      TASController.ToggleTASMode();
    }
    if (Input.GetKeyDown(KeyCode.Semicolon)) {
      TASController.PlaybackRecording();
    }
    if (Input.GetKeyDown(KeyCode.F4)) {
      if (lastCr != null) {
        ChartEncoder.Encode(lastCr);
        TextDrawing.DrawToConsole($"Wrote chart to file {lastCr.chart.songName}.chart");
      }
    }
  }
}
