using System.Collections;
using System.Linq;
using Fungus;
using UnityEngine;

namespace EHUtil;

public class CoroutineController : MonoBehaviour
{
  public void SkipToIncineratorPartTwo()
  {
    StartCoroutine(DoSkipToIncineratorPartTwo());
  }

  private IEnumerator DoSkipToIncineratorPartTwo()
  {
    yield return new WaitForSeconds(.1f);
    var list = GameObject.FindObjectsOfType<Flowchart>().First(f => f.gameObject.name == "EventFlowChart").FindBlock("Start").CommandList;
    list.RemoveRange(13, 8); // Remove Red looking around in the cutscene
    yield return new WaitForSeconds(.5f);
    var readers = GameObject.FindObjectsOfType<Everhood.Chart.ChartReader>();
    foreach (var reader in readers)
      if (reader.gameObject.name == "ChartEditor Incinerator part 2") reader.StartChartReader();
  }
}