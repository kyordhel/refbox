using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Robotics;

namespace RefBox
{
	/// <summary>
	/// Encapsulates all console commands
	/// </summary>
	public class Kernel
	{
		#region Variables

		/// <summary>
		/// The blackboard itself
		/// </summary>
		private Blackboard blackboard;

		/// <summary>
		/// Command dictionary
		/// </summary>
		private Dictionary<string, StringEventHandler> commands;

		/// <summary>
		/// Stores the list of system prototypes
		/// </summary>
		private Dictionary<string, int> systemPrototypes;

		/// <summary>
		/// Regular expression used to split commands
		/// </summary>
		private Regex rxCommandSplitter;

		/// <summary>
		/// Handles console input
		/// </summary>
		private ConsoleManager consoleManager;

		/// <summary>
		/// Stores the list of delegates used while tracing variables
		/// </summary>
		private Dictionary<string, SharedVariableWrittenEventHandler> tracers;

		/// <summary>
		/// A process manager used to start and stop the process of the modules
		/// </summary>
		private ModuleProcessManager procMan;

		#endregion

		#region Consutructor

		public Kernel(Blackboard blk)
		{
			this.blackboard = blk;
			this.procMan = new ModuleProcessManager((Robotics.Utilities.LogWriter)blackboard.Log);
			this.commands = new Dictionary<string, StringEventHandler>();
			this.tracers = new Dictionary<string, SharedVariableWrittenEventHandler>();
			this.rxCommandSplitter = new Regex(@"^(?<cmd>\w+)(\s+(?<par>.*))?$");
			RegisterCommands();
			RegisterSystemPrototypes();
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

			foreach (IModuleClient m in this.blackboard.Modules)
			{
				if (m == this.blackboard.VirtualModule)
					continue;
				// completionTree.AddWord("list " + m.Name + " commands");
				completionTree.AddWord("info " + m.Name);
				completionTree.AddWord("sim " + m.Name);
				completionTree.AddWord("proc check " + m.Name);
				completionTree.AddWord("proc close " + m.Name);
				completionTree.AddWord("proc kill " + m.Name);
				completionTree.AddWord("proc launch " + m.Name);
				completionTree.AddWord("proc restart " + m.Name);
				completionTree.AddWord("proc start " + m.Name);
				// completionTree.AddWord("put " + m.Name);
				foreach (IPrototype proto in m.Prototypes)
				{
					string par = proto.ParamsRequired ? " \"0\"" : String.Empty;
					//completionTree.AddWord(String.Format("put {0} {1}{2}", m.Name, proto.Command, par));
					completionTree.AddWord(String.Format("put {0}{1}", proto.Command, par));
				}
			}

			completionTree.AddWord("list modules");
			completionTree.AddWord("list commands");
			completionTree.AddWord("list vars");

			foreach (SharedVariable sv in this.blackboard.VirtualModule.SharedVariables)
			{
				completionTree.AddWord("cat " + sv.Name);
				completionTree.AddWord("read " + sv.Name);
				completionTree.AddWord("trace " + sv.Name);
			}
		}

		public void RegisterCommands()
		{
			commands.Add("cat", new StringEventHandler(ReadCommand));
			commands.Add("exit", new StringEventHandler(QuitCommand));
			commands.Add("help", new StringEventHandler(HelpCommand));
			commands.Add("info", new StringEventHandler(InfoCommand));
			commands.Add("list", new StringEventHandler(ListCommand));
			commands.Add("log", new StringEventHandler(LogCommand));
			commands.Add("put", new StringEventHandler(PutCommand));
			commands.Add("proc", new StringEventHandler(ProcCommand));
			commands.Add("quit", new StringEventHandler(QuitCommand));
			commands.Add("read", new StringEventHandler(ReadCommand));
			commands.Add("sim", new StringEventHandler(SimCommand));
			commands.Add("trace", new StringEventHandler(TraceCommand));
		}

		private void RegisterSystemPrototypes()
		{
			this.systemPrototypes = new Dictionary<string, int>();
			foreach (IPrototype p in blackboard.VirtualModule.Prototypes)
				systemPrototypes.Add(p.Command, 0);
		}

		private void TraceVariable(SharedVariable sv)
		{
			string s = String.Format("[Tracer] {0}{1} {2} = {3}",
				sv.Type, sv.IsArray ? "[]" : String.Empty,
				sv.Name,
				sv.ReadStringData());
			this.consoleManager.Report(s);
		}

		#endregion

		#region General commands

		public void HelpCommand(string s)
		{
			Help.ShowHelp(s, this.commands.Keys);
		}

