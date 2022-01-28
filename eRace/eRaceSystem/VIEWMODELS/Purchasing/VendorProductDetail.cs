using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRaceSystem.VIEWMODELS.Purchasing
{
    public class VendorProductDetail
    {
        // Product
        public int ProductID { get; set; }
        public string ItemName { get; set; }
        public decimal ItemSalePrice { get; set; }
        public int QuantityOnHand { get; set; }
        public int QuantityOnOrder { get; set; }
        public int ReOrderLevel { get; set; }

        // OrderDetail
        public int? OrderDetailID { get; set; }
        public int? POQuantity { get; set; }
        public decimal? POCost { get; set; }

        // VendorCatalog
        public string OrderUnitType { get; set; }
        public int OrderUnitSize { get; set; }
        public decimal OrderUnitCost { get; set; }
    }
}
