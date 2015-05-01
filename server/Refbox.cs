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
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the test's elapsed time.
		/// </summary>
		public int ElapsedTime{
			get{ return (int)(sw.ElapsedMilliseconds / 1000);}
		}

		/// <summary>
		/// Gets the test's remaining time.
		/// </summary>
		public int RemainingTime{
			get{ return Math.Max(testTime - (int)(sw.ElapsedMilliseconds / 1000), 0);}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Called by cnn (the UDP Connector) when a message is received.
		/// </summary>
		/// <param name="connector">The Connector object that rises the event</param>
		/// <param name="message">The message received</param>
		private void connector_MessageReceived (Connector connector, string message)
		{
			Event e = Serializer.DeserializeEvent (message);
			if (e == null)
				return;
		}

		/// <summary>
		/// Broadcasts a CONTINUE signal to the robot (See CONTINUE rule in the Rulebook)
		/// </summary>
		/// <param name="text">The text to be sent to the robot</param>
		public void SendContinueText(string text){
			this.cnn.Broadcast(Signal.CreateContinue(text));
		}

		/// <summary>
		/// Starts the message log, the test countdown, and broadcasts the START and first TIME signals
		/// </summary>
		/// <param name="testTime">Specifies the total test time in seconds</param>
		public void StartTest(int testTime){
			this.testTime = testTime;
			this.cnn.Start ();
			this.cnn.Broadcast (Signal.Start);
			this.sw.Reset ();
			this.sw.Start ();
			this.timer.Change (0, 60000);
		}

		/// <summary>
		/// Stops the  message log, the test countdown, and broadcasts the STOP signal
		/// </summary>
		public void StopTest(){
			this.cnn.Broadcast (Signal.Stop);
			this.cnn.Stop ();
			this.sw.Stop ();
		}

		/// <summary>
		/// Called by the timer every time the time elapses. It broadcasts the remaining time
		/// </summary>
		/// <param name="o">unused</param>
		private void TimerTick(object o){
			this.cnn.Broadcast (Signal.CreateTime (this.RemainingTime));
		}

		#endregion
	}
}

