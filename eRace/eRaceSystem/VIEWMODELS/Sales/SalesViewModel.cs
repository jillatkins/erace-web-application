using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRaceSystem.VIEWMODELS.Sales
{
    public class CategoryList
    {
        public int CategoryID { get; set; }
        public string Description { get; set; }
    }
    
    public class ProductsList
    {
        public int ProductID { get; set; }
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
        public int QtyOnHand { get; set; }
        public decimal ReStockCharge { get; set; }
    }

    public class InvoiceProducts
    {
        public int InvoiceID { get; set; }
        public int ProductID { get; set; }
        public string ItemName { get; set; }
        public int CategoryID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public bool RestockChgExists { get; set; }
        public decimal RestockCharge { get; set; }
        public string Reason { get; set; }
    }

    public class InvoiceDetails
    {
        public int InvoiceID { get; set; }
        public int InvoiceDetailId { get; set; }
        public int ProductID { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductSummary
    {
        public int ProductID { get; set; }
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; }
        public decimal ExtendedPrice { get; set; }
    }

    public class ProcessedRefunds
    {
        public int RefundInvID { get; set; }
        public int OriginalInvoiceID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
    }

    #region Command POCOs
    public class PurchaseDetails
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class Purchase
    {
        public DateTime PurchaseDate { get; set; }
        public int EmployeeID { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GST { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<PurchaseDetails> Details { get; set; }
    }

    public class RefundDetails
    {
        public int OriginalInvoiceID { get; set; }
        public int ProductID { get; set; }
        public string Reason { get; set; }
        public int Quantity { get; set; }
    }

    public class Refund
    {
        public DateTime RefundDate { get; set; }
        public int EmployeeID { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GST { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<RefundDetails> Details { get; set; }
    }

    #endregion

}
