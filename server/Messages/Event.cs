using System;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace RefBox
{
	[Serializable, XmlRoot(ElementName = "event", Namespace = "")]
	public class Event
	{
		/// <summary>
		/// Gets or sets the type of the signal
		/// </summary>
		[XmlAttribute("type")]
		public string Type { get; set; }

		/// <summary>
		/// Gets the signal value.
		/// </summary>
		[XmlText]
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the IPEndPoint where message comes from.
		/// </summary>
		[XmlIgnore]
		public IPEndPoint Source{ get; set; }
	}
}

