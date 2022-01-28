using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRaceSystem.VIEWMODELS.Purchasing
{
    public class VendorCategory
    {
        public string CategoryDescription { get; set; }
        public IEnumerable<VendorProductDetail> Products { get; set; }
    }
}
