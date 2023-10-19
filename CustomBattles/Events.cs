using System.Collections.Generic;

namespace EHUtil;

public class ChartData {
  public List<ChartDataObject> notes;
  public List<ChartDataObject> sections;
}

public class ChartDataObject {
  public string ID;
  public List<EventCommand> events = new();
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

public class ShootSinusObstacleEventCommand : ShootObstacleEventCommand {
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