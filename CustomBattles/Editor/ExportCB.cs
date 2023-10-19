using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;

public class Note {
  public int noteID;
  public List<EventCommand> events;
}

public class Section {
  public string sectionID;
  public List<EventCommand> events;
}

public class EventCommand {
  public string eventName;
}

public class ShootObstacleEventCommand : EventCommand {
  public string prefab;
  public bool jumpable;
  public float speed;
}

public class SetAnimatorTriggerEventCommand : EventCommand {
  public string animator;
  public string variableName;
}

public class SetActiveEventCommand : EventCommand {
  public string target;
  public bool state;
}

public class ShootSinusObstacleEventCommand : EventCommand {
  public string prefab;
  public bool jumpable;
  public float speed;
  public float waveHeightX;
  public float waveHeightY;
  public float waveSpeedX;
  public float waveSpeedY;
  public bool customStartTimeSinWave = false;
  public float startTimeSinWaveX;
  public float startTimeSinWaveY;
}

public class PlayParticleEventCommand : EventCommand {
  public string particleSystem;
}

public class CreateAssetBundles {
  [MenuItem("Assets/Generate chart data")]
  static void GenerateChartJson() {
    var notes = new List<Note>();
    var notesGO = GameObject.Find("NOTES");
    foreach (Transform note in notesGO.transform) {
      var _event = note.GetChild(0).gameObject;
      var n = new Note();
      n.noteID = _event.GetComponent<EverhoodModding.NoteHolder>().noteID;
      n.events = new List<EventCommand>();

      ReadEvents(_event, n.events);

      notes.Add(n);
    }
    
    var sections = new List<Section>();
    var sectionsGO = GameObject.Find("SECTIONS");
    foreach (Transform section in sectionsGO.transform) {
      var _event = section.gameObject;
      var s = new Section();
      s.sectionID = _event.GetComponent<EverhoodModding.SectionHolder>().sectionID;
      s.events = new List<EventCommand>();

      ReadEvents(_event, s.events);

      sections.Add(s);
    }

    using (StreamWriter sw = new StreamWriter("Assets/noteData.txt")) sw.Write(Serialize(notes));
    using (StreamWriter sw = new StreamWriter("Assets/sectionData.txt")) sw.Write(Serialize(sections));
  }

  private static void ReadEvents(GameObject go, List<EventCommand> events) {
    // SetAnimatorTriggerEventCommand
    var sats = go.GetComponents<EverhoodModding.SetAnimatorTriggerEventCommand>();
    if (sats.Length > 0) {
      foreach (var sat in sats) {
        events.Add(new SetAnimatorTriggerEventCommand {
          eventName = "SetAnimatorTriggerEventCommand",
          animator = sat.animator.gameObject.name,
          variableName = sat.triggerVariableName
        });
      }
    }

    // SetActiveEventCommmand
    var sas = go.GetComponents<EverhoodModding.SetActiveEventCommand>();
    if (sas.Length > 0) {
      foreach (var sa in sas) {
        events.Add(new SetActiveEventCommand {
          eventName = "SetActiveEventCommand",
          target = sa.target.name,
          state = sa.state
        });
      }
    }

    // ShootObstacleEventCommand
    var sos = go.GetComponents<EverhoodModding.ShootObstacle>();
    if (sos.Length > 0) {
      foreach (var so in sos) {
        events.Add(new ShootObstacleEventCommand {
          eventName = "ShootObstacleEventCommand",
          prefab = so.prefab.name,
          jumpable = so.jumpable,
          speed = so.speed
        });
      }
    }

    // ShootSinusObstacleEventCommand
    var ssos = go.GetComponents<EverhoodModding.ShootSinusObstacle>();
    if (ssos.Length > 0) {
      foreach (var sso in ssos) {
        events.Add(new ShootSinusObstacleEventCommand {
          eventName = "ShootSinusObstacleEventCommand",
          prefab = sso.prefab.name,
          jumpable = sso.jumpable,
          speed = sso.speed,
          waveHeightX = sso.waveHeightX,
          waveHeightY = sso.waveHeightY,
          waveSpeedX = sso.waveSpeedX,
          waveSpeedY = sso.waveSpeedY,
          customStartTimeSinWave = sso.customStartTimeSinWave,
          startTimeSinWaveX = sso.startTimeSinWaveX,
          startTimeSinWaveY = sso.startTimeSinWaveY
        });
      }
    }

    // PlayParticleEventCommand
    var pps = go.GetComponents<EverhoodModding.PlayParticleEventCommand>();
    if (pps.Length > 0) {
      foreach (var pp in pps) {
        events.Add(new PlayParticleEventCommand {
          eventName = "PlayParticleEventCommand",
          particleSystem = pp.particleSystem.gameObject.name
        });
      }
    }
  }

  [MenuItem("Assets/Export CB to AssetBundle")]
  static void BuildAllAssetBundles() {
    BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
  }

  private static string Serialize(List<Note> notes) {
    string data = "";
    foreach (Note note in notes) {
      data += $"{note.noteID},{note.events.Count}\n";
      foreach (var evt in note.events) {
        data += $"{evt.eventName},";
        foreach (FieldInfo field in evt.GetType().GetFields()) if (field.Name != "eventName") data += $"{field.GetValue(evt)},";
        data.Remove(data.Length - 1);
        data += "\n";
      }
    }
    return data;
  }

  private static string Serialize(List<Section> sections) {
    string data = "";
    foreach (Section section in sections) {
      data += $"{section.sectionID},{section.events.Count}\n";
      foreach (var evt in section.events) {
        data += $"{evt.eventName},";
        foreach (FieldInfo field in evt.GetType().GetFields()) if (field.Name != "eventName") data += $"{field.GetValue(evt)},";
        data.Remove(data.Length - 1);
        data += "\n";
      }
    }
    return data;
  }
}