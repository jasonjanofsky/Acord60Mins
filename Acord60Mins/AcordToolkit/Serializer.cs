using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AcordToolkit
{
	public partial class TXLife_Type
	{
		public static TXLife_Type NewFromFile(string filePath)
		{
			string ss = System.IO.File.ReadAllText(filePath);
			return NewFromString(ss);
		}

		public static TXLife_Type NewFromString(string txLifeString) {
			XmlSerializer serializer = new XmlSerializer(typeof(TXLife_Type));
			TXLife_Type tt = (TXLife_Type)serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(txLifeString)));
			return tt;
		}

		public override string ToString()
		{
			return Serialize();
		}

		public string Serialize() {
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TXLife_Type));
			using (StringWriter textWriter = new Utf8StringWriter())
			{
				
				xmlSerializer.Serialize(textWriter, this);
				return textWriter.ToString();
			}
		}

		public string SaveToFile(string filePath) {
			return SerializeToFile(filePath);
		}

		public string SerializeToFile(string filePath)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TXLife_Type));
			using (TextWriter textWriter = new StreamWriter(filePath))
			{
				xmlSerializer.Serialize(textWriter, this);
				return textWriter.ToString();
			}
		}

		public class Utf8StringWriter : StringWriter
		{
			public override Encoding Encoding { get { return Encoding.UTF8; } }
		}
	}
}
