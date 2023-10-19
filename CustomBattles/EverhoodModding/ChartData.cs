using System;
using ChartNoteSectionData;
using UnityEngine;

namespace EverhoodModding
{
	public class ChartData : MonoBehaviour
	{
		public ChartData()
		{
		}

		public const string notesParentName = "NOTES";

		public const string sectionsParentName = "SECTIONS";

		public AudioClip song;

		public Data.ChartData chartData;
	}
}
