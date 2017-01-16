using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

public partial class ConverterRules
{
	public static ConverterRules NewFromFile(string filePath)
	{
		string ss = System.IO.File.ReadAllText(filePath);
		return NewFromString(ss);
	}

	public static ConverterRules NewFromString(string converterRulesString)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(ConverterRules));
		ConverterRules tt = (ConverterRules)serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(converterRulesString)));
		return tt;
	}

	public override string ToString()
	{
		return Serialize();
	}

	public string Serialize()
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConverterRules));
		using (StringWriter textWriter = new StringWriter())
		{
			xmlSerializer.Serialize(textWriter, this);
			return textWriter.ToString();
		}
	}

	public string SaveToFile(string filePath)
	{
		return SerializeToFile(filePath);
	}

	public string SerializeToFile(string filePath)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConverterRules));
		using (TextWriter textWriter = new StreamWriter(filePath))
		{
			xmlSerializer.Serialize(textWriter, this);
			return textWriter.ToString();
		}
	}
}

