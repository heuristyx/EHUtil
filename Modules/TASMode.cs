using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Everhood.Battle;
using IL.MoonSharp.Interpreter.Tree.Expressions;
using UnityEngine;

namespace EHUtil;

public class TASModeController : MonoBehaviour
{
  private BattlePlayer player;
  public bool tasToggled = false;
  public bool tasMode = false;
  public bool replayMode = false;
  private int replayIndex = 0;
  private float lastSongTime = 0f;
  private TASRecording recording = new TASRecording();

  private KeyCode[] validKeys = {
    KeyCode.A, KeyCode.LeftArrow,
    KeyCode.D, KeyCode.RightArrow,
    KeyCode.W, KeyCode.UpArrow, KeyCode.Space,
    KeyCode.S, KeyCode.DownArrow
  };

  public void ToggleTASMode()
  {
    tasMode = !tasMode;
    tasToggled = true;
  }

  private void Update()
  {
    // On toggle TAS Mode off
    if (tasToggled && !tasMode && EHUtil.lastCr != null)
    {
      Time.timeScale = 1f;
      EHUtil.lastAudioSource.Play();
      tasToggled = false;
    }

    // TAS Mode update
    if (tasMode && EHUtil.lastCr != null)
    {
      if (tasToggled)
      {
        Time.timeScale = 0f;
        EHUtil.lastAudioSource.Pause();
        recording.Clear();
        tasToggled = false;
      }

      foreach (var key in validKeys)
      {
        if (Input.GetKeyDown(key))
          recording.AddInput(EHUtil.lastAudioSource.time, key);
      }

      if (lastSongTime < EHUtil.lastAudioSource.time)
      {
        Time.timeScale = 0f;
        EHUtil.lastAudioSource.Pause();
      }

      if (Input.GetKeyDown(KeyCode.Period))
      {
        Time.timeScale = 1f;
        EHUtil.lastAudioSource.Play();
      }

      lastSongTime = EHUtil.lastAudioSource.time;
    }

    // Replay mode update
    if (replayMode && !tasMode && EHUtil.lastCr != null)
    {
      for (int i = replayIndex; i < recording.inputs.Count; i++)
      {
        var currentKvp = recording.inputs.ElementAt(i);
        if (currentKvp.Key <= EHUtil.lastAudioSource.time)
        {
          foreach (var key in currentKvp.Value)
          {
            switch (key)
            {
              case KeyCode.A:
              case KeyCode.LeftArrow:
                player.horizontalInput.playerMovementObservers[0].Move(-1f);
                break;
              case KeyCode.D:
              case KeyCode.RightArrow:
                player.horizontalInput.playerMovementObservers[0].Move(1f);
                break;
              case KeyCode.W:
              case KeyCode.UpArrow:
              case KeyCode.Space:
                player.verticalInput.playerMovementObservers[0].Move(1f);
                break;
              case KeyCode.S:
              case KeyCode.DownArrow:
                if (player.absorbBehaviour != null)
                  player.absorbBehaviour.Absorb();
                break;
              default:
                break;
            }
          }
          replayIndex = i + 1;
        }
        else break;
      }
    }
  }

  internal void PlaybackRecording()
  {
    if (replayMode)
    {
      replayMode = false;
      replayIndex = 0;
      return;
    }
    player = FindObjectOfType<BattlePlayer>();
    if (recording.inputs.Count > 0 && player != null)
      replayMode = true;
  }
}

internal class TASRecording
{
  public Dictionary<float, List<KeyCode>> inputs = new Dictionary<float, List<KeyCode>>();

  public void AddInput(float time, KeyCode input)
  {
    if (!inputs.ContainsKey(time)) inputs.Add(time, new List<KeyCode> { input });
    else inputs[time].Add(input);
  }

  public void Clear()
  {
    inputs.Clear();
  }
}