using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using AcordToolkit;

namespace Acord60Mins
{
	public class IncomingRequest
	{

		//create two lists for our internal/external errors.  One we can save out to 
		//an internal log, the other will be shared through the TXLifeResponse.
		List<string> internalErrors = new List<string>();
		List<string> publicErrors = new List<string>();


		private string _appPath;
		/// <summary>
		/// The path of this application - we need to get the location of the converters dll to run the conversions.
		/// </summary>
		public string AppPath
		{
			get
			{
				if (_appPath == null) {
					var env = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
					_appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "").Trim();
				}
				return _appPath;
			}
			set 
			{
				_appPath = value;
			}
		}

		private string _rulesPath;
		/// <summary>
		/// The path to the rules file that we will want to run against.
		/// </summary>
		public string RulesPath
		{
			get
			{
				if (_rulesPath == null) {
					_rulesPath = "ConverterRules.xml";
				}
				return _rulesPath;
			}
			set {
				_rulesPath = value;
			}
		}

		private string _convertersDLLPath;
		/// <summary>
		/// The location of the converters dll file.
		/// </summary>
		public string ConvertersDLLPath
		{
			get
			{
				if (_convertersDLLPath == null) {
					_convertersDLLPath = AppPath + "\\Converters.dll";
				}
				return _convertersDLLPath;
			}
			set {
				_convertersDLLPath = value;
			}
		}


		/// <summary>
		/// The full receive method takes in a string txlife file, runs client conversion on it, based on the
		/// auth in the file and attempts to convert it from the fules located in the ConverterRules.xml file.  The object can
		/// then can be shipped off to another location for persistence.  Finally, a TXLifeResponse is returned.  This
		/// method is meant to be hooked up to a web controller, or other, wherein a string is received and a response is
		/// sent out.  Validation on the TXLife is not performed in here as, when receiving from many clients, one might
		/// need to convert a file into a properly validating file.
		/// </summary>
		/// <param name="txLifeRequest">A string of the TXLifeRequest file.</param>
		/// <returns>A string representation of the TXLifeResponse</returns>
		public string Receive(string txLifeRequest)
		{
			//clear out our existing errors.
			this.internalErrors.Clear();
			this.publicErrors.Clear();

			//process the conversion from the clientrules xml file.
			txLifeRequest = ProcessClientConverion(txLifeRequest);

			//setup our deserialization.
			TXLife_Type txr = null;

			try
			{
				//Create our txlife object from the incoming string.
				txr = TXLife_Type.NewFromString(txLifeRequest);
			}
			catch (Exception ex)
			{
				internalErrors.Add($"Error Reading TXLifeRequest Conversion Failed, the error was, \"{ex.Message}\".");
				publicErrors.Add($"Could not read the TXLife file.");
			}

			//setup the response object for sending back out.
			TXLife_Type response = new TXLife_Type();
			TXLifeResponse_Type txrr = new TXLifeResponse_Type();
			response.Items = new object[] { txrr };
			txrr.TransRefGUID = System.Guid.NewGuid().ToString();
			txrr.TransExeDate = System.DateTime.Now;
			txrr.TransExeTime = System.DateTime.Now;
			txrr.TransResult = new TransResult_Type();
			txrr.TransResult.ResultCode = new RESULT_CODES();
			
			if (publicErrors.Count == 0) {
				txrr.TransResult.ResultCode.tc = "1";
				txrr.TransResult.ResultCode.Value = "Success";
			} else {
				txrr.TransResult.ResultCode.tc = "5";
				txrr.TransResult.ResultCode.Value = "Failure";
				
				//Log out the public errors.  We are hard coding the errors here at a severe, cannot be overridden.
				//There is a possibility to add in severity here.
				foreach(string ss in publicErrors) {
					txrr.TransResult.ResultInfo = new ResultInfo_Type[] { new ResultInfo_Type { ResultInfoDesc = ss, ResultInfoSeverity = new OLI_LU_MSGSEVERITY { tc = "5", Value = "The message is severe and cannot be overridden." } } };
				}
			}
		
			//return the TXLife Response
			return response.ToString();
		}

		/// <summary>
		/// The client conversion is processed based on the converterrules.xml file
		/// and the conversion method is run through reflection.
		/// </summary>
		/// <param name="txLifeRequest"></param>
		/// <returns></returns>
		public string ProcessClientConverion(string txLifeRequest) {
			//grab the converter rule from the converterrules.xml file and
			//run the conversion from the converters.dll file.
			ConverterRule converterRule = GetConverterRule(txLifeRequest);

			//Did we get a rule back?  If not, carry on, nothing to see here.
			if (!string.IsNullOrEmpty(converterRule?.Class))
			{
				//if we did get a converter rule back, run it here.
				txLifeRequest = RunConversion(txLifeRequest, converterRule);
			}

			//send back our txlife post conversion.
			return txLifeRequest;
		}

		/// <summary>
		/// Determines which converter rule we need based on a node within the incoming AcordLife file.
		/// </summary>
		/// <param name="TXLife">The TXLife incoming file.</param>
		/// <returns>The converter rule that should be run, based on the incoming file.</returns>
		private ConverterRule GetConverterRule(string TXLife)
		{
			XmlDocument xmlDoc = new XmlDocument();
			try {
				xmlDoc.LoadXml(TXLife);
			} catch(Exception ex) {
				internalErrors.Add($"Could not read the xml file to determine how to map for conversion, the error was, \"{ex.Message}\".");
				publicErrors.Add($"Could not read the xml file to validate the incoming account.");
			}
			
			string companyName = string.Empty;
			XmlNodeList xnl = null;

			//grab the userloginname from the file and let's get our file like this.
			xnl = xmlDoc.GetElementsByTagName("UserLoginName");

			if (xnl != null && xnl.Count > 0 && !string.IsNullOrEmpty(xnl[0].InnerText))
			{
				companyName = xnl[0].InnerText;
			}

			ConverterRule p = null;
			if (!string.IsNullOrEmpty(companyName))
			{
				//now, process the client rules from the login name in the incoming request.
				p = ProcessClientRules(companyName.Trim());
			}
			return p;
		}

		private ConverterRule ProcessClientRules(string AccountName)
		{
			ConverterRules cc = null;
			try {
				cc = ConverterRules.NewFromFile(RulesPath);
			} catch(Exception ex) {
				internalErrors.Add($"Could not load converter rules file, the error was, \"{ex.Message}\".");
				publicErrors.Add($"Could not get internal rules to convert the file.");
			}

			ConverterRule crr = cc.ConverterRule.Where(t => String.Equals(t.Name, AccountName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

			return crr;
		}

		private string RunConversion(string xmlContent, ConverterRule cc)
		{
			string parms = xmlContent;
			object o = null;
			
			try {
				o = Acord60Mins.Reflection.RunMethod(ConvertersDLLPath, cc.Class.Trim(), "Process", parms);
				return (string)o;
			} catch(Exception ex) {
				internalErrors.Add($"Could not convert, running the converter failed, the error was, \"{ex.Message}\".");
				publicErrors.Add("We were unable to process the transaction, conversion failed.");
			}
			
			return xmlContent;
		}
	}
}

