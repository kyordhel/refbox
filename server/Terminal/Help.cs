using System;
using System.Collections.Generic;
using System.Text;

namespace RefBox.Terminal
{
	public class Help
	{
		public static void ShowHelp(string command, IEnumerable<string> commands)
		{
			switch (command)
			{
				case "continue": Continue(); break;
				case "exit": Quit(); break;
				case "help": HelpOnHelp(); break;
				case "info": Info(); break;
				case "list": List(); break;
				case "quit": Quit(); break;
				case "restart": Restart(); break;
				case "start": Start(); break;
				case "stop": Stop(); break;
				case "test": Time(); break;
				case "team": Time(); break;
				case "time": Time(); break;

				default:
					MainHelp(commands);
					break;
			}
			Console.WriteLine();
		}

		private static void Continue()
		{
			WriteCommandHelp("continue <input text>", "Sends the input text as a CONTINUE signal (CONTINUE RULE)");
		}

		private static void HelpOnHelp()
		{
			WriteCommandHelp("help", "Displays the list of available commands");
			WriteCommandHelp("help <command>", "Displays help for the specifiec command");
		}

		private static void Info()
		{
			WriteCommandHelp("info", "Displays the current test, team and time");
		}

		private static void List()
		{
			WriteCommandHelp("list teams", "Displays the list of all teams");
			WriteCommandHelp("list tests", "Displays the list of all tests and their times");
		}
		
		private static void Quit()
		{
			WriteCommandHelp("quit", "Shuts down the refbox and exits");
		}

		private static void Restart()
		{
			WriteCommandHelp("start", "Sends the START signal without restarting the countdown");
		}

		private static void Start()
		{
			WriteCommandHelp("start", "Sends the START signal and starts the countdown");
		}

		private static void Stop()
		{
			WriteCommandHelp("stop", "Sends the STOP signal and stops the countdown");
		}

		private static void Team()
		{
			WriteCommandHelp("team <team name>", "Sets the current team");
		}

		private static void Test()
		{
			WriteCommandHelp("test <test name>", "Sets the current test");
		}

		private static void Time()
		{
			WriteCommandHelp("time", "Displays the current test, team and time");
		}

		private static void WriteCommandHelp(string command, string text)
		{
			WriteCommandHelp(command, 25, text);
		}

		private static void WriteCommandHelp(string command, int commandSize, string text)
		{
			//string[] parts;
			//int textMaxSize = Console.BufferWidth - commandSize;
			//string padding = String.Empty.PadRight(commandSize, ' ');
			//command = command.PadRight(commandSize, ' ');
			//if (text.Length < textMaxSize)
			//    parts = new String[] { text };
			//else{
			//    parts = new String[1 + text.Length / textMaxSize];
			//    for (int i = 0; i < parts.Length; ++i)
			//        parts[i] = text.Substring(i * textMaxSize, Math.Min(textMaxSize, text.Length - i * textMaxSize));
			//}
			//Console.Write("{0}{1}", command, parts[0]);
			//for(int i = 1; i < parts.Length; ++i)
			//    Console.Write("{0}{1}", padding, parts[i]);
			//Console.WriteLine();
			string[] parts = text.Split(' ');
			string padding = String.Empty.PadRight(commandSize, ' ');
			command = command.PadRight(commandSize, ' ');
			Console.Write("{0}{1} ", command, parts[0]);
			for (int i = 1; i < parts.Length; ++i)
			{
				if ((Console.CursorLeft + parts[i].Length) >= Console.BufferWidth)
				{
					Console.WriteLine();
					Console.Write(padding);
				}
				Console.Write("{0} ", parts[i]);
			}
			Console.WriteLine();

		}

		private static void MainHelp(IEnumerable<string> commands)
		{

			List<string> cmdList = new List<string>(commands);
			cmdList.Sort();
			Console.WriteLine("Type help <command> for detailed help about a specific command.");
			Console.WriteLine("List of supported commands:");
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			foreach (string cmd in cmdList)
				Console.WriteLine("  {0}", cmd);
			Console.ForegroundColor = color;
		}
	}
}