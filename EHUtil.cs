using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using Evergreen;
using System;
using Everhood.Battle;
using Doozy.Engine.Extensions;

using static EHUtilConfig.Config;
using System.Collections;
using Fungus;
using System.Linq;
using UnityEngine.SceneManagement;

namespace EHUtil;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency("com.heuristyx.plugins.evergreen")]
public class EHUtil : BaseUnityPlugin
{
  public static PluginInfo PluginInfo { get; private set; }

  public const string Guid = "com.heuristyx.plugins.ehutil";
  public const string Name = "EHUtil";
  public const string Version = "1.1.0";

  internal static TASModeController TASController;
  internal static CoroutineController CoroutineController;
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

    Main.Init();

    CoroutineController = this.gameObject.AddComponent<CoroutineController>();
    TASController = this.gameObject.AddComponent<TASModeController>();

    var UIgo = new GameObject("EHUtil UI");
    UIgo.transform.SetParent(TextDrawing.GetCanvas().transform);
    UI = UIgo.AddComponent<EHUtilUI>();
    UIgo.AddComponent<RectTransform>().FullScreen(true);

    Evergreen.Evergreen.OnToggleDebugMode += (self, state) => { UI.DebugUI.gameObject.SetActive(state); };

    Evergreen.Evergreen.ToggleDebugMode(false);
  }

  private void Update()
  {
    if (Evergreen.Evergreen.IsBaseGame)
    {
      if (SceneManager.GetActiveScene().name == "IntroMenu" && skipIntro.Value)
        SceneManager.LoadScene("MainMenu");
    }

    InputHandler.HandleInput();
  }
}
