using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eRaceSystem.DAL;
using eRaceSystem.ENTITIES;
using eRaceSystem.VIEWMODELS.Receiving;

namespace eRaceSystem.BLL.Receiving
{
    [DataObject]
    public class ReceivingController
    {
        //Step 1: create BLL method to populate my dropdown list of vendor names and active purchase order numbers
        public List<KeyValueOption<int>> Vendor_DDL()
        {
            using (var context = new eRaceContext())
            {
                var results = from x in context.Orders
                              where !x.Closed && x.OrderNumber != null && x.OrderDate != null
                              select new KeyValueOption<int>
                              {
                                    DisplayText = x.OrderNumber + " - " + x.Vendor.Name,
                                    Key = x.VendorID
                              };
                return results.ToList();
            }
        }
        //step 3: create BLL method to grab vendor details from the dropdown list using the vendorID value      
        public VendorDetails GetVendorDetails(int vendorID)
        {
            using (var context = new eRaceContext())
            {
                var results = from x in context.Vendors
                              where x.VendorID == vendorID
                              select new VendorDetails
                              {
                                  VendorName = x.Name,
                                  Address = x.Address + " " + x.City,
                                  ContactName = x.Contact,
                                  Phone = x.Phone
                              };
                return results.FirstOrDefault();
            }
        }
        public List<PurchaseOrderDetails> GetPurchaseOrderDetails(int vendorID)
        {
            using (var context = new eRaceContext())
            {
                var results = (from ord in context.Orders
                              join ordDetails in context.OrderDetails on ord.OrderID equals ordDetails.OrderID
                              join prodDetails in context.Products on ordDetails.ProductID equals prodDetails.ProductID
                              join receivedOrder in context.ReceiveOrderItems on ordDetails.OrderDetailID equals receivedOrder.OrderDetailID
                              join returnOrder in context.ReturnOrderItems on ordDetails.OrderDetailID equals returnOrder.OrderDetailID
                              where ord.VendorID == vendorID
                              select new PurchaseOrderDetails
                              {
                                    OrderID = ordDetails.OrderID,
                                    ItemName = prodDetails.ItemName,
                                    QtyOrdered = ordDetails.Quantity * ordDetails.OrderUnitSize,
                                    OrderUnitSize = ordDetails.Quantity/ ordDetails.OrderUnitSize,
                                    
                                    QuantityOutstanding = (ordDetails.Quantity * ordDetails.OrderUnitSize) - (receivedOrder.ItemQuantity * ordDetails.OrderUnitSize),
                                    ReceivedItemQuantity = receivedOrder.ItemQuantity/ordDetails.OrderUnitSize,
                                    RejectedUnits = returnOrder.ItemQuantity/ordDetails.OrderUnitSize,
                                    ReturnReason = returnOrder.Comment,                                   
                              }).Distinct();
                return results.ToList();
            }
        }
        //public void ReceiveOrder()
        //{
        //    ReceiveOrder order = null;
        //    using (var context  = new eRaceContext())
        //    {

        //    }
        //}
        //public void CloseOrder()
        //{

        //}
    }
}