		public void InfoCommand(string s)
		{
			ConsoleColor color;

			if (!this.blackboard.Modules.Contains(s))
			{
				Console.WriteLine("Unknown module {0}", s);
				return;
			}
			IModuleClient m = this.blackboard.Modules[s];
			WriteModule(m);
			if (!String.IsNullOrEmpty(m.Alias) && (m.Alias != m.Name))
				Console.WriteLine("Alias:  {0}", m.Alias);
			if (!m.Enabled) return;

			if (m.Simulation.SimulationEnabled)
			{
				color = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Simulation enabled");
				Console.ForegroundColor = color;
			}

			IModuleClientTcp tcpModule = m as IModuleClientTcp;
			if (tcpModule != null)
			{
				color = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine();
				Console.WriteLine("TCP information:");
				Console.ForegroundColor = color;
				StringBuilder sb = new StringBuilder(100);
				foreach (System.Net.IPAddress a in tcpModule.ServerAddresses)
				{
					sb.Append(a.ToString());
					sb.Append(", ");
				}
				if (sb.Length > 2)
					sb.Length -= 2;
				Console.WriteLine("TCP Port:  {0}", tcpModule.Port);
				Console.WriteLine("Addresses: {0}", sb.ToString());
			}

			if (m.ProcessInfo != null)
			{
				color = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine();
				Console.WriteLine("Process information:");
				Console.ForegroundColor = color;
				Console.WriteLine("Process Name: {0}", m.ProcessInfo.ProcessName);
				Console.WriteLine("Program Path: {0}", m.ProcessInfo.ProgramPath);
				Console.WriteLine("Program Args: {0}", m.ProcessInfo.ProgramArgs);
			}

			color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			Console.WriteLine("Prototypes (supported commands):");
			Console.ForegroundColor = color;
			WriteModulePrototypes(m);
			Console.WriteLine();

		}

		public void ListCommand(string s)
		{
			if (String.IsNullOrEmpty(s))
			{
				WriteCommandOptions("list", "modules", "vars", "commands", "<module name> commands");
				return;
			}

			switch (s.ToLower())
			{
				case "modules":
					ListModules();
					return;

				case "variables":
				case "vars":
					ListVariables();
					return;

				case "commands":
					ListPrototypes();
					return;
			}
		}

		public void LogCommand(string s)
		{
			if (this.consoleManager == null)
				return;

			if (String.IsNullOrEmpty(s))
			{
				this.consoleManager.EnableLog(3);
				return;
			}

			int verbosity;
			if (!Int32.TryParse(s, out verbosity) || (verbosity < 0) || (verbosity > 9))
			{
				Console.WriteLine("Invalid verbosity level. Values must be between 1 and 9");
				return;
			}
			if (verbosity == 0)
				this.consoleManager.DisableLog();
			else
				this.consoleManager.EnableLog(verbosity);
			//SharedVariable sv = blackboard.VirtualModule.SharedVariables[""];
			//sv.ToString();
		}

		public void ProcCommand(string s)
		{
			Match m;
			IModuleClientTcp module;

			if (String.IsNullOrEmpty(s) || !(m = rxCommandSplitter.Match(s)).Success)
			{
				WriteCommandOptions("proc", "check <module name>", "close <module name>", "kill", "launch <module name>", "restart <module name>", "start <module name>");
				return;
			}

			string action = m.Result("${cmd}");
			string moduleName = m.Result("${par}");
			if (!this.blackboard.Modules.Contains(moduleName))
			{
				Console.WriteLine("Unknown module {0}", s);
				return;
			}
			module = this.blackboard.Modules[moduleName] as IModuleClientTcp;
			if (module == null)
			{
				Console.WriteLine("Module {0} does not support this operation", module.Name);
				return;
			}

			switch (action.ToLower())
			{
				case "check":
					ProcCheck(module);
					return;

				case "close":
					ProcClose(module);
					return;

				case "kill":
					ProcKill(module);
					return;

				case "launch":
					ProcLaunch(module);
					return;

				case "restart":
					ProcRestart(module);
					return;

				case "start":
					ProcStart(module);
					return;
			}
		}

		public void PutCommand(string s)
		{
			bool result;
			string src = String.Empty;
			int spacePos = s.IndexOf(' ');
			if (spacePos > 0)
			{
				src = s.Substring(0, spacePos);
				if (!blackboard.Modules.Contains(src))
					src = blackboard.VirtualModule.Name + " ";
				else
					src = String.Empty;
			}
			result = blackboard.Inject(src + s);
			Console.WriteLine("Injection {0}", result ? "succeeded" : "failed");
		}

