using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using eRaceSystem.DAL;
using eRaceSystem.VIEWMODELS.Purchasing;
using eRaceSystem.ENTITIES;

namespace eRaceSystem.BLL.Purchasing
{
    [DataObject]
    public class PurchasingController
    {
        #region Queries
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SelectionList> Vendor_List()
        {
            using (var context = new eRaceContext())
            {
                var results = from x in context.Vendors
                              select new SelectionList
                              {
                                  ID = x.VendorID,
                                  Description = x.Name
                              };
                return results.OrderBy(x => x.Description).ToList();
            }
        }

        public VendorDetails Vendor_Get(int vendorid)
        {
            using (var context = new eRaceContext())
            {
                var results = from v in context.Vendors
                              where v.VendorID == vendorid
                              select new VendorDetails
                              {
                                  Name = v.Name,
                                  Contact = v.Contact,
                                  Phone = v.Phone
                              };
                return results.FirstOrDefault();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<VendorCategory> VendorCategoryProducts_List(int vendorid)
        {
            using (var context = new eRaceContext())
            {
                var results = from vc in context.VendorCatalogs
                              where vc.VendorID == vendorid
                              group vc by vc.Product.Category into vcc
                              select new VendorCategory
                              {
                                  CategoryDescription = vcc.Key.Description,
                                  Products = from p in vcc
                                             select new VendorProductDetail
                                             {
                                                 ProductID = p.ProductID,
                                                 ItemName = p.Product.ItemName,
                                                 ItemSalePrice = p.Product.ItemPrice,
                                                 QuantityOnHand = p.Product.QuantityOnHand,
                                                 QuantityOnOrder = p.Product.QuantityOnOrder,
                                                 ReOrderLevel = p.Product.ReOrderLevel,

                                                 OrderUnitType = p.OrderUnitType,
                                                 OrderUnitSize = p.OrderUnitSize,
                                                 OrderUnitCost = p.OrderUnitCost
                                             }
                              };
                return results.ToList();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<VendorProductDetail> OrderDetailProducts_List(int vendorid, int orderid)
        {
            using (var context = new eRaceContext())
            {
                var results = (from vc in context.VendorCatalogs
                              join od in context.OrderDetails on vc.ProductID equals od.ProductID
                              join p in context.Products on vc.ProductID equals p.ProductID
                              where vc.VendorID == vendorid
                                && od.Order.Vendor.VendorID == vendorid
                                && od.OrderID == orderid
                              select new VendorProductDetail
                              {
                                  ProductID = p.ProductID,
                                  ItemName = p.ItemName,
                                  ItemSalePrice = p.ItemPrice,
                                  QuantityOnHand = p.QuantityOnHand,
                                  QuantityOnOrder = p.QuantityOnOrder,
                                  ReOrderLevel = p.ReOrderLevel,

                                  OrderUnitType = vc.OrderUnitType,
                                  OrderUnitSize = vc.OrderUnitSize,
                                  OrderUnitCost = vc.OrderUnitCost,

                                  OrderDetailID = od.OrderDetailID,
                                  POQuantity = od.Quantity,
                                  POCost = od.Cost
                              }).Distinct();
                return results.ToList();
            }
        }

        public PurchaseOrder PurchaseOrder_Find(int vendorid)
        {
            using (var context = new eRaceContext())
            {
                var results = from o in context.Orders
                              where o.VendorID == vendorid
                                && o.OrderDate == null
                                && o.OrderNumber == null
                              select new PurchaseOrder
                              {
                                  VendorID = o.VendorID,
                                  Name = o.Vendor.Name,
                                  Phone = o.Vendor.Phone,
                                  Contact = o.Vendor.Contact,

                                  OrderID = o.OrderID,
                                  OrderNumber = o.OrderNumber,
                                  OrderDate = o.OrderDate,
                                  Subtotal = o.SubTotal,
                                  TaxGST = o.TaxGST,
                                  Closed = o.Closed,
                                  Comment = o.Comment
                              };
                return results.FirstOrDefault();
            }
        }
        #endregion Queries

        #region Save
        public int PurchaseOrder_Save(PurchaseOrder purchaseOrder, List<VendorProductDetail> orderDetails, string userName)
        {
            int orderID = 0;
            using (var context = new eRaceContext())
            {
                // Get the employee ID from their username, no clue if this is correct, had to add users loginID to the employees table.
                /* Clarified with Robbin, isn't worried about resolving sent user against the DB so just hard coded an appropriate employeeID
                int? employeeID = (from e in context.Employees
                                  where e.LoginId == userName
                                  select e.EmployeeID).FirstOrDefault();

                if (employeeID == null)
                    throw new Exception("Invalid user for this activity...");
                */

                if (purchaseOrder.OrderID == 0) // New Order
                {
                    // Create new order in DB
                    Order order = new Order()
                    {
                        // OrderID is identity
                        OrderNumber = null,
                        OrderDate = null,
                        EmployeeID = 35, // Clarified with Robbin, isn't worried about resolving sent user against the DB so just hard coded an appropriate employeeID
                        TaxGST = purchaseOrder.TaxGST,
                        SubTotal = purchaseOrder.Subtotal,
                        VendorID = purchaseOrder.VendorID,
                        Closed = false,
                        Comment = purchaseOrder.Comment
                    };
                    // Get the identity orderID for the order details to FK
                    context.Orders.Add(order);
                    orderID = order.OrderID; 

                    // For each item create a new OrderDetail and add it to the DB
                    foreach (VendorProductDetail item in orderDetails)
                    {
                        OrderDetail orderItem = new OrderDetail()
                        {
                            // OrderDetailID is identity
                            OrderID = orderID,
                            ProductID = item.ProductID,
                            Quantity = (int)item.POQuantity,
                            OrderUnitSize = item.OrderUnitSize,
                            Cost = (decimal)item.POCost
                        };
                        context.OrderDetails.Add(orderItem);
                    }

                    // Commit Transaction
                    context.SaveChanges();
                }
                else // Existing Order
                {
                    // Lookup the old order items and remove from DB
                    var oldOrderItems = (from od in context.OrderDetails
                                         where od.OrderID == purchaseOrder.OrderID
                                         select od).ToList();

                    foreach (OrderDetail item in oldOrderItems)
                    {
                        context.OrderDetails.Remove(item);
                    }

                    // For each item create a new OrderDetail and add it to the DB 
                    foreach (VendorProductDetail item in orderDetails)
                    {
                        OrderDetail orderItem = new OrderDetail()
                        {
                            // OrderDetailID is identity
                            OrderID = purchaseOrder.OrderID,
                            ProductID = item.ProductID,
                            Quantity = (int)item.POQuantity,
                            OrderUnitSize = item.OrderUnitSize,
                            Cost = (decimal)item.POCost
                        };
                        context.OrderDetails.Add(orderItem);
                    }

                    // Make a new Order and UPDATE it in the DB
                    Order order = new Order()
                    {
                        OrderID = purchaseOrder.OrderID,
                        OrderNumber = null,
                        OrderDate = null,
                        EmployeeID = 35, // Clarified with Robbin, isn't worried about resolving sent user against the DB so just hard coded an appropriate employeeID
                        TaxGST = purchaseOrder.TaxGST,
                        SubTotal = purchaseOrder.TaxGST,
                        VendorID = purchaseOrder.VendorID,
                        Closed = false,
                        Comment = purchaseOrder.Comment
                    };
                    context.Entry(order).State = System.Data.Entity.EntityState.Modified;

                    // Complete Transaction
                    context.SaveChanges();
                }
                return orderID;
            }

        }

        public int PurchaseOrder_Place(PurchaseOrder purchaseOrder, List<VendorProductDetail> orderDetails, string userName)
        {
            int orderID = 0;
            using (var context = new eRaceContext())
            {
                // Get the employee ID from their username, no clue if this is correct, had to add users login to the employees table.
                /* Clarified with Robbin, isn't worried about resolving sent user against the DB so just hard coded an appropriate employeeID
                int? employeeID = (from e in context.Employees
                                   where e.LoginId == userName
                                   select e.EmployeeID).FirstOrDefault();

                if (employeeID == null)
                    throw new Exception("Invalid user for this activity...");
                */

                // Order Number is not an identity, so find the next highest number in sequence
                int? orderNumber = (from o in context.Orders
                                    select o.OrderNumber).Max();

                // This exception could get thrown if there are no records in the db, 
                //but probably dont want to just start at 1 again if it finds null if something went wrong with the db
                if (orderNumber == null)
                    throw new Exception("An error occurred finding an ordernumber from the DB...");
                else
                    orderNumber++;

                if (purchaseOrder.OrderID == 0) // New Order
                {
                    // Create new order in DB
                    Order order = new Order()
                    {
                        // OrderID is identity
                        OrderNumber = orderNumber,
                        OrderDate = DateTime.Now,
                        EmployeeID = 35, // Clarified with Robbin, isn't worried about resolving sent user against the DB so just hard coded an appropriate employeeID
                        TaxGST = purchaseOrder.TaxGST,
                        SubTotal = purchaseOrder.Subtotal,
                        VendorID = purchaseOrder.VendorID,
                        Closed = false,
                        Comment = purchaseOrder.Comment
                    };
                    // Get the identity orderID for the order details to FK
                    context.Orders.Add(order);
                    orderID = order.OrderID;

                    // For each item create a new OrderDetail and add it to the DB, also update the QuantityOnOrder in the Product Table
                    foreach (VendorProductDetail item in orderDetails)
                    {
                        OrderDetail orderItem = new OrderDetail()
                        {
                            // OrderDetailID is identity
                            OrderID = orderID,
                            ProductID = item.ProductID,
                            Quantity = (int)item.POQuantity,
                            OrderUnitSize = item.OrderUnitSize,
                            Cost = (decimal)item.POCost
                        };
                        context.OrderDetails.Add(orderItem);

                        var itemProduct = (from p in context.Products
                                           where p.ProductID == item.ProductID
                                           select p).First();
                        itemProduct.QuantityOnOrder += (int)item.POQuantity * item.OrderUnitSize;
                        context.Entry(itemProduct).State = System.Data.Entity.EntityState.Modified;
                    }
                    // Commit Transaction
                    context.SaveChanges();


                }
                else // Existing Order
                {
                    // Lookup the old order items and remove from DB
                    var oldOrderItems = (from od in context.OrderDetails
                                         where od.OrderID == purchaseOrder.OrderID
                                         select od).ToList();

                    foreach (OrderDetail item in oldOrderItems)
                    {
                        context.OrderDetails.Remove(item);
                    }

                    // For each item create a new OrderDetail and add it to the DB, also update the QuantityOnOrder in the Product Table
                    foreach (VendorProductDetail item in orderDetails)
                    {
                        OrderDetail orderItem = new OrderDetail()
                        {
                            // OrderDetailID is identity
                            OrderID = purchaseOrder.OrderID,
                            ProductID = item.ProductID,
                            Quantity = (int)item.POQuantity,
                            OrderUnitSize = item.OrderUnitSize,
                            Cost = (decimal)item.POCost
                        };
                        context.OrderDetails.Add(orderItem);

                        var itemProduct = (from p in context.Products
                                           where p.ProductID == item.ProductID
                                           select p).First();
                        itemProduct.QuantityOnOrder += (int)item.POQuantity * item.OrderUnitSize;
                        context.Entry(itemProduct).State = System.Data.Entity.EntityState.Modified;
                    }

                    // Make a new Order and UPDATE it in the DB
                    Order order = new Order()
                    {
                        OrderID = purchaseOrder.OrderID,
                        OrderNumber = orderNumber,
                        OrderDate = DateTime.Now,
                        EmployeeID = 35, // Clarified with Robbin, isn't worried about resolving sent user against the DB so just hard coded an appropriate employeeID
                        TaxGST = purchaseOrder.TaxGST,
                        SubTotal = purchaseOrder.TaxGST,
                        VendorID = purchaseOrder.VendorID,
                        Closed = false,
                        Comment = purchaseOrder.Comment
                    };
                    context.Entry(order).State = System.Data.Entity.EntityState.Modified;

                    // Complete Transaction
                    context.SaveChanges();
                }
                return orderID;
            }
        }
        #endregion Save

        #region Delete
        public void PurchaseOrder_Delete(int orderID)
        {
            using (var context = new eRaceContext())
            {
                // Check the order exists and is open. User shouldn't be able to send order id's that are placed but validate anyways
                var order = (from o in context.Orders
                             where o.OrderID == orderID
                                && o.OrderNumber == null
                                && o.OrderDate == null
                             select o).FirstOrDefault();

                if (order == null)
                    throw new Exception("That order ID does not exist or is placed...");

                // Find any OrderDetails that are associated with that orderID and delete them first, kill children first
                var orderDetailsList = (from od in context.OrderDetails
                                        where od.OrderID == orderID
                                        select od).ToList();

                foreach (OrderDetail item in orderDetailsList)
                {
                    var existingItem = context.OrderDetails.Find(item.OrderDetailID);
                    context.OrderDetails.Remove(existingItem);
                }

                // Find and delete the order
                var existingOrder = context.Orders.Find(orderID);
                context.Orders.Remove(existingOrder);

                // Commit Transaction
                context.SaveChanges();
            }
        }
        #endregion Delete
    }
}
