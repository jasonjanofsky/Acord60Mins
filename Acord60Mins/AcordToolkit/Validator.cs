using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace AcordToolkit
{
	/// <summary>
	/// Allows for validation of Acord files against a specified standard.  This object uses a constructor factory model.  When creating the object
	/// use, "Validator vv = Validator.NewFromSchemaString(filePath);" or another constructor like "NewFromSchemaFile or NewFromSchemaURL".
	/// </summary>
	public class Validator
	{
		/// <summary>
		/// Errors that were picked up during TXLife File validation.
		/// </summary>
		public List<XmlValidationError> ValidationErrors = new List<XmlValidationError>();

		/// <summary>
		/// Stores the xml reader settings for the validation run.
		/// </summary>
		XmlReaderSettings readerSettings = new XmlReaderSettings();
		
		/// <summary>
		/// Takes in an XSD file stored to a string and performs validation runs against it.
		/// </summary>
		/// <param name="xsdString">The XSD file in a string.</param>
		/// <returns>A Validator object prepared to run against the XSD string.</returns>
		public static Validator NewFromSchemaString(string xsdString) {
			Validator vv = new Validator();
			TextReader tt = new StringReader(xsdString);
			vv.readerSettings.Schemas.Add(XmlSchema.Read(tt, null));
			return vv;
		}


		/// <summary>
		/// Takes in an XSD file path performs validation runs against it.
		/// </summary>
		/// <param name="filePath">The XSD file location.</param>
		/// <returns>A Validator object prepared to run against the XSD file.</returns>
		public static Validator NewFromSchemaFile(string filePath)
		{
			Validator vv = new Validator();
			TextReader tt = new StreamReader(filePath);
			vv.readerSettings.Schemas.Add(XmlSchema.Read(tt, null));
			return vv;
		}

		/// <summary>
		/// Takes in an XSD url path performs validation runs against it.
		/// </summary>
		/// <param name="url">The XSD file url.</param>
		/// <returns>A Validator object prepared to run against the XSD url.</returns>
		public static Validator NewFromSchemaURL(string url) {
			Validator vv = new Validator();
			vv.readerSettings.Schemas.Add(null, url);
			return vv;
		}

		/// <summary>
		/// Runs a validation against the TXLife Schema on a file.
		/// </summary>
		/// <param name="filePath">The location of the TXLife</param>
		/// <returns>A list of validation issues.  If empty, the file validates!</returns>
		public List<XmlValidationError> ValidateTXLifeFile(string filePath)
		{
			string ss = System.IO.File.ReadAllText(filePath);
			return ValidateTXLife(ss);
		}

		/// <summary>
		/// Runs a validation against the TXLife Schema on a string.
		/// </summary>
		/// <param name="xmlDoc">The TXLife file in a string.</param>
		/// <returns>A list of validation issues.  If empty, the file validates!</returns>
		public List<XmlValidationError> ValidateTXLife(string xmlDoc)
		{
			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlDoc));
			return ValidateTXLife(ms);
		}

		/// <summary>
		/// Runs a validation against the TXLife Schema on a stream.
		/// </summary>
		/// <param name="xmlDoc">The TXLife file in a stream.</param>
		/// <returns>A list of validation issues.  If empty, the file validates!</returns>
		public List<XmlValidationError> ValidateTXLife(Stream xmlDoc)
		{
			ValidationErrors.Clear();
			XmlReaderSettings xmls = new XmlReaderSettings();
			
			readerSettings.ValidationType = ValidationType.Schema;
			readerSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
			readerSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
			readerSettings.ValidationEventHandler += ValidationCallBack;

			XmlReader reader = null;
			try
			{
				reader = XmlReader.Create(xmlDoc, readerSettings);
			}
			catch (XmlSchemaException ex)
			{
				ValidationErrors.Add(new XmlValidationError("An error occured loading the schema, verify that all valid schema files are loading properly.  The error was: " + ex.Message, ex.LineNumber, ex.LinePosition, XmlSeverityType.Error, 201));
			}

			try
			{
				while (reader.Read())
				{

				}
			}
			catch (XmlException ex)
			{
				ValidationErrors.Add(new XmlValidationError("Could not read the xml file, the error was: " + ex.Message, ex.LineNumber, ex.LinePosition, XmlSeverityType.Error, 201));
			}

			return ValidationErrors;
		}
		private void ValidationCallBack(object sender, System.Xml.Schema.ValidationEventArgs e)
		{
			ValidationErrors.Add(new XmlValidationError(e.Message, e.Exception.LineNumber, e.Exception.LinePosition, e.Severity, 201));
		}
	}

	/// <summary>
	/// A representation of an error from the XML File.
	/// </summary>
	public class XmlValidationError
	{
		/// <summary>
		/// Contains details on the node, if a node is missing, this property will contain the node and the names of other nodes that 
		/// should appear on the parent node.
		/// </summary>
		public string FullMessage { get; set; }

		public int Line { get; set; }
		public int Column { get; set; }
		public XmlSeverityType Severity { get; set; }
		public int? AcordErrCode { get; set; }

		/// <summary>
		/// The shorter message on the error.
		/// </summary>
		public string Message { get; set; }
		public string Help { get; set; }

		public XmlValidationError() { }
		public XmlValidationError(string msg, int line, int col, XmlSeverityType sev)
		{
			this.FullMessage = msg;
			this.Line = line;
			this.Column = col;
			this.Severity = sev;
			string fullpadded = this.FullMessage.PadRight(this.FullMessage.Length + 1);
			if (fullpadded.Contains(". "))
			{
				this.Message = fullpadded.Substring(0, fullpadded.IndexOf(". ") + 1);
				if (fullpadded.IndexOf(". ") + 2 <= fullpadded.Length)
				{
					this.Help = fullpadded.Substring(fullpadded.IndexOf(". ") + 2);
				}
			}
			this.AcordErrCode = 999;
		}
		public XmlValidationError(string msg, int line, int col, XmlSeverityType sev, int? acordErrorCode)
		{
			this.FullMessage = msg;
			this.Line = line;
			this.Column = col;
			this.Severity = sev;
			string fullpadded = this.FullMessage.PadRight(this.FullMessage.Length + 1);
			if (fullpadded.Contains(". "))
			{
				this.Message = fullpadded.Substring(0, fullpadded.IndexOf(". ") + 1);
				if (fullpadded.IndexOf(". ") + 2 <= fullpadded.Length)
				{
					this.Help = fullpadded.Substring(fullpadded.IndexOf(". ") + 2);
				}
			}
			this.AcordErrCode = acordErrorCode.GetValueOrDefault(999);
		}
	}

}