		public void QuitCommand(string s)
		{
			this.blackboard.Stop();
		}

		public void ReadCommand(string s)
		{
			if (!this.blackboard.VirtualModule.SharedVariables.Contains(s))
			{
				Console.WriteLine("Unknown shared variable {0}", s);
				return;
			}
			SharedVariable sv = this.blackboard.VirtualModule.SharedVariables[s];
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("{0}{1} {2} = ", sv.Type, sv.IsArray ? "[]" : String.Empty, sv.Name);
			Console.ForegroundColor = color;
			Console.WriteLine(sv.ReadStringData());
		}

		public void SimCommand(string s)
		{
			if (!this.blackboard.Modules.Contains(s))
			{
				Console.WriteLine("Unknown module {0}", s);
				return;
			}
			IModuleClient m = this.blackboard.Modules[s];
			m.Stop();
			m.Simulation.SuccessRatio = m.Simulation.SimulationEnabled ? 2 : 1;
			m.Start();
			WriteModule(m);
			if (m.Simulation.SimulationEnabled)
			{
				ConsoleColor color = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Simulation enabled");
				Console.ForegroundColor = color;
			}
		}

		public void TraceCommand(string s)
		{
			if (!this.blackboard.VirtualModule.SharedVariables.Contains(s))
			{
				Console.WriteLine("Unknown shared variable {0}", s);
				return;
			}
			SharedVariable sv = this.blackboard.VirtualModule.SharedVariables[s];
			ConsoleColor color = Console.ForegroundColor;
			if (!tracers.ContainsKey(sv.Name) || (tracers[sv.Name] == null))
			{
				tracers[sv.Name] = new SharedVariableWrittenEventHandler(TraceVariable);
				sv.Written += tracers[sv.Name];
				Console.WriteLine("[Tracer] {0}{1} {2} tracer enabled.", sv.Type, sv.IsArray ? "[]" : String.Empty, sv.Name);
			}
			else
			{
				sv.Written -= tracers[sv.Name];
				tracers[sv.Name] = null;
				Console.WriteLine("[Tracer] {0}{1} {2} tracer disabled.", sv.Type, sv.IsArray ? "[]" : String.Empty, sv.Name);
			}
		}

		#endregion

		#region List Commands

		public void ListModules()
		{
			foreach (IModuleClient m in this.blackboard.Modules)
				WriteModule(m);

			Console.WriteLine();
		}

		private void ListPrototypes()
		{
			foreach (IModuleClient m in this.blackboard.Modules)
			{
				if (!m.Enabled) continue;
				if (m == blackboard.VirtualModule)
					continue;
				WriteModule(m);
				WriteModulePrototypes(m);
				Console.WriteLine();
			}
		}

		public void ListVariables()
		{
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			foreach (SharedVariable sv in this.blackboard.VirtualModule.SharedVariables)
			{
				Console.Write("{0}{1}", sv.Type, sv.IsArray ? "[]" : String.Empty);
				Console.CursorLeft = 15;
				Console.Write(sv.Name);
				Console.CursorLeft = 35;
				Console.WriteLine("{0} subscribers.", sv.Subscriptions.Count);
			}
			Console.ForegroundColor = color;
			Console.WriteLine();
		}

		#endregion

		#region Proc Commands

		private void ProcCheck(IModuleClientTcp module)
		{
			IModuleProcessInfo pi = module.ProcessInfo;
			if ((pi == null) || (String.IsNullOrEmpty(pi.ProcessName)))
			{
				WriteError("No process information found for module {0}", module);
				return;
			}

			Console.Write("Checking running instances for module {0}... ", module.Name);
			try
			{
				int count = this.procMan.CheckModule(module);
				if (count >= 0)
					Console.WriteLine("Done! There are {0} instances of {1} being executed.", count, module.Name);
				else
					Console.WriteLine("Failed!");
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				WriteError("Failed: " + ex.Message);
			}
		}

		private void ProcClose(IModuleClientTcp module)
		{
			IModuleProcessInfo pi = module.ProcessInfo;
			if ((pi == null) || (String.IsNullOrEmpty(pi.ProcessName)))
			{
				WriteError("No process information found for module {0}", module);
				return;
			}

			Console.Write("Attempting to close module {0}... ", module.Name);
			try
			{
				this.procMan.ShutdownModule(module, ModuleShutdownMethod.CloseThenKill);
				Console.WriteLine("Done!");
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				WriteError("Failed: " + ex.Message);
			}
		}

