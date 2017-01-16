using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcordToolkit;

namespace Converters
{
	public class CompanyA : IConverter
	{
		public string Process(string txLife) {
			TXLife_Type txr = TXLife_Type.NewFromString(txLife);

			foreach(TXLifeRequest_Type txrr in txr.Items.Where(t=>t.GetType() == typeof(TXLifeRequest_Type))) {
				txrr.TransTrackingID = "I Converted!";
			}

			return txr.ToString();
		}
	}
}
