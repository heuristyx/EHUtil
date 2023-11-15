using Evergreen;
using UnityEngine.SceneManagement;

namespace EHUtil;

public class Commands
{
  public static void Init()
  {
    Evergreen.Commands.RegisterCommand("scene", "Moves to the scene with the given build index", 1, JumpToScene);
  }

  private static void JumpToScene(string[] args)
  {
    if (int.TryParse(args[0], out int index))
    {
      SceneManager.LoadScene(index);
      TextDrawing.DrawToConsole($"Loaded scene {index} ({SceneManager.GetSceneByBuildIndex(index).name})");
    }
    else return;
  }
}