		private void ProcKill(IModuleClientTcp module)
		{
			IModuleProcessInfo pi = module.ProcessInfo;
			if ((pi == null) || (String.IsNullOrEmpty(pi.ProcessName)))
			{
				WriteError("No process information found for module {0}", module);
				return;
			}

			Console.Write("Attempting to kill module {0}... ", module.Name);
			try
			{
				this.procMan.ShutdownModule(module, ModuleShutdownMethod.KillOnly);
				Console.WriteLine("Done!");
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				WriteError("Failed: " + ex.Message);
			}
		}

		private void ProcLaunch(IModuleClientTcp module)
		{
			IModuleProcessInfo pi = module.ProcessInfo;
			if ((pi == null) || String.IsNullOrEmpty(pi.ProcessName) || String.IsNullOrEmpty(pi.ProgramPath))
			{
				WriteError("No process information found for module {0}", module);
				return;
			}

			Console.Write("Attempting to launch module {0}... ", module.Name);
			try
			{
				this.procMan.LaunchModule(module, ModuleStartupMethod.LaunchIfNotRunning);
				Console.WriteLine("Done!");
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				WriteError("Failed: " + ex.Message);
			}
		}

		private void ProcRestart(IModuleClientTcp module)
		{
			IModuleProcessInfo pi = module.ProcessInfo;
			if ((pi == null) || String.IsNullOrEmpty(pi.ProcessName) || String.IsNullOrEmpty(pi.ProgramPath))
			{
				WriteError("No process information found for module {0}", module);
				return;
			}

			Console.Write("Attempting to restart module {0}... ", module.Name);
			try
			{
				this.procMan.LaunchModule(module, ModuleStartupMethod.KillThenLaunch);
				Console.WriteLine("Done!");
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				WriteError("Failed: " + ex.Message);
			}
		}

		private void ProcStart(IModuleClientTcp module)
		{
			IModuleProcessInfo pi = module.ProcessInfo;
			if ((pi == null) || String.IsNullOrEmpty(pi.ProgramPath))
			{
				WriteError("No process information found for module {0}", module);
				return;
			}

			Console.Write("Attempting to start module {0}... ", module.Name);
			try
			{
				this.procMan.LaunchModule(module, ModuleStartupMethod.LaunchAlways);
				Console.WriteLine("Done!");
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				WriteError("Failed: " + ex.Message);
			}
		}

		#endregion

		#region Write

		private void WriteCommandOptions(string command, params string[] options)
		{
			ConsoleColor color = Console.ForegroundColor;
			foreach (string opt in options)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\tlist ");
				Console.ForegroundColor = color;
				Console.WriteLine(opt);
			}
		}

		private void WriteModule(IModuleClient m)
		{
			string status;
			ConsoleColor color = Console.ForegroundColor;
			if (!m.Enabled)
			{
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine("{0} Disabled", m.Name.PadRight(20, ' '));
				Console.ForegroundColor = color;
				return;
			}
			else if (m.Simulation.SimulationEnabled)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("{0} OK (Simulated)", m.Name.PadRight(20, ' '));
				Console.ForegroundColor = color;
				return;
			}
			else if (!m.IsConnected)
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine("{0} Disconnected", m.Name.PadRight(20, ' '));
				Console.ForegroundColor = color;
				return;
			}
			else if (!m.IsAlive || !m.Ready)
			{
				status = String.Empty;
				if (!m.Ready) status = "Not ready";
				if (!m.IsAlive) status = (String.IsNullOrEmpty(status) ? "Not responding" : status + ", not responding");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("{0} {1}", m.Name.PadRight(20, ' '), status);
				Console.ForegroundColor = color;
				return;
			}

			status = "OK";
			if (m.Busy) status = "Busy";
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("{0} {1}", m.Name.PadRight(20, ' '), status);
			Console.ForegroundColor = color;
		}

		private void WriteModulePrototypes(IModuleClient m)
		{
			foreach (IPrototype p in m.Prototypes)
			{
				if (systemPrototypes.ContainsKey(p.Command))
					continue;
				Console.Write("\t");
				WritePrototype(p);
			}
		}

		private void WritePrototype(IPrototype p)
		{
			Console.Write(p.Command.PadRight(20, ' '));
			Console.Write("{0} ", p.HasPriority ? "H" : " ");
			Console.Write("{0} ", p.ResponseRequired ? "R" : " ");
			Console.Write("{0} ", p.ParamsRequired ? "P" : " ");
			Console.WriteLine("\ttimeout: {0}ms", p.Timeout.ToString().PadLeft(6, ' '));
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

		#endregion
	}
}