using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RefBox
{
	public class UdpListener
	{
		private IPEndPoint broadcastEP;
		private UdpClient listener;
		private int listenPort;

		public UdpListener (int listenPort = 3001)
		{
			this.listenPort = listenPort;
			this.listener = new UdpClient(listenPort);
			this.broadcastEP = new IPEndPoint (IPAddress.Any, listenPort);
		}

		public void Run(){
			Thread t = new Thread (new ThreadStart(AsyncRcv));
			t.IsBackground = true;
			t.Start ();
		}

		private void AsyncRcv()
		{
			Console.WriteLine ("Udp Listener 3001");
			IPEndPoint clientEP = new IPEndPoint (IPAddress.Any, this.listenPort);
			while (true) {
				string text = ASCIIEncoding.UTF8.GetString( listener.Receive (ref clientEP));
				Console.WriteLine ("{0} says: {1}", clientEP, text);
			}
		}

		public void Poll()
		{
			Console.WriteLine ("Udp Listener");
			Console.WriteLine("Waiting for broadcast");
			IPEndPoint clientEP = new IPEndPoint (IPAddress.Any, this.listenPort);
			string response = Serializer.Serialize (Signal.Start);

			while (true) {
				string text = ASCIIEncoding.UTF8.GetString( listener.Receive (ref clientEP));
				Console.WriteLine ("{0} says: {1}", clientEP, text);
				if(text != response)
				Send (response, clientEP);
			}
		}

		public void Send(string tts, IPEndPoint ep){
			byte[] dgram = ASCIIEncoding.UTF8.GetBytes (tts);
			listener.Send (dgram, dgram.Length, ep);
		}

		public void Send(string tts){
			byte[] dgram = ASCIIEncoding.UTF8.GetBytes (tts);
			listener.Send (dgram, dgram.Length, broadcastEP);
		}
	}
}

