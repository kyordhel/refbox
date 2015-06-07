using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RefBox
{
	public static class Loader
	{
		/// <summary>
		/// Retrieves the list of teams from teams.txt
		/// </summary>
		/// <returns>A list of teams</returns>
		public static List<string> LoadTeams()
		{
			List<string> teams;
			try
			{
				teams = LoadTextFile("teams.txt");
			}
			catch
			{
				teams = new List<string>();
			}
			if (teams.Count < 1)
				teams.Add("Team");
			return teams;
		}

		/// <summary>
		/// Retrieves the list of tests from tests.txt
		/// </summary>
		/// <returns>A list of tests</returns>
		public static List<TestInfo> LoadTests()
		{
			List<TestInfo> tests = new List<TestInfo>();
			try
			{
				List<string> lines = LoadTextFile("tests.txt");
				foreach (string line in lines)
				{
					TestInfo ti;
					if (TestInfo.TryParse(line, out ti))
						tests.Add(ti);
				}
			}
			catch { }
			if (tests.Count < 1)
				tests.Add(new TestInfo("Test", 5));
			return tests;
		}

		/// <summary>
		/// Loads a text file removing empty lines and comments startin with #
		/// </summary>
		/// <param name="path">The path of the file to load</param>
		/// <returns>A list containing all the lines in the file</returns>
		public static List<string> LoadTextFile(string path)
		{
			string[] lines = File.ReadAllText(path).Split('\r', '\n');
			List<string> acceptedLines = new List<string>(lines.Length);
			for (int i = 0; i < lines.Length; ++i)
			{
				lines[i] = lines[i].Trim();
				int ix = lines[i].IndexOf('#');
				if (ix >= 0)
					lines[i] = lines[i].Substring(ix).Trim();
				if (lines[i].Length > 0)
					acceptedLines.Add(lines[i]);
			}
			return acceptedLines;
		}
	}
}
