using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Everhood;
using Everhood.Battle;
using Everhood.Chart;
using Fungus;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EHUtil;

public static class CustomBattles {
  public static GameObject currentSceneContainer;
  public static GameObject player;
  public static List<Animator> animators;
  public static List<ParticleSystem> particleSystems;
  public static Everhood.Chart.Chart currentChart;
  public static VirtualPathData genericVPD;

  public static void Load() {
    EHUtil.loadCustomBattle = true;
    EHUtil.UMod.GetType("UMod.ModHost").GetMethod("Activate").Invoke(LoadEverhoodMod.modHost, new object[]{});
    SceneManager.LoadScene("AtmBattle", LoadSceneMode.Additive);
  }

  // public static void Init() {
  //   GameObject chartObj = new GameObject("Chart");
  //   chartObj.SetActive(false); // Halt updates
  //   Everhood.Chart.ChartReader cr = chartObj.AddComponent<Everhood.Chart.ChartReader>();
  //   // var assets = LoadEverhoodMod.modHost.GetType().GetProperty("Assets").GetValue(LoadEverhoodMod.modHost);
  //   // var assetList = assets.GetType().GetMethod("FindAllWithExtension").Invoke(assets, new object[]{".chart"});
  //   // foreach (var asset in (IEnumerable)assetList) {
  //   //   EHUtil.Log.LogInfo(asset.GetType().GetProperty("Name").GetValue(asset));
  //   // }

  //   cr.chart = ChartEncoder.Decode(Path.Combine(EHUtil.PluginInfo.Location, @"..\Xephaaaa Finale 333.chart"));
  //   AudioSource theaudioSources = chartObj.AddComponent<AudioSource>();
  //   cr.audioSource = theaudioSources;
  //   cr.audioSource.clip = // song;
  //   chartObj.SetActive(true);
  // }

  public static void Init() {
    // Get rid of unneeded ATM Battle environments
    EHUtil.Log.LogInfo(GameObject.Find("ATM") != null);
    GameObject.Destroy(GameObject.Find("WORLD"));
    GameObject.Destroy(GameObject.Find("ATM"));
    GameObject.Destroy(GameObject.Find("Projectile_FireBlastpool"));
    GameObject.Destroy(GameObject.Find("Projectile_UnabsorbablePsyBlastpool"));
    GameObject.Destroy(GameObject.Find("Projectile_EnergyBlastpool"));

    GameObject.Destroy(GameObject.Find("Chart starter"));

    // Disable existing cameras
    foreach (var camera in GameObject.FindObjectsOfType<Camera>())
      camera.gameObject.SetActive(false);

    foreach (var obj in GameObject.FindObjectsOfType<SectionEventHandler>())
      GameObject.Destroy(obj);

    var CBManager = new GameObject("CBManager");

    GameObject.Destroy(GameObject.Find("ChartEditor").transform.GetChild(0).gameObject);
    GameObject.Destroy(GameObject.Find("ChartEditor").transform.GetChild(1).gameObject);

    var assets = LoadEverhoodMod.modHost.GetType().GetProperty("Assets").GetValue(LoadEverhoodMod.modHost);
    var assetList = assets.GetType().GetMethod("FindAllWithExtension").Invoke(assets, new object[]{".wav"});
    AudioClip clip = new AudioClip();
    foreach (var asset in (IEnumerable)assetList) {
      EHUtil.Log.LogInfo(asset.GetType().GetProperty("Name").GetValue(asset));
      clip = (AudioClip) asset.GetType().GetProperty("AssetObject").GetValue(asset);
    }

    // Swap charts
    currentChart = ChartEncoder.Decode(Path.Combine(EHUtil.PluginInfo.Location, @"..\Xephaaaa Finale 333.chart"));

    var ce = GameObject.Find("ChartEditor");
    var cechart = ce.GetComponent<Chart>();
    ChartEncoder.CopyChart(currentChart, cechart);
    var cs = ce.AddComponent<ChartStarter>();
    cs.chart = currentChart;
    cs.clip = clip;

    player = GameObject.Find("Player Battle v2");
    // animators = FindChildrenRecursive(sceneContainerClone, new List<Animator>());
    // particleSystems = FindChildrenRecursive(sceneContainerClone, new List<ParticleSystem>());

    genericVPD = ScriptableObject.CreateInstance<NormalVirtualGrid>();
    genericVPD.virtualPath = new VirtualPathData.PathData() {
      StartsPoints = new[] {
        new Vector3(-4.93f, 3.53f, 8f),
        new Vector3(-2.36f, 3.53f, 8f),
        new Vector3(0f, 3.53f, 8f),
        new Vector3(2.36f, 3.53f, 8f),
        new Vector3(4.93f, 3.53f, 8f),
      },
      EndPoints = new[] {
        new Vector3(-4.93f, -2f, -10f),
        new Vector3(-2.36f, -2f, -10f),
        new Vector3(0f, -2f, -10f),
        new Vector3(2.36f, -2f, -10f),
        new Vector3(4.93f, -2f, -10f),
      }
    };

    // SetNoteData();
    // SetSectionData();
    
    foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
      obj.SetActive(true);
    }

