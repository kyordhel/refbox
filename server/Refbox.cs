using System;
using System.Diagnostics;
using System.Threading;

namespace RefBox
{
	/// <summary>
	/// Implements the Referee Box
	/// </summary>
	public class Refbox
	{
		#region Variables

		/// <summary>
		/// The Udp connector for sending and receiving messages.
		/// </summary>
		private Connector cnn;

		/// <summary>
		/// Real time clock used to measure time (system clock independent)
		/// </summary>
		private Stopwatch sw;

		/// <summary>
		/// Timer used to send the remaining time every 60 seconds
		/// </summary>
		private Timer timer;

		/// <summary>
		/// Stores the test time
		/// </summary>
		private int testTime;

		/// <summary>
		/// The test log.
		/// </summary>
		private Log testLog;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="RefBox.Refbox"/> class.
		/// </summary>
		public Refbox ()
		{
			this.cnn = new Connector ();
			this.cnn.MessageReceived+=new MessageReceivedEventHandler(connector_MessageReceived);
			this.timer = new Timer(new TimerCallback(TimerTick));
			this.sw = new Stopwatch ();
			this.testTime = 600;
			this.TestName = null;
			this.TeamName = "Unknown";
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the test's elapsed time in seconds.
		/// </summary>
		public int ElapsedTime{
			get{ return (int)(sw.ElapsedMilliseconds / 1000);}
		}

		/// <summary>
		/// Gets the test's remaining time in seconds.
		/// </summary>
		public int RemainingTime{
			get{ return Math.Max(testTime - (int)(sw.ElapsedMilliseconds / 1000), 0);}
		}

		/// <summary>
		/// Stores the name of the current team
		/// </summary>
		public string TeamName{ get; private set; }

		/// <summary>
		/// Stores the name of the current test
		/// </summary>
		public string TestName{ get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Broadcast the specified signal.
		/// </summary>
		/// <param name="signal">The signal to send.</param>
		public void Broadcast(Signal signal){
			this.cnn.Broadcast (signal);
			if (testLog != null)
				testLog.Write (ElapsedTime, signal);
		}

		void ChangeTeam (string teamName)
		{
			if (this.TestName == "Unknown")
				throw new OperationCanceledException ("PrepareTest method must be called first");
			if(String.IsNullOrEmpty(teamName)) throw new ArgumentNullException("teamName");
			this.TeamName = teamName;
			this.testLog = new Log (TestName, testTime, teamName);
		}

		/// <summary>
		/// Called by cnn (the UDP Connector) when a message is received.
		/// </summary>
		/// <param name="connector">The Connector object that rises the event</param>
		/// <param name="message">The message received</param>
		private void connector_MessageReceived (Connector connector, System.Net.IPEndPoint source, string message)
		{
			Event e = Serializer.DeserializeEvent (message);
			if (e == null)
				return;
			e.Source = source;
			if (testLog != null)
				testLog.Write (ElapsedTime, e);
		}

		/// <summary>
		/// Supplies the Referee Box with information for the next test
		/// </summary>
		/// <param name="teamName">Team's name.</param>
		/// <param name="testName">Test's name.</param>
		/// <param name="testTime">Specifies the total test time in seconds.</param>
		public void PrepareTest(string testName, int testTime){
			if(String.IsNullOrEmpty(testName)) throw new ArgumentNullException("testName");
			if((testTime < 30) || (testTime > 86400))
				throw new ArgumentOutOfRangeException("testTime", "testTime must be between 30 seconds and 1 day (86400 secs)");
			this.testTime = testTime;
			this.TestName = testName;
			this.cnn.Start();
		}

		/// <summary>
		/// Supplies the Referee Box with information for the next test
		/// </summary>
		/// <param name="teamName">Team's name.</param>
		/// <param name="testName">Test's name.</param>
		/// <param name="testTime">Specifies the total test time.</param>
		public void PrepareTest(string testName, TimeSpan testTime){
			this.PrepareTest (testName, (int)testTime.TotalSeconds);
		}

		/// <summary>
		/// Supplies the Referee Box with information for the next test
		/// </summary>
		/// <param name="teamName">Team's name.</param>
		/// <param name="testName">Test's name.</param>
		/// <param name="testTime">Specifies the total test time in seconds.</param>
		public void PrepareTest(string testName, int testTime, string teamName){
			this.PrepareTest (testName, testTime);
			this.ChangeTeam(teamName);
		}

		/// <summary>
		/// Supplies the Referee Box with information for the next test
		/// </summary>
		/// <param name="teamName">Team's name.</param>
		/// <param name="testName">Test's name.</param>
		/// <param name="testTime">Specifies the total test time.</param>
		public void PrepareTest(string testName, TimeSpan testTime, string teamName){
			this.PrepareTest (testName, (int)testTime.TotalSeconds, teamName);
		}

		/// <summary>
		/// Broadcasts a START and TIME signals without and logs the time of restart.
		/// Test countdown timer is not afected
		/// </summary>
		public void RestartTest(){
			this.Broadcast (Signal.Start);
		}

		/// <summary>
		/// Broadcasts a CONTINUE signal to the robot (See CONTINUE rule in the Rulebook)
		/// </summary>
		/// <param name="text">The text to be sent to the robot</param>
		public void SendContinueText(string text){
			this.Broadcast(Signal.CreateContinue(text));
		}

		/// <summary>
		/// Starts the message log, the test countdown, and broadcasts the START and first TIME signals
		/// </summary>
		public void StartTest(){
			this.sw.Reset ();
			this.Broadcast (Signal.Start);
			this.sw.Start ();
			this.timer.Change (0, 60000);
		}

		/// <summary>
		/// Stops the  message log, the test countdown, and broadcasts the STOP signal
		/// </summary>
		public void StopTest(){
			this.timer.Change (-1, -1);
			this.cnn.Broadcast (Signal.Stop);
			this.sw.Stop ();
		}

		/// <summary>
		/// Called by the timer every time the time elapses. It broadcasts the remaining time
		/// </summary>
		/// <param name="o">unused</param>
		private void TimerTick(object o){
			this.Broadcast (Signal.CreateTime (this.RemainingTime));
			if (this.RemainingTime <= 0) {
				this.timer.Change (-1, -1);
				StopTest ();
			}
			else if (this.RemainingTime <= 10)
				this.timer.Change (1000, 1000);
			else if (this.RemainingTime <= 30)
				this.timer.Change (10000, 10000);
			else if (this.RemainingTime <= 60)
				this.timer.Change (30000, 10000);
			else if((RemainingTime % 60) != 0)
				this.timer.Change (1000 * (RemainingTime % 60), 60000);
		}

		#endregion
	}
}

