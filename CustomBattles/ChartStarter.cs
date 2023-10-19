using System.Collections;
using Everhood;
using Everhood.Chart;
using UnityEngine;

namespace EHUtil;

public class ChartStarter : MonoBehaviour {
  public Chart chart;
  public AudioClip clip;

  private void Awake() {
    StartCoroutine(StartChart());
  }

  private IEnumerator StartChart() {
    yield return new WaitForSeconds(0.5f);
    var cr = GetComponent<Everhood.Chart.ChartReader>();
    cr._addresableLoaded = true;
    cr.audioSource.outputAudioMixerGroup = SettingsHandler.GetInstance().BattleMusic;
    cr.audioSource.clip = clip;
    cr._notes = chart.notes;
    cr._sections = chart.sections;
    cr._ticks = chart.tick;
    cr.enabled = true;
    var co = GetComponent<ChartObserver>();
    co.FindObservers();
    cr.chartObserver = co;

    EHUtil.Log.LogWarning("Starting chartreader");
    cr.StartChartReader(0.5f);
  }
}