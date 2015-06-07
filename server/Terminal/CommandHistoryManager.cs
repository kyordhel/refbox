using System;
using System.Collections.Generic;
using System.Text;

namespace RefBox.Terminal
{
	public class CommandHistoryManager
	{
		/// <summary>
		/// Stores the command history
		/// </summary>
		private List<string> history;
		/// <summary>
		/// Stores the command history cursor
		/// </summary>
		private int cursor;

		public CommandHistoryManager() {
			this.history = new List<string>(100);
			this.cursor = -1;
		}

		public void Add(string commandLine)
		{
			if (String.IsNullOrEmpty(commandLine) || String.IsNullOrEmpty(commandLine.Trim()) )
				return;
			if ((this.history.Count > 0) && (String.Compare(commandLine, this.history[this.history.Count - 1], true) == 0))
				return;
			this.history.Add(commandLine);
			this.cursor = this.history.Count;
		}

		public string Next()
		{
			if (this.cursor < (this.history.Count - 1))
			{
				++cursor;
				return this.history[cursor];
			}
			return String.Empty;
		}

		public string Prev()
		{
			if (this.cursor > 0)
			{
				--cursor;
				return this.history[cursor];
			}
			return String.Empty;
		}

		public string Current()
		{
			if ((this.cursor < 0) || (this.cursor >= this.history.Count))
				return String.Empty;
			return this.history[cursor];
		}

		public void Reset()
		{
			this.cursor = this.history.Count - 1;
		}
	}
}
