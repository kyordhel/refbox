using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RefBox
{
	public static class Serializer
	{
		#region Variables

		/// <summary>
		/// Stores the namespace strings for serialized objects
		/// </summary>
		private static readonly XmlSerializerNamespaces ns;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes the <see cref="RefBox.Serializer"/> class.
		/// </summary>
		static Serializer(){
			Serializer.ns = new XmlSerializerNamespaces();
			Serializer.ns.Add ("", "");
		}

		#endregion

		#region Methods

		/// <summary>
		/// Serializes the specified object into a string.
		/// </summary>
		/// <param name="s">The object to serialize.</param>
		/// <returns>A string containing the XML representation of the object.</returns>
		public static string Serialize(Signal s){
			return SerializeObject (s);
		}

		/// <summary>
		/// Deserializes an event from the provided string s. If deserialization fails, returns null
		/// </summary>
		/// <param name="s">A string containing an event</param>
		public static Event DeserializeEvent(string s){
			try {
				using (StringReader textReader = new StringReader(s)) {
					XmlSerializer serializer = new XmlSerializer (typeof(Event));
					return (Event)serializer.Deserialize (textReader);
				}
			} catch{
				return null;
			}
		}

		/// <summary>
		/// Serializes the specified object into a string.
		/// </summary>
		/// <param name="o">The object to serialize.</param>
		/// <returns>A string containing the XML representation of the object.</returns>
		private static string SerializeObject(object o)
		{
			string serialized;
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
			settings.Indent = true;
			settings.OmitXmlDeclaration = true;
			using (StringWriter textWriter = new StringWriter())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings)) {
					XmlSerializer serializer = new XmlSerializer (o.GetType());
					serializer.Serialize (xmlWriter, o, ns);
				}
				serialized = textWriter.ToString ();
			}
			return serialized;
		}

		#endregion
	}
}

