using System;
using System.Xml;
using System.Xml.Serialization;

namespace RefBox
{
	[Serializable, XmlRoot(ElementName = "signal", Namespace = "")]
	public class Signal
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

		public override string ToString ()
		{
			return string.Format ("[Signal: Type={0}, Value={1}]", Type, Value);
		}

		/// <summary>
		/// Creates a time signal with the specified remaining time in seconds
		/// </summary>
		/// <param name="remainingTime">Remaining time in seconds</param>
		public static Signal CreateTime(int remainingTime){
			if (remainingTime < 0)
				remainingTime = 0;
			return new Signal () { Type="time", Value = remainingTime.ToString() };
		}

		/// <summary>
		/// Creates a continue (CONTINUE Rule) signal with the specified text
		/// </summary>
		/// <param name="remainingTime">Text to send to the robot</param>
		public static Signal CreateContinue(string text){
			return new Signal () { Type="continue", Value = text };
		}

		/// <summary>
		/// Gets a start signal
		/// </summary>
		public static Signal Start{
			get{ return new Signal () { Type="start", Value = "START" }; }
		}

		/// <summary>
		/// Gets a stop signal
		/// </summary>
		public static Signal Stop{
			get{ return new Signal () { Type="stop", Value = "STOP" }; }
		}

		/// <summary>
		/// Gets a time signal with a remaining time of zero
		/// </summary>
		public static Signal Time{
			get{ return new Signal () { Type="time", Value = "0" }; }
		}
	}
}

