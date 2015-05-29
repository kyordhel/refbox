using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RefBox
{
	public class TestInfo
	{
		/// <summary>
		/// Initializes a new instance of TestInfo
		/// </summary>
		private TestInfo(){}

		/// <summary>
		/// Initializes a new instance of TestInfo
		/// </summary>
		/// <param name="name">The name of the test</param>
		/// <param name="duration">The duration of the test in minutes</param>
		public TestInfo(string name, int duration)
		{
			this.Name = name;
			this.Duration = new TimeSpan(0, duration, 0);
		}

		/// <summary>
		/// Gets or sets the name of the test
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Gets or sets the duration of the test
		/// </summary>
		public TimeSpan Duration { get; set; }

		public override string ToString()
		{
			return String.Format(
				"{0} [{1}:{2}]",
				this.Name, this.Duration.TotalMinutes,
				this.Duration.Seconds.ToString().PadLeft(2, '0')
			);
		}

		private static Regex rxParser;

		static TestInfo()
		{
			rxParser = new Regex(@"(?<name>\w+(\s+\w+)?)\s+\[?\s*(?<min>\d{1,3}):(?<sec>\d{2})\s*\]?");
		}

		public static bool TryParse(string s, out TestInfo ti)
		{
			ti = new TestInfo("Team", 0);
			Match m = rxParser.Match(s);
			if (!m.Success)
				return false;
			ti.Name = m.Result("${name}");
			int min = Int32.Parse(m.Result("${min}"));
			int sec;
			Int32.TryParse(m.Result("${sec}"), out sec);
			ti.Duration = new TimeSpan(0, min, sec);
			return true;
		}
	}
}
