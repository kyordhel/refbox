using System;
using System.Windows.Forms;
using RefBox.Terminal;

namespace RefBox
{
	public class Program
	{
		[STAThread]
		public static void Main (string[] args)
		{
			Console.Title = "RefBox Server";
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Refbox box = new Refbox ();
			Console.WriteLine("RefBox Server Started");
			Console.WriteLine("Press F1 to start the GUI");
			Console.WriteLine();

			Kernel kernel = new Kernel(box);
			ConsoleManager consoleManager = new ConsoleManager(box, kernel);
			kernel.Execute("help");
			consoleManager.WritePrompt();
			while (true)
			{
				consoleManager.Poll();
			}
			Console.WriteLine("Good bye!");
			Console.WriteLine();

			// Console.WriteLine("")
			RunGUI(box);
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

		public static void RunGUI(Refbox box)
		{
			Application.Run(new RefBoxGUI() { Refbox = box });
		}
	}
}

