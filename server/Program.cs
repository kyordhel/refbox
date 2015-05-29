using System;

namespace RefBox
{
	public class Program
	{
		public static void Main (string[] args)
		{
			// Console.CancelKeyPress+=new ConsoleCancelEventHandler(CtrlC_event);
			// new UdpListener ().Run();
			Refbox box = new Refbox ();
			box.PrepareTest ("GPSR", new TimeSpan (0, 5, 0), "eR@sers");
			box.StartTest ();
			System.Threading.Thread.Sleep (50);
			while (true) {
				Console.Write ("Enter CONTINUE text: ");
				box.SendContinueText (Console.ReadLine ());
			}
			box.StopTest ();
		}
	}
}

