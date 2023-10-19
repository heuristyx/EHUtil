using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace EHUtil;

public static class LoadEverhoodMod {
  public static object modHost;

  public static void Load() {
    Uri path = new Uri(Path.Combine(EHUtil.PluginInfo.Location, @"..\CustomBattles", "XEPHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA.everhoodMod"));
    var ModType = EHUtil.UMod.GetType("UMod.Mod");
    modHost = ModType.GetMethod("Load").Invoke(null, new object[]{path, false});
  }

  public static void Activate() {
    EHUtil.UMod.GetType("UMod.ModHost").GetMethod("Activate").Invoke(modHost, new object[]{});

    // var assets = EHUtil.UMod.GetType("UMod.ModHost").GetProperty("Assets").GetValue(modHost);
    // var value = assets.GetType().GetProperty("AssetCount").GetValue(assets);
    // EHUtil.Log.LogInfo($"Assets: {value}");
    // var sourcesProp = assets.GetType().GetProperty("AssetSources", BindingFlags.NonPublic | BindingFlags.Instance);
    // EHUtil.Log.LogInfo(sourcesProp != null);
    // var sources = sourcesProp.GetValue(assets);
    // foreach (var source in (IEnumerable)sources) {
    //   EHUtil.Log.LogInfo(source.GetType().GetProperty("FullName").GetValue(source));
    // }

    var scenes = EHUtil.UMod.GetType("UMod.ModHost").GetProperty("Scenes").GetValue(modHost);
    var scene = scenes.GetType().GetProperty("DefaultScene").GetValue(scenes);
    scene.GetType().GetMethod("Load").Invoke(scene, new object[]{false});
  }
}