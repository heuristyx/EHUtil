using System.IO;
using UnityEngine;

namespace EHUtil;

public static class Assets {
  public static AssetBundle bundle;
  public const string bundleName = "ehutil";
  public const string assetBundleFolder = "assets";

  public static GameObject checkpointBar;

  public static string AssetBundlePath {
    get {
      return Path.Combine(Path.GetDirectoryName(EHUtil.PluginInfo.Location), assetBundleFolder, bundleName);
    }
  }

  public static void Init() {
    bundle = AssetBundle.LoadFromFile(AssetBundlePath);

    checkpointBar = bundle.LoadAsset<GameObject>("line");
  }
}