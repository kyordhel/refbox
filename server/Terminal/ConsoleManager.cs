using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RefBox.Terminal
{
	public class ConsoleManager
	{
		#region Variables

		/// <summary>
		/// Temporary input buffer
		/// </summary>
		private string currentLine;

		/// <summary>
		/// Cursor for the temporary input buffer
		/// </summary>
		private int currentPos;

		/// <summary>
		/// The RefBox itself
		/// </summary>
		private Refbox refbox;

		/// <summary>
		/// Stores the command list for auto completion
		/// </summary>
		private CompletionTree completionTree;

		/// <summary>
		/// Stores the list of recent typed commands
		/// </summary>
		private CommandHistoryManager history;

		/// <summary>
		/// Stopwatch used for double tab detection
		/// </summary>
		private Stopwatch tabSw;

		/// <summary>
		/// Flag used for double tab detection
		/// </summary>
		private bool firstTab;

		/// <summary>
		/// Object that contains all the console commands
		/// </summary>
		private Kernel kernel;

		/// <summary>
		/// Object used to synchronize the access to the console
		/// </summary>
		private static object consoleLock;

		#endregion

		#region Constructor

		static ConsoleManager() {
			ConsoleManager.consoleLock = new Object();
		}

		public ConsoleManager(Refbox refbox, Kernel kernel)
		{
			this.kernel = kernel;
			this.refbox = refbox;
			this.currentLine = String.Empty;
			this.tabSw = new Stopwatch();
			this.firstTab = false;
			this.history = new CommandHistoryManager();
			this.completionTree = new CompletionTree();
			this.kernel.ConsoleManager = this;
			this.kernel.FillCompletionTree(this.completionTree);
		}

		#endregion

		#region Properties

		public string Prompt { get { return "RefBox> "; } }

		/// <summary>
		/// Gets an object used to synchronize the access to the console
		/// </summary>
		public static object ConsoleLock { get { return ConsoleManager.consoleLock; } }

		#endregion

		#region Methods

		private void CompleteCommand(string prefix)
		{
			string sufix = completionTree.CompleteWord(prefix);
			Insert(sufix);
		}

		private void ExecuteCommand(string command)
		{
			this.history.Add(command);
			if (kernel.Execute(command)) return;
			string sufix = completionTree.CompleteWord(command);
			if (!String.IsNullOrEmpty(sufix))
			{
				lock (ConsoleManager.ConsoleLock)
				{
					Console.WriteLine("\tUnknown command. Did you meant: {0}{1}?", command, sufix);
				}
			}
		}

		private void FixCursor()
		{
			lock (ConsoleManager.ConsoleLock)
			{
				Console.CursorLeft = Prompt.Length + currentPos;
			}
		}

		private void HandleChar(char c)
		{
			tabSw.Stop();
			switch (c)
			{
				case '\0': break;

				case '\t':
					if ( (firstTab && (tabSw.ElapsedMilliseconds > 500)) || !firstTab)
					{
						// Here is the first time the tab has been pressed
						firstTab = true;
						tabSw.Reset();
						tabSw.Start();
						CompleteCommand(currentLine);
					}
					else
					{
						// Here is the second time the tab has been pressed
						firstTab = false;
						tabSw.Reset();
						WriteAlternatives(currentLine);
					}
					return;

				//case '\b':
				//    BackSpace();
				//    break;

				case '\n':
				case '\r':
					NewLineChar(c);
					break;

				default:
					//if((c > 31) && (c < 128)) Insert(c);
					if (c > 31) Insert(c);
					break;
			}
			firstTab = false;
		}

		public void HandleKey(ConsoleKey key)
		{
			switch (key)
			{
				case ConsoleKey.Delete:
					Delete();
					return;

				case ConsoleKey.Backspace:
					BackSpace();
					return;

				case ConsoleKey.LeftArrow:
					ArrowLeft();
					return;

				case ConsoleKey.RightArrow:
					ArrowRight();
					return;

				case ConsoleKey.UpArrow:
					ArrowUp();
					return;

				case ConsoleKey.DownArrow:
					ArrowDown();
					return;

				case ConsoleKey.End:
					currentPos = currentLine.Length;
					FixCursor();
					return;

				case ConsoleKey.Home:
					currentPos = 0;
					FixCursor();
					return;

				case ConsoleKey.F1:
					Program.RunGUI(refbox);
					break;
			}
		}

		private void Insert(char c)
		{
			lock (ConsoleManager.ConsoleLock)
			{
				if (currentPos < currentLine.Length)
				{
					currentLine = currentLine.Insert(currentPos, c.ToString());
					Console.Write(currentLine.Substring(currentPos));
				}
				else
				{
					currentLine += c;
					Console.Write(c);
				}
				++currentPos;
				FixCursor();
			}
		}

		private void Insert(string s)
		{
			if (currentPos < currentLine.Length)
			{
				currentLine.Insert(currentPos, s);
			}
			else
			{
				currentLine += s;
				lock (ConsoleManager.ConsoleLock)
				{
					Console.Write(s);
				}
			}
			currentPos += s.Length;
			FixCursor();
		}

		public void Poll()
		{
			if (!Console.KeyAvailable)
			{
				System.Threading.Thread.Sleep(10);
				return;
			}
			ConsoleKeyInfo cki = Console.ReadKey(true);
			HandleKey(cki.Key);
			HandleChar(cki.KeyChar);			
		}

		public void Report(string s)
		{
			lock (ConsoleManager.ConsoleLock)
			{
				Console.WriteLine("\r{0}", s);
				WritePrompt(true);
				Console.Write(this.currentLine);
			}
		}

		private void WriteAlternatives(string prefix)
		{
			string[] alternatives = this.completionTree.GetAlternatives(prefix);
			if (alternatives == null)
				return;

			lock (ConsoleManager.ConsoleLock)
			{
				ConsoleColor color = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine();
				foreach (string alternative in alternatives)
					Console.WriteLine(alternative);
				Console.ForegroundColor = color;
				Console.WriteLine();
				WritePrompt();
				Console.Write(currentLine);
			}
		}

		public void WritePrompt()
		{
			lock (ConsoleManager.ConsoleLock)
			{
				Console.Write(Prompt);
			}
		}

		public void WritePrompt(bool clearLine)
		{
			lock (ConsoleManager.ConsoleLock)
			{
				if (clearLine)
				{
					Console.Write("\r{0}", Prompt.PadRight(Console.BufferWidth - 1, ' '));
					Console.CursorLeft = Prompt.Length;
				}
				else Console.Write(Prompt);
			}
		}

		#region Key handling Methods

		private void ArrowDown()
		{
			string next = history.Next();
			currentLine = next;
			currentPos = currentLine.Length;
			lock (ConsoleManager.ConsoleLock)
			{
				WritePrompt(true);
				Console.Write(next);
			}
		}

		private void ArrowLeft()
		{
			if (currentPos > 0)
				--currentPos;
			lock (ConsoleManager.ConsoleLock)
			{
				if (Console.CursorLeft > 5)
					--Console.CursorLeft;
			}
		}

		private void ArrowRight()
		{
			if (currentPos >= currentLine.Length)
				return;
			++currentPos;
			lock (ConsoleManager.ConsoleLock)
			{
				++Console.CursorLeft;
			}
		}

		private void ArrowUp()
		{
			string prev = history.Prev();
			if (!String.IsNullOrEmpty(prev))
			{
				currentLine = prev;
				currentPos = currentLine.Length;
				lock (ConsoleManager.ConsoleLock)
				{
					WritePrompt(true);
					Console.Write(prev);
				}
			}
		}

		private void BackSpace()
		{
			if (currentPos < 1)
				return;

			lock (ConsoleManager.ConsoleLock)
			{
				--Console.CursorLeft;
				--currentPos;
				currentLine = currentLine.Remove(currentPos, 1);
				int cPos = Console.CursorLeft;
				string tail = currentLine.Substring(currentPos);
				Console.Write("{0} ", tail);
				Console.CursorLeft = cPos;
			}
		}

		private void NewLineChar(char c)
		{
			if ((c == '\n') && (Environment.NewLine.Length > 1))
				return;
			lock (ConsoleManager.ConsoleLock)
			{
				Console.WriteLine();
				ExecuteCommand(currentLine);
				currentLine = String.Empty;
				currentPos = 0;
					WritePrompt();
			}
		}

		private void Delete()
		{
			if (currentPos >= currentLine.Length)
				return;

			lock (ConsoleManager.ConsoleLock)
			{
				int cPos = Console.CursorLeft;
				if (currentPos == currentLine.Length)
				{
					Console.Write(' ');
					currentLine = currentLine.Remove(currentPos);
				}
				else
				{
					string tail = currentLine.Substring(currentPos + 1);
					currentLine = currentLine.Remove(currentPos) + tail;
					Console.Write("{0} ", tail);
				}

				Console.CursorLeft = cPos;
			}
		}

		#endregion

		#endregion
	}
}
