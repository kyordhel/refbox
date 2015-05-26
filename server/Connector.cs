using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RefBox
{
	/// <summary>
	/// Represents the method that will handle the MessageReceived event of a connector
	/// </summary>
	/// <param name="connector">The Connector object that rises the event</param>
	/// <param name="source">The IPEndPoint where message comes from</param>
	/// <param name="message">The message received</param>
	public delegate void MessageReceivedEventHandler(Connector connector, IPEndPoint source, string message);

	/// <summary>
	/// Implements UDP connectivity
	/// </summary>
	public class Connector
	{
		#region Variables

		/// <summary>
		/// The input port.
		/// </summary>
		public const int portIn = 3000;
		/// <summary>
		/// The output port.
		/// </summary>
		public const int portOut = 3001;

		/// <summary>
		/// Udp listener on input port
		/// </summary>
		private UdpClient listener;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="RefBox.Connector"/> class.
		/// </summary>
		public Connector ()
		{

		}

		#endregion

		#region Properties
		#endregion

		#region Events

		/// <summary>
		/// Occurs when a message is received.
		/// </summary>
		public event MessageReceivedEventHandler MessageReceived;

		#endregion

		#region Methods

		/// <summary>
		/// Broadcast the specified signal.
		/// </summary>
		/// <param name="signal">The signal to send.</param>
		public void Broadcast(Signal signal){
			if (listener == null)
				return;

			// Serialize signal
			string serialized = Serializer.Serialize (signal);
			// Get DGRAM
			byte[] dgram = ASCIIEncoding.UTF8.GetBytes (serialized + "\x03");
			// Get all ip addresses
			IPAddress[] addresses = Dns.GetHostAddresses (Dns.GetHostName ());
			// Over each IP addresses
			for(int i = 0; i < addresses.Length; ++i)
			{
				byte[] bAddress = addresses [i].GetAddressBytes ();
				bAddress [3] = 255;
				IPAddress address = new IPAddress (bAddress);
				IPEndPoint ep = new IPEndPoint (address, portOut);
				listener.Send (dgram, dgram.Length, ep);
			}
		}

		/// <summary>
		/// Raises the message received event.
		/// </summary>
		/// <param name="message">The message's source.</param>
		/// <param name="message">The message.</param>
		protected void OnMessageReceived(string message, IPEndPoint source){
			try{
				if(this.MessageReceived != null)
					MessageReceived(this, source, message);
			}catch{}
		}

		private void ReceiveCallback(IAsyncResult result)
		{
			if (listener == null)
				return;
			IPEndPoint clientEP = new IPEndPoint (IPAddress.Any, portIn);
			byte[] data = this.listener.EndReceive (result, ref clientEP);
			this.listener.BeginReceive (ReceiveCallback, null);
			string sData = ASCIIEncoding.UTF8.GetString (data);
			OnMessageReceived(sData, clientEP);
		}

		/// <summary>
		/// Starts async reception of UDP messages
		/// </summary>
		public void Start(){
			if(listener != null)
				return;
			this.listener = new UdpClient (portIn);
			this.listener.EnableBroadcast = true;
			this.listener.DontFragment = true;
			this.listener.BeginReceive(ReceiveCallback, null);
		}

		/// <summary>
		/// Stops async reception of UDP messages
		/// </summary>
		public void Stop(){
			this.listener.Close ();
			this.listener = null;
		}

		#endregion
	}
}

