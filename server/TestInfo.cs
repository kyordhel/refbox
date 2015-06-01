using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RefBox
{
	public class TestInfo
	{
		private string name;
		private TimeSpan duration;

		/// <summary>
		/// Initializes a new instance of TestInfo
		/// </summary>
		private TestInfo() { }

		/// <summary>
		/// Initializes a new instance of TestInfo
		/// </summary>
		/// <param name="name">The name of the test</param>
		/// <param name="duration">The duration of the test</param>
		public TestInfo(string name, TimeSpan duration)
		{
			this.Name = name;
			this.Duration = duration;
		}

		/// <summary>
		/// Initializes a new instance of TestInfo
		/// </summary>
		/// <param name="name">The name of the test</param>
		/// <param name="m">The accumulative duration of the test in minutes</param>
		public TestInfo(string name, int m)
			: this(name, new TimeSpan(0, m, 0)) { }

		/// <summary>
		/// Initializes a new instance of TestInfo
		/// </summary>
		/// <param name="name">The name of the test</param>
		/// <param name="h">The accumulative duration of the test in hours</param>
		/// <param name="m">The accumulative duration of the test in minutes</param>
		/// <param name="s">The accumulative duration of the test in seconds</param>
		public TestInfo(string name, int h, int m, int s)
			: this(name, new TimeSpan(h, m, s)){}

		/// <summary>
		/// Gets the name of the test
		/// </summary>
		public string Name { 
			get{return this.name;}
			protected set
			{
				if (String.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "Test name cannot be null nor empty");
				this.name = value;
			}
		}

		/// <summary>
		/// Gets the duration of the test
		/// </summary>
		public TimeSpan Duration {
			get { return this.duration; }
			protected set
			{
				if ((value.TotalSeconds < 30) || (value.TotalSeconds > 86400))
					throw new ArgumentOutOfRangeException("value", "Test Time must be between 30 seconds and 1 day (86400 secs)");
				this.duration = value;
			}
		}

		/// <summary>
		/// Gets the duration of the test in seconds
		/// </summary>
		public int DurationMin
		{
			get { return (int)this.duration.TotalSeconds; }
			protected set
			{
				if ((value < 1) || (value > 1440))
					throw new ArgumentOutOfRangeException("value", "Test Time must be between 30 seconds and 1 day (86400 secs)");
				this.duration = new TimeSpan(0, value, 0);
			}
		}

		/// <summary>
		/// Gets the duration of the test in seconds
		/// </summary>
		public int TestTime
		{
			get { return (int)this.duration.TotalSeconds; }
			protected set
			{
				if ((value < 30) || (value > 86400))
					throw new ArgumentOutOfRangeException("value", "Test Time must be between 30 seconds and 1 day (86400 secs)");
				this.duration = new TimeSpan(0, 0, value);
			}
		}

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