    EHUtil.Log.LogInfo("Trying to serialize");
    var type = EHUtil.UModShared.GetType("UMod.Shared.Linker.LinkBehaviour");
    var comps = GameObject.FindObjectsOfType(type);
    var chartdata = new ChartData();
    foreach (var comp in comps) {
      EHUtil.Log.LogInfo($"Serializing type {comp.GetType()} (looking for {EHUtil.UModShared.GetType("UMod.Shared.Linker.LinkBehaviour")})");
      EHUtil.UModShared.GetType("UMod.Shared.Linker.LinkBehaviour").GetMethod("Serialize").Invoke(comp, new object[]{ chartdata });
    }
  }

  // public static void Init(GameObject sceneContainer) {
  //   currentSceneContainer = sceneContainer;
  //   // Get rid of unneeded ATM Battle environments
  //   GameObject.Destroy(GameObject.Find("WORLD"));
  //   GameObject.Destroy(GameObject.Find("ATM"));
  //   GameObject.Destroy(GameObject.Find("Projectile_FireBlastpool"));
  //   GameObject.Destroy(GameObject.Find("Projectile_UnabsorbablePsyBlastpool"));
  //   GameObject.Destroy(GameObject.Find("Projectile_EnergyBlastpool"));

  //   GameObject.Destroy(GameObject.Find("Chart starter"));

  //   // Disable existing cameras
  //   foreach (var camera in GameObject.FindObjectsOfType<Camera>()) camera.gameObject.SetActive(false);

  //   foreach (var obj in GameObject.FindObjectsOfType<SectionEventHandler>())
  //     GameObject.Destroy(obj);

  //   sceneContainerClone = GameObject.Instantiate(sceneContainer);

  //   GameObject.Destroy(GameObject.Find("ChartEditor").transform.GetChild(0).gameObject);
  //   GameObject.Destroy(GameObject.Find("ChartEditor").transform.GetChild(1).gameObject);

  //   // Swap charts
  //   currentChart = ChartEncoder.Decode(Path.Combine(EHUtil.PluginInfo.Location, @"..\Xephaaaa Finale 333.chart"));
    
  //   var ce = GameObject.Find("ChartEditor");
  //   var cechart = ce.GetComponent<Chart>();
  //   ChartEncoder.CopyChart(currentChart, cechart);
  //   ce.AddComponent<ChartStarter>().chart = currentChart;

  //   player = GameObject.Find("Player Battle v2");
  //   animators = FindChildrenRecursive(sceneContainerClone, new List<Animator>());
  //   particleSystems = FindChildrenRecursive(sceneContainerClone, new List<ParticleSystem>());

  //   genericVPD = ScriptableObject.CreateInstance<NormalVirtualGrid>();
  //   genericVPD.virtualPath = new VirtualPathData.PathData() {
  //     StartsPoints = new[] {
  //       new Vector3(-4.93f, 3.53f, 8f),
  //       new Vector3(-2.36f, 3.53f, 8f),
  //       new Vector3(0f, 3.53f, 8f),
  //       new Vector3(2.36f, 3.53f, 8f),
  //       new Vector3(4.93f, 3.53f, 8f),
  //     },
  //     EndPoints = new[] {
  //       new Vector3(-4.93f, -2f, -10f),
  //       new Vector3(-2.36f, -2f, -10f),
  //       new Vector3(0f, -2f, -10f),
  //       new Vector3(2.36f, -2f, -10f),
  //       new Vector3(4.93f, -2f, -10f),
  //     }
  //   };

  //   SetNoteData();
  //   SetSectionData();
  // }

  private static void SetNoteData() {
    var notes = GameObject.Find("Chart Data").transform.GetChild(0);
    var notesCount = notes.transform.childCount;
    for (int i = 0; i < notesCount; i++) {
      var child = notes.transform.GetChild(i).GetChild(0).gameObject;
      child.SetActive(false);
      child.AddComponent<EventCommandsGroupExecutor>();
      var neh = child.AddComponent<NoteEventHandler>();
      neh.noteID = int.Parse(Assets.chartData.notes[i].ID);
      foreach (EventCommand evt in Assets.chartData.notes[i].events)
        AddEvent(evt, child);
      child.SetActive(true);
    }
  }

  private static void SetSectionData() {
    var sections = GameObject.Find("Chart Data").transform.GetChild(1);
    var sectionsCount = sections.transform.childCount;
    for (int i = 0; i < sectionsCount; i++) {
      var child = sections.transform.GetChild(i).gameObject;
      child.SetActive(false);
      child.AddComponent<EventCommandsGroupExecutor>();
      var seh = child.AddComponent<SectionEventHandler>();
      seh.sectionID = " " + Assets.chartData.sections[i].ID;
      foreach (EventCommand evt in Assets.chartData.sections[i].events)
        AddEvent(evt, child);
      child.SetActive(true);
    }
  }

  private static void AddEvent(EventCommand evt, GameObject obj) {
    switch (evt.eventName) {
      case "ShootObstacleEventCommand":
      case "ShootSinusObstacleEventCommand":
        var sp = obj.AddComponent<ShootProjectileEventCommand>();
        sp.speed = (evt as ShootObstacleEventCommand).speed;
        var prefab = Assets.cbBundle.LoadAsset<GameObject>((evt as ShootObstacleEventCommand).prefab);
        sp.projectilPrefab = prefab;
        sp.projectilePoolSize = 100; // TO-DO: using chart data to determine optimal poolsize
        sp.target = player;
        var dpc = obj.AddComponent<DamagePlayerCollisionEvent>();
        dpc.player = player.GetComponent<BattlePlayer>();
        dpc.SetJumpable((evt as ShootObstacleEventCommand).jumpable);
        sp.collisionEvent = dpc;
        sp.noteEventHandler = obj.GetComponent<NoteEventHandler>();
        sp.virtualPathData = genericVPD;

        if (evt.eventName == "ShootSinusObstacleEventCommand") {
          var _evt = (ShootSinusObstacleEventCommand) evt;
          sp.sinusMovement = true;
          sp.waveHeightX = _evt.waveHeightX;
          sp.waveHeightY = _evt.waveHeightY;
          sp.waveSpeedX = _evt.waveSpeedX;
          sp.waveSpeedY = _evt.waveSpeedY;
          sp.customStartTimeSinWave = _evt.customStartTimeSinWave;
          if (_evt.customStartTimeSinWave) {
            sp.startTimeSinWaveX = _evt.startTimeSinWaveX;
            sp.startTimeSinWaveY = _evt.startTimeSinWaveY;
          }
        }
        break;
      case "SetAnimatorTriggerEventCommand":
        var sat = obj.AddComponent<Everhood.SetAnimatorTriggerEventCommand>();
        var animator = animators.First(a => a.gameObject.name == (evt as SetAnimatorTriggerEventCommand).animator);
        bool disabled = !animator.gameObject.activeSelf;
        if (disabled) animator.gameObject.SetActive(true);
        sat.animator = animator;
        sat.triggerVariableName = (evt as SetAnimatorTriggerEventCommand).variableName;
        if (disabled) animator.gameObject.SetActive(false);
        break;
      case "SetActiveEventCommand":
        var sa = obj.AddComponent<Everhood.SetActiveEventCommand>();
        //sa.target = FindByNameRecursive(sceneContainerClone, (evt as SetActiveEventCommand).target);
        sa.state = (evt as SetActiveEventCommand).state;
        break;
      case "PlayParticleEventCommand":
        var pp = obj.AddComponent<Everhood.PlayParticleEventCommand>();
        pp.particleSystem = particleSystems.First(a => a.gameObject.name == (evt as PlayParticleEventCommand).particleSystem);
        break;
      default:
        break;
    }
  }

  private static List<T> FindChildrenRecursive<T>(GameObject parent, List<T> result) {
    foreach (Transform transform in parent.transform) {
      var comp = transform.gameObject.GetComponent<T>();
      if (comp != null) result.Add(comp);
      if (transform.childCount > 0) FindChildrenRecursive(transform.gameObject, result);
    }
    return result;
  }

  private static GameObject FindByNameRecursive(GameObject parent, string name) {
    foreach (Transform transform in parent.transform) {
      if (transform.gameObject.name == name) return transform.gameObject;
      if (transform.childCount > 0) {
        var obj = FindByNameRecursive(transform.gameObject, name);
        if (obj != null) return obj;
      }
    }
    return null;
  }
}