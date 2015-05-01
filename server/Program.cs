using System;

namespace RefBox
{
	public class Program
	{
		public static void Main (string[] args)
		{
			new UdpListener ().Run();
			Refbox box = new Refbox ();
			box.StartTest (480);
			System.Threading.Thread.Sleep (50);
			while (true) {
				Console.Write ("Enter CONTINUE text: ");
				box.SendContinueText (Console.ReadLine ());
			}
			box.StopTest ();
		}
	}
}

