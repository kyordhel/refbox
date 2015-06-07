using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RefBox.Terminal
{
	public delegate void StringEventHandler(string s);

	/// <summary>
	/// Encapsulates all console commands
	/// </summary>
	public class Kernel
	{
		#region Variables

		/// <summary>
		/// The RefBox itself
		/// </summary>
		private Refbox refbox;

		/// <summary>
		/// Command dictionary
		/// </summary>
		private Dictionary<string, StringEventHandler> commands;

		/// <summary>
		/// Regular expression used to split commands
		/// </summary>
		private Regex rxCommandSplitter;

		/// <summary>
		/// Handles console input
		/// </summary>
		private ConsoleManager consoleManager;

		/// <summary>
		/// Stores the list of teams
		/// </summary>
		private List<string> teams;

		/// <summary>
		/// Stores the list of tests
		/// </summary>
		private SortedList<string, TestInfo> tests;

		/// <summary>
		/// Stores the current team
		/// </summary>
		private string currentTeam;

		/// <summary>
		/// Stores the current test
		/// </summary>
		private TestInfo currentTest;

		#endregion

		#region Consutructor

		public Kernel(Refbox refbox)
		{
			this.refbox = refbox;
			this.commands = new Dictionary<string, StringEventHandler>();
			this.rxCommandSplitter = new Regex(@"^(?<cmd>\w+)(\s+(?<par>.*))?$");
			teams = Loader.LoadTeams();
			List<TestInfo> tests = Loader.LoadTests();
			this.tests = new SortedList<string, TestInfo>(tests.Count);
			foreach (TestInfo t in tests)
				this.tests.Add(t.Name, t);
			RegisterCommands();
		}

		#endregion

		#region Properties

		/// <summary>
		/// The console manager (used to enable/disable log mode
		/// </summary>
		public ConsoleManager ConsoleManager
		{
			get { return this.consoleManager; }
			set { this.consoleManager = value; }
		}

		#endregion

		#region Methods

		public bool Execute(string command)
		{
			Match m = rxCommandSplitter.Match(command);
			if (!m.Success)
				return false;
			string c = m.Result("${cmd}");
			string p = m.Result("${par}");
			if (!this.commands.ContainsKey(c))
				return false;
			this.commands[c](p);
			return true;
		}

		internal void FillCompletionTree(CompletionTree completionTree)
		{
			foreach (string command in this.commands.Keys)
			{
				completionTree.AddWord(command);
				completionTree.AddWord(String.Format("help {0}", command));
			}
			completionTree.AddWord("list tests");
			completionTree.AddWord("list teams");
			foreach (string team in teams)
			{
				completionTree.AddWord(String.Format("team {0}", team));
			}
			foreach (string test in tests.Keys)
			{
				completionTree.AddWord(String.Format("test {0}", test));
			}
		}

		private string FormatTime(int seconds)
		{
			int min = seconds / 60;
			int sec = seconds % 60;
			string smin = min.ToString().PadLeft(2, '0');
			string ssec = sec.ToString().PadLeft(2, '0');
			return smin + ":" + ssec;
		}

		public void RegisterCommands()
		{
			commands.Add("continue", new StringEventHandler(ContinueCommand));
			commands.Add("exit", new StringEventHandler(QuitCommand));
			commands.Add("help", new StringEventHandler(HelpCommand));
			commands.Add("info", new StringEventHandler(InfoCommand));
			commands.Add("list", new StringEventHandler(ListCommand));
			commands.Add("quit", new StringEventHandler(QuitCommand));
			commands.Add("start", new StringEventHandler(StartCommand));
			commands.Add("stop", new StringEventHandler(StopCommand));
			commands.Add("restart", new StringEventHandler(RestartCommand));
			commands.Add("team", new StringEventHandler(TeamCommand));
			commands.Add("test", new StringEventHandler(TestCommand));
			commands.Add("time", new StringEventHandler(TimeCommand));
		}

		private void WriteCommandOptions(string command, params string[] options)
		{
			ConsoleColor color = Console.ForegroundColor;
			foreach (string opt in options)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\t{0} ", command);
				Console.ForegroundColor = color;
				Console.WriteLine(opt);
			}
		}

		#endregion

		#region General commands

		public void ContinueCommand(string s)
		{
			if (!this.refbox.TestStarted)
			{
				WriteError("Test is not running");
				return;
			}
			refbox.SendContinueText(s);
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("CONTINUE Sent: {0}", s);
			Console.ForegroundColor = color;
			Console.WriteLine();
		}

		public void HelpCommand(string s)
		{
			Help.ShowHelp(s, this.commands.Keys);
		}

		public void InfoCommand(string s)
		{
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			if (this.refbox.TestStarted)
			{
				Console.Write("\tTest: {0}. ", refbox.TestInfo.Name);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Running");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("\tTeam: {0}.", refbox.TeamName);
				Console.WriteLine("\tTime: {0}", FormatTime(refbox.ElapsedTime));
			}
			else
			{
				Console.Write("\tTest: {0}. ", currentTest);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Not running");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\tTeam: {0}.", currentTeam);
			}
			Console.ForegroundColor = color;
			Console.WriteLine();
		}

		public void ListCommand(string s)
		{
			if (String.IsNullOrEmpty(s))
			{
				WriteCommandOptions("list", "teams", "tests");
				return;
			}

			switch (s.ToLower())
			{
				case "teams":
					ListTeams();
					return;

				case "tests":
					ListTests();
					return;
			}
		}

		public void QuitCommand(string s)
		{
			refbox.StopTest();
			Environment.Exit(-1);
		}

		public void RestartCommand(string s)
		{
			if (this.refbox.TestStarted)
			{
				Console.WriteLine("Test already started");
				return;
			}
			refbox.RestartTest();
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("Restarting {0} for {1} on {3}", refbox.TestInfo.Name, refbox.TeamName, FormatTime(refbox.ElapsedTime));
			Console.ForegroundColor = color;
			Console.WriteLine();
		}

		public void StartCommand(string s)
		{
			if (this.refbox.TestStarted)
			{
				WriteError("Test already started");
				return;
			}
			if (String.IsNullOrEmpty(currentTeam))
			{
				WriteError("Set a team first using the team command");
				return;
			}
			if (String.IsNullOrEmpty(currentTeam))
			{
				WriteError("Set a test first using the test command");
				return;
			}
			refbox.PrepareTest(currentTest, currentTeam);
			refbox.StartTest();
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("Starting {0} for {1}", refbox.TestInfo.Name, refbox.TeamName);
			Console.ForegroundColor = color;
			Console.WriteLine();
			TimeCommand(s);
		}

		public void StopCommand(string s)
		{
			if (!this.refbox.TestStarted)
			{
				WriteError("Test is not running");
				return;
			}
			refbox.StopTest();
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("Stopped {0} for {1}", refbox.TestInfo.Name, refbox.TeamName);
			Console.ForegroundColor = color;
			Console.WriteLine();
		}

		public void TimeCommand(string s)
		{
			if (!this.refbox.TestStarted)
			{
				WriteError("Test is not running");
				return;
			}
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("Remaining test time: {0} (Elapsed: {1})", FormatTime(refbox.RemainingTime), FormatTime(refbox.ElapsedTime));
			Console.ForegroundColor = color;
			Console.WriteLine();
		}

		public void TestCommand(string s)
		{
			if (this.refbox.TestStarted)
			{
				WriteError("Can't define a new test, {0} is running for {1}", refbox.TestInfo.Name, refbox.TeamName);
				return;
			}
			if (!this.tests.ContainsKey(s))
			{
				WriteError("Unknown test. Aborting.");
				return;
			}
			currentTest = tests[s];
			refbox.PrepareTest(tests[s]);
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("Current test changed to {0}", currentTest.Name);
			Console.ForegroundColor = color;
			Console.WriteLine();
		}

		public void TeamCommand(string s)
		{
			if (this.refbox.TestStarted)
			{
				WriteError("Can't define a new team, {0} is running for {1}", refbox.TestInfo.Name, refbox.TeamName);
				return;
			}
			if (!this.teams.Contains(s))
			{
				WriteError("Unknown team. Aborting.");
				return;
			}
			currentTeam = s;
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("Current team changed to {0}", s);
			Console.ForegroundColor = color;
			Console.WriteLine();
		}

		#endregion

		#region List Commands

		public void ListTeams()
		{
			foreach (string team in teams)
				WriteTeam(team);

			Console.WriteLine();
		}

		private void ListTests()
		{
			foreach (TestInfo test in tests.Values)
			{
				WriteTest(test);
				Console.WriteLine();
			}
		}

		#endregion

		private void WriteTeam(string team)
		{
			Console.Write("\t{0}", team);
		}

		private void WriteTest(TestInfo test)
		{
			Console.Write("\t{0}{1}min", test.Name.PadRight(20, ' '), test.DurationMin);
		}

		private void WriteError(string s)
		{
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine(s);
			Console.ForegroundColor = color;
		}

		private void WriteError(string format, params object[] args)
		{
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine(format, args);
			Console.ForegroundColor = color;
		}
	}
}