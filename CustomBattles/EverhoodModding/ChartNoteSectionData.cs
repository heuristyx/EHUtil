using System;
using System.Collections.Generic;

namespace ChartNoteSectionData
{
	public class Data
	{
		public Data()
		{
		}

		[Serializable]
		public struct Note
		{
			public Note(int noteID, float noteTick, int noteCorridorID, float noteDurationTick, string noteTypeID, int noteColorID)
			{
				this = default(Data.Note);
				this.noteID = noteID;
				this.noteTick = noteTick;
				this.noteCorridorID = noteCorridorID;
				this.noteDurationTick = noteDurationTick;
				this.noteTypeID = noteTypeID;
				this.noteColorID = noteColorID;
			}

			public int noteID;

			public float noteTick;

			public int noteCorridorID;

			public float noteDurationTick;

			public string noteTypeID;

			public int noteColorID;
		}

		[Serializable]
		public struct Section
		{
			public Section(string sectionID, float sectionTick)
			{
				this.sectionID = sectionID;
				this.sectionTick = sectionTick;
			}

			public string sectionID;

			public float sectionTick;
		}

		[Serializable]
		public class ChartData
		{
			public ChartData()
			{
			}

			public string songName;

			public int resolution;

			public int bpm;

			public float tick;

			public List<Data.Note> notes = new List<Data.Note>();

			public List<Data.Section> sections = new List<Data.Section>();
		}
	}
}
