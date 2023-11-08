using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Evergreen;
using Everhood.Chart;

namespace EHUtil;

public static class ChartEncoder
{
  public static void Encode(Everhood.Chart.ChartReader cr)
  {
    string path = Path.Combine(Path.GetDirectoryName(EHUtil.PluginInfo.Location), $"{cr.chart.songName}.chart");
    string file = "[Song]\n{\n";
    file += $"  Resolution = {cr.chart.resolution}";
    file += "\n}\n[SyncTrack]\n{\n";
    file += "  0 = TS 4\n";
    file += $"  0 = B {cr.chart.bpm * 1000}";
    file += "\n}\n[Events]\n{\n";
    foreach (var s in cr._sections)
      file += $"  {s.sectionTick} = E \"{s.sectionID}\"\n";
    file += "}\n[ExpertSingle]\n{\n";
    foreach (var n in cr._notes)
      file += $"  {n.noteTick} = N {n.noteCorridorID} 0 {n.noteID}\n";
    file += "}";
    using (StreamWriter sw = new StreamWriter(path)) sw.Write(file);
    TextDrawing.DrawToConsole($"Wrote chart to {path}.");
  }

  public static Chart Decode(string path)
  {
    string notesField = "";
    string sectionsField = "";
    int resolution = 0;
    int bpm = 0;
    bool readEvents = false;
    bool readNotes = false;

    using (StreamReader sr = new StreamReader(path))
    {
      string line;
      while ((line = sr.ReadLine()) != null)
      {
        if (line == "}")
        {
          readEvents = false;
          readNotes = false;
        }

        if (line.StartsWith("  Resolution = ")) resolution = int.Parse(line.Substring(15));
        if (line.StartsWith("  0 = B ")) bpm = int.Parse(line.Substring(8)) / 1000;

        if (readEvents) sectionsField += line + "\n";
        if (readNotes) notesField += line + "\n";

        if (line == "[Events]")
        {
          readEvents = true;
          sr.ReadLine();
        }
        if (line == "[ExpertSingle]")
        {
          readNotes = true;
          sr.ReadLine();
        }
      }
    }

    var chart = new Chart();
    chart.notes = chart.GetNotes(notesField);
    chart.sections = GetSectionsFixed(sectionsField);
    chart.bpm = bpm;
    chart.resolution = resolution;
    chart.GetTick();
    chart.songName = "x";

    TextDrawing.DrawToConsole($"Decoded chart at {path}.");

    return chart;
  }

  public static void CopyChart(Chart from, Chart to)
  {
    to.bpm = from.bpm;
    to.notes = from.notes;
    to.sections = from.sections;
    to.resolution = from.resolution;
    to.tick = from.tick;
    to.songName = from.songName;
  }

  public static void HotswapChart(Chart chart, Everhood.Chart.ChartReader cr)
  {
    chart.songAssetRefence = cr.chart.songAssetRefence;
    //cr.chart.songAssetRefence.ReleaseAsset();
    cr.chart = chart;
    cr.Awake();
    TextDrawing.DrawToConsole($"Hotswapped chart.");
  }

  public static List<Everhood.Chart.Section> GetSectionsFixed(string sectionField)
  {
    List<Everhood.Chart.Section> list = new List<Everhood.Chart.Section>();
    string[] array = sectionField.Split(new string[]
    {
        "\r\n",
        "\n"
    }, StringSplitOptions.None);
    char[] separator = new char[]
    {
        ' ',
        '\t'
    };
    for (int i = 0; i < array.Length; i++)
    {
      if (!string.IsNullOrEmpty(array[i]))
      {
        string text = array[i];
        text = string.Join("\n", from s in text.Split(new char[] {
            '\n'
          })
                                 select s.Trim());
        int num = int.Parse(text.Split(separator)[0]);
        int num2 = text.IndexOf('"') + 1;
        int num3 = text.LastIndexOf('"');
        string text2 = text.Substring(num2, num3 - num2);
        text2 = text2.Replace("section", "");
        //text2 = text2.Remove(0, 1);
        Everhood.Chart.Section item = new Everhood.Chart.Section(text2, (float)num);
        list.Add(item);
      }
    }
    return list;
  }
}