using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRaceSystem.VIEWMODELS.Purchasing
{
    public class PurchaseOrder
    {
		// Vendor Info
		public int VendorID { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Contact { get; set; }

		// Order Info
		public int OrderID { get; set; }
		public int? OrderNumber { get; set; }
		public DateTime? OrderDate { get; set; }
		public decimal Subtotal { get; set; }
		public decimal TaxGST { get; set; }
		public bool Closed { get; set; } 
		public string Comment { get; set; }
	}
}
