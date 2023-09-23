using System.IO;

namespace EHUtil;

public static class ChartEncoder {
  public static void Encode(Everhood.Chart.ChartReader cr) {
    string path = Path.Combine(Path.GetDirectoryName(EHUtil.PluginInfo.Location), $"{cr.chart.songName}.chart");
    string file = "[Song]\n{\n";
    file += $"  Resolution = {cr.chart.resolution}";
    file +=  "\n}\n[SyncTrack]\n{\n";
    file +=  "  0 = TS 4\n";
    file += $"  0 = B {cr.chart.bpm * 1000}";
    file +=  "\n}\n[Events]\n{\n";
    foreach (var s in cr._sections)
      file += $"  {s.sectionTick} = E \"{s.sectionID}\"\n";
    file +=  "}\n[ExpertSingle]\n{\n";
    foreach (var n in cr._notes)
      file += $"  {n.noteTick} = N {n.noteCorridorID} 0 {n.noteID}\n";
    file += "}";
    using (StreamWriter sw = new StreamWriter(path)) sw.Write(file);
  }
}