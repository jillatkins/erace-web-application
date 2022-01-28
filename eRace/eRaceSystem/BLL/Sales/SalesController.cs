using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using eRaceSystem.ENTITIES;
using eRaceSystem.DAL;
using eRaceSystem.VIEWMODELS.Sales;
using System.Data.Entity;
using System.Data.SqlClient;
using System.ComponentModel;
#endregion

namespace eRaceSystem.BLL.Sales
{
    [DataObject]
    public class SalesController
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CategoryList> Categories_List()
        {
            using (var context = new eRaceContext())
            {
                var result = from x in context.Categories
                             select new CategoryList
                             {
                                 CategoryID = x.CategoryID,
                                 Description = x.Description
                             };
                return result.ToList();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<ProductsList> Products_By_Category(int categoryId)
        {
            using (var context = new eRaceContext())
            {
                var result = from x in context.Products
                             where x.CategoryID == categoryId
                             select new ProductsList
                             {
                                 ProductID = x.ProductID,
                                 ItemName = x.ItemName
                             };
                return result.ToList();
            }
        }

        public List<ProductsList> Products_List(int productID)
        {
            using (var context = new eRaceContext())
            {
                var result = from x in context.Products
                             where x.ProductID == productID
                             select new ProductsList
                             {
                                 ProductID = x.ProductID,
                                 ItemName = x.ItemName,
                                 ItemPrice = x.ItemPrice,
                                 ReStockCharge = x.ReStockCharge
                             };
                return result.ToList();
            }
        }

        public ProductSummary GetProducts(int productID)
        {
            using (var context = new eRaceContext())
            {
                var results = (from x in context.Products
                               where x.ProductID == productID
                               select new ProductSummary
                               {
                                   ProductID = x.ProductID,
                                   ItemName = x.ItemName,
                                   ItemPrice = x.ItemPrice,
                                   Quantity = 0,
                                   ExtendedPrice = 0
                               }); ;

                return results.SingleOrDefault();
            }
        }

        public List<ProductSummary> PurchaseProducts(int invoiceID)
        {
            using (var context = new eRaceContext())
            {
                var results = (from x in context.InvoiceDetails
                               where x.InvoiceDetailID == invoiceID
                               select new ProductSummary
                               {
                                   ProductID = x.Product.ProductID,
                                   ItemName = x.Product.ItemName,
                                   ItemPrice = x.Product.ItemPrice,
                                   Quantity = x.Quantity
                               });

                return results.ToList();
            }
        }

        public List<InvoiceProducts> GetInvoiceProducts(int invoiceID)
        {
            using(var context = new eRaceContext())
            {
                var results = (from x in context.InvoiceDetails
                               where x.InvoiceID == invoiceID
                               select new InvoiceProducts
                               {
                                   InvoiceID = x.InvoiceID,
                                   ProductID = x.ProductID,
                                   ItemName = x.Product.ItemName,
                                   CategoryID = x.Product.CategoryID,
                                   Quantity = x.Quantity,
                                   Price = x.Product.ItemPrice,
                                   Amount = x.Product.ItemPrice * x.Quantity,
                                   RestockCharge = x.Product.ReStockCharge,
                                   RestockChgExists = false,
                                   Reason = ""

                               });
                return results.ToList();
            }
        }

        public List<ProcessedRefunds> GetProcessedRefundsList(int invoiceID)
        {
            using(var context = new eRaceContext())
            {
                var results = (from x in context.StoreRefunds
                               where x.OriginalInvoiceID == invoiceID
                               select new ProcessedRefunds
                               {
                                   RefundInvID = x.InvoiceID,
                                   ProductID = x.ProductID,
                                   ProductName = x.Product.ItemName
                               });
                return results.ToList();
            }
        }

        // ****
        // TRANSACTION / SAVE
        // ****
        public void ProcessPurchase(Purchase purchase)
        {
            using (var context = new eRaceContext())
            {
                Invoice invoice = null;
                ICollection<InvoiceDetail> invoiceDetail = new List<InvoiceDetail>();

                foreach (var item in purchase.Details)
                {
                    var detail = new InvoiceDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    invoiceDetail.Add(detail);

                    Product product = context.Products.Find(item.ProductID);
                    product.QuantityOnHand = product.QuantityOnHand - item.Quantity;
                }

                invoice = new Invoice()
                {
                    InvoiceDate = purchase.PurchaseDate,
                    EmployeeID = purchase.EmployeeID,
                    SubTotal = purchase.SubTotal,
                    GST = purchase.GST,
                    Total = purchase.Total,
                    InvoiceDetails = invoiceDetail
                };

                context.Invoices.Add(invoice);
                context.SaveChanges();  
            }
        }

        public int ProcessRefund(Refund refund)
        {
            int newInvoiceID = 0;
            using (var context = new eRaceContext())
            {
                Invoice refundInvoice = null;
                //StoreRefund storeRefund = null;
                ICollection<StoreRefund> refundDetail = new List<StoreRefund>();

                foreach (var item in refund.Details)
                {
                    var detail = new StoreRefund
                    {
                        OriginalInvoiceID = item.OriginalInvoiceID,
                        ProductID = item.ProductID,
                        Reason = item.Reason
                    };
                    refundDetail.Add(detail);

                    Product product = context.Products.Find(item.ProductID);
                    product.QuantityOnHand = product.QuantityOnHand + item.Quantity;
                }

                refundInvoice = new Invoice
                {
                    InvoiceDate = refund.RefundDate,
                    EmployeeID = refund.EmployeeID,
                    SubTotal = refund.SubTotal,
                    GST = refund.GST,
                    Total = refund.Total,
                    StoreRefunds = refundDetail
                };
                context.Invoices.Add(refundInvoice);
                context.SaveChanges();
                newInvoiceID = refundInvoice.InvoiceID;
            }

            return newInvoiceID;
        }

    }
}
