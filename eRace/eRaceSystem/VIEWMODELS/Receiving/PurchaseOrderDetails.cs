using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRaceSystem.VIEWMODELS.Receiving
{
    public class PurchaseOrderDetails
    {
        public int OrderID { get; set; }
        public int? OrderNumber { get; set; }
        public string ItemName { get; set; }
        public int QtyOrdered { get; set; }
        public int OrderUnitSize { get; set; }
        public int QuantityOutstanding { get; set; }
        public int ReceivedItemQuantity { get; set; }
        public int RejectedUnits { get; set; }
        public string ReturnReason { get; set; }
        public int SalvagedItems { get; set; }
        public int VendorID { get; set; }
    }
}
