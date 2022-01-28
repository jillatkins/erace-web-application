using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRaceSystem.VIEWMODELS.Receiving
{
    public class PurchaseOrders
    {
        public int VendorID { get; set; }
        public int ? PurchaseOrderNumber { get; set; }
        public string VendorName { get; set; }
    }

}
