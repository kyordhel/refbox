using System;
using System.Windows.Forms;

namespace RefBox
{
	public class Program
	{
		[STAThread]
		public static void Main (string[] args)
		{
			Refbox box = new Refbox ();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new RefBoxGUI() { Refbox = box });
			// Console.CancelKeyPress+=new ConsoleCancelEventHandler(CtrlC_event);
			// new UdpListener ().Run();

			// box.PrepareTest ("GPSR", new TimeSpan (0, 5, 0), "eR@sers");
			// box.StartTest ();
			// System.Threading.Thread.Sleep (50);
			// while (true) {
			//	Console.Write ("Enter CONTINUE text: ");
			// 	box.SendContinueText (Console.ReadLine ());
			// }
			// box.StopTest ();
		}
	}
}

