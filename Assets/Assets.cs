using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EHUtil;

public static class Assets {
  public static AssetBundle bundle;
  public static AssetBundle cbBundle;
  public const string bundleName = "ehutil";
  public const string assetBundleFolder = "assets";

  public static GameObject checkpointBar;
  public static GameObject sceneContainer;
  public static AudioClip audioClip;
  public static ChartData chartData = new ChartData();
  public static string battleTemplateScene;

  public static string AssetBundlePath {
    get {
      return Path.Combine(Path.GetDirectoryName(EHUtil.PluginInfo.Location), assetBundleFolder, bundleName);
    }
  }

  public static void Init() {
    bundle = AssetBundle.LoadFromFile(AssetBundlePath);

    checkpointBar = bundle.LoadAsset<GameObject>("line");

    // cbBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(EHUtil.PluginInfo.Location), assetBundleFolder, "assetbundle"));
    // sceneContainer = cbBundle.LoadAsset<GameObject>("SceneContainer");
    // audioClip = cbBundle.LoadAllAssets<AudioClip>()[0];

    // string[] noteData = cbBundle.LoadAsset<TextAsset>("noteData").text.Split(new[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
    // chartData.notes = DeserializeChartData(noteData);

    // string[] sectionData = cbBundle.LoadAsset<TextAsset>("sectionData").text.Split(new[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
    // chartData.sections = DeserializeChartData(sectionData);
  }

  // Cannot use Unity's deserialize with modded types, so here's a terrible homebrewed deserializer
  public static List<ChartDataObject> DeserializeChartData(string[] data) {
    int eventCount = 0;
    var list = new List<ChartDataObject>();
    ChartDataObject obj = new ChartDataObject();
    foreach (string line in data) {
      string[] segments = line.Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries);
      if (eventCount == 0) {
        obj = new ChartDataObject();
        obj.ID = segments[0];
        eventCount = int.Parse(segments[1]);
      } else {
        string eventName = segments[0];
        EventCommand evt = new EventCommand();
        switch (eventName) {
          case "ShootObstacleEventCommand":
            evt = new ShootObstacleEventCommand {
              eventName = eventName,
              prefab = segments[1],
              jumpable = bool.Parse(segments[2]),
              speed = float.Parse(segments[3])
            };
            break;
          case "ShootSinusObstacleEventCommand":
            evt = new ShootSinusObstacleEventCommand {
              eventName = eventName,
              prefab = segments[1],
              jumpable = bool.Parse(segments[2]),
              speed = float.Parse(segments[3]),
              waveHeightX = float.Parse(segments[4]),
              waveHeightY = float.Parse(segments[5]),
              waveSpeedX = float.Parse(segments[6]),
              waveSpeedY = float.Parse(segments[7]),
              customStartTimeSinWave = bool.Parse(segments[8]),
              startTimeSinWaveX = float.Parse(segments[9]),
              startTimeSinWaveY = float.Parse(segments[10])
            };
            break;
          case "SetAnimatorTriggerEventCommand":
            evt = new SetAnimatorTriggerEventCommand {
              eventName = eventName,
              animator = segments[1],
              variableName = segments[2]
            };
            break;
          case "SetActiveEventCommand":
            evt = new SetActiveEventCommand {
              eventName = eventName,
              target = segments[1],
              state = bool.Parse(segments[2])
            };
            break;
          case "PlayParticleEventCommand":
            evt = new PlayParticleEventCommand {
              eventName = eventName,
              particleSystem = segments[1]
            };
            break;
          default:
            break;
        }
        obj.events.Add(evt);
        eventCount--;
      }

      if (eventCount == 0) list.Add(obj);
    }

    return list;
  }
}