using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using AcordToolkit;

namespace AcordTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void Load121()
		{
			//Simple test to see if we can load and deserialize a known good TXLife file.
			TXLife_Type tt = TXLife_Type.NewFromFile("Sample_121_Request.xml");
		}

		[TestMethod]
		public void Create121() {
			//simple test to see if we can create and output a TXLife.

			TXLife_Type txl = new TXLife_Type();
			TXLifeRequest_Type txr = new TXLifeRequest_Type();
			txl.Items = new TXLifeRequest_Type[] { txr };
			OLI_LU_TRANS_TYPE_CODES transCode = new OLI_LU_TRANS_TYPE_CODES();

			txr.TransType = new OLI_LU_TRANS_TYPE_CODES();
			txr.TransType.tc = "121";
			txr.TransType.Value = "General Requirement Order Request";

			string ss = txl.ToString();

			TXLife_Type final = TXLife_Type.NewFromString(ss);
		}


		[TestMethod]
		public void LoadConverterRules()
		{
			//simple test to see if we can load a known good converter rules file.
			ConverterRules cc = ConverterRules.NewFromFile("ConverterRules.xml");	
		}

		[TestMethod]
		public void PerformFileConversion() {

			//simple test to see if our converters are working.
			string ss = System.IO.File.ReadAllText("Sample_121_Request.xml");

			Acord60Mins.IncomingRequest irr = new Acord60Mins.IncomingRequest();
			string output = irr.ProcessClientConverion(ss);

			Assert.IsTrue(output.Contains("I Converted!"), "The text \"I Converted!\" should have appeared in the TXLifeRequest Tracking ID node");
		}

		[TestMethod]
		public void PerformTXLifeValidation()
		{
			//validate a TXLife file by taking a known good and changing some key items.  This test should succeed
			//if the validator catches the errors.			
			string ss = System.IO.File.ReadAllText("Sample_121_Request.xml");
			
			//put in some junk.
			ss = ss.Replace("Party", "Smarty").Replace("ReqCode", "BadNode");
			
			//use our latest known txlife schema file.
			AcordToolkit.Validator vv = Validator.NewFromSchemaFile("TXLife2.36.00.xsd");
			
			//run the validation
			vv.ValidateTXLife(ss);

			if (vv.ValidationErrors.Count > 0) {
				vv.ValidationErrors.ForEach(t => System.Diagnostics.Debug.WriteLine($"At Line: {t.Line} there is an error: {t.Message}, here is the full message: {t.FullMessage}"));
			} else {
				Assert.Fail("Validation did not pick up any errors");
			}
		}

		[TestMethod]
		public void ReceptionTest()
		{
			//Test to run through the complete reception method.
			string ss = System.IO.File.ReadAllText("Sample_121_Request.xml");

			Acord60Mins.IncomingRequest irr = new Acord60Mins.IncomingRequest();
			string output = irr.Receive(ss);

			TXLife_Type response = TXLife_Type.NewFromString(output);
		}
	}
}