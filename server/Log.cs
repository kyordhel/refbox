using System;
using System.IO;
using System.Net;
using System.Text;

namespace RefBox
{
	public class Log
	{
		public const string LogPath = "logs";
		private TextWriter writer;

		private Log(string fileName){
			this.FileName = fileName;
			if (File.Exists (fileName)) {
			string old = fileName;
				do {
					old = old + ".old";
				} while(File.Exists(old));
				File.Move (fileName, old);
			}
			this.writer = StreamWriter.Synchronized (new StreamWriter (fileName){AutoFlush = true});
		}

		public Log (string testName, int testTime, string teamName)
			: this(Path.Combine(LogPath, String.Format("{0}_{1}_{2}.log",
			                                String.IsNullOrEmpty (testName) ? "test" : StringToValidFilename (testName),
			                                String.IsNullOrEmpty (teamName) ? "team" : StringToValidFilename (teamName),
											DateTime.Now.ToString("yyyy-MM-dd_hhmmss")
		))){

			Log.sessionLog.WriteLine ();
			Log.sessionLog.Write ('\f');
			string now = Now;
			Log.sessionLog.WriteLine ("{0} Created log for Team {1} on performing {2}", now, teamName, testName);
			this.WriteHeader (
				String.Format("Team Name: {0}", teamName),
				String.Format("Test Name: {0}", testName),
				String.Format("Test Time: {0}", testTime),
				String.Format("Log started on {0}", now)
			);
		}

		~Log(){
			try {
				if (writer == null)
					return;
				writer.Flush ();
				writer.Close ();
				writer = null;
			} catch {}
		}

		public string FileName{ get; private set;}

		private void Write(char c){
			this.writer.Write (c);
		}

		private void Write(string s){
			this.writer.Write (s);
		}

		private void Write(string format, params object[] args){
			this.writer.Write (format, args);
		}

		public void Write(int elapsed, Signal signal){

			String record = String.Format ("{0} Sent SIGNAL {1} = {2}", ElapsedToString(elapsed), signal.Type, signal.Value);
			this.WriteLine (record);
			if (this != Log.sessionLog) {
				Log.sessionLog.WriteLine ("{0} Time {1}", Now, record);
			}
		}

		public void Write(int elapsed, Event e){
			string source = e.Source == null ?
				String.Format ("{0} Src {1}", String.Empty.PadLeft(5), e.Source) :
				String.Empty;
			string sEvent = String.Format ("{0} Rcv  EVENT  {1} = {2}{4}{5} Src {3}", Log.Now, e.Type, e.Value, source);
			this.WriteLine ("{0}{1}{2}", sEvent, Environment.NewLine, source);
			if (this != Log.sessionLog) {
				string sNow = Now;
				Log.sessionLog.WriteLine ("{0} Time {1}{2}{3}{4}", sNow, sEvent, Environment.NewLine, String.Empty.PadLeft(sNow.Length + 6), source);
			}
		}

		private void WriteHeader(params string[] lines){
			this.writer.WriteLine(String.Empty.PadRight(78, '#'));
			this.writer.WriteLine("# {0} #", String.Empty.PadRight(74, ' '));
			foreach (string line in lines)
				this.writer.WriteLine("# {0} #", line.PadRight(74, ' '));
			this.writer.WriteLine("# {0} #", String.Empty.PadRight(74, ' '));
			this.writer.WriteLine(String.Empty.PadRight(78, '#'));
			this.writer.WriteLine ();
		}

		private void WriteLine(){
			this.writer.WriteLine ();
		}

		private void WriteLine(string s){
			this.writer.WriteLine (s);
		}

		private void WriteLine(string format, params object[] args){
			this.writer.WriteLine (format, args);
		}

		private static Log sessionLog;

		static Log(){
			if(!Directory.Exists(LogPath))
				Directory.CreateDirectory("logs");
			Log.sessionLog = new Log(Path.Combine(LogPath, String.Format("refbox_{0}.log", DateTime.Now.ToString("yyyy-MM-dd_hhmmss"))));
			Log.sessionLog.WriteHeader(String.Format("RefBox started on {0}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
		}

		private static string Now{get { return DateTime.Now.ToString ("yyyy-MM-dd hh:mm:ss"); } }

		private static string StringToValidFilename(string s){
			if (String.IsNullOrEmpty (s))
				return "a";
			StringBuilder sb = new StringBuilder ();
			for (int i = 0; i < s.Length; ++i) {
				char c = s [i];
				if ( ((c >= '0') && (c <= '9')) || ((c >= 'a') && (c <= 'z')) || (c == '_') || (c == '.') )
					sb.Append (c);
				else if ((c >= 'A') && (c <= 'Z'))
					sb.Append ((char)(c + 'a' - 'A'));
				else if (c == '@')
					sb.Append ("_at_");
				else if((c == 32) || (c == '\t'))
					sb.Append ('_');
			}
			return sb.ToString ();
		}

		/// <summary>
		/// Gets a string representation (minutes:seconds) of the provided elapsed time
		/// </summary>
		/// <param name="elapsed">Elapsed time in seconds</param>
		/// <returns>String representation (minutes:seconds) of the provided elapsed time</returns>
		private static string ElapsedToString(int elapsed){
			int min = elapsed / 60;
			int sec = elapsed % 60;
			string smin = min.ToString ().PadLeft (2, '0');
			string ssec = sec.ToString ().PadLeft (2, '0');
			return smin + ":" + ssec;
		}
	}
}

