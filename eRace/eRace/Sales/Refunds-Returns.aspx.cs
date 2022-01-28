using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eRaceSystem.VIEWMODELS.Sales;
using eRaceSystem.BLL.Sales;

namespace eRace.Sales
{
    public partial class Refunds_Returns : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated && User.IsInRole("Clerk"))
            {
                Message.Text = "Login Successful as a Clerk";
            }
            else
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }

        protected void OnClick_Lookup_Invoice(object sender, EventArgs e)
        {
            InvoiceItems.DataBind();
            var controller = new SalesController();
            RefundInvoiceNum.Text = "Refund Invoice #";

            if (OriginalInvoice.Text == "")
            {
                MessageUserControl.ShowInfo("ERROR: Please enter a valid Invoice Number");
            }
            else
            {
                int invoiceID = int.Parse(OriginalInvoice.Text);
                List<InvoiceProducts> invoiceProducts = controller.GetInvoiceProducts(invoiceID);

                if (invoiceProducts.Count() == 0)
                {
                    MessageUserControl.ShowInfo("No records exist for invoice #" + invoiceID);
                }
                else
                {

                    foreach (InvoiceProducts item in invoiceProducts)
                    {
                        item.Price = Math.Round(item.Price, 2);
                        item.Amount = Math.Round(item.Amount, 2);
                        item.RestockCharge = Math.Round(item.RestockCharge, 2);

                        if (item.RestockCharge != 0)
                        {
                            item.RestockChgExists = true;
                        }
                    }

                    InvoiceItems.DataSource = invoiceProducts;
                    InvoiceItems.DataBind();

                    foreach (GridViewRow item in InvoiceItems.Rows)
                    {
                        if (item.FindHiddenField("CategoryId").Value.ToInt() == 3)
                        {
                            item.Enabled = false;
                        }
                    }

                    RefundBtn.Enabled = true;
                    InvoiceItems.Enabled = true;

                }
            }
        }

        protected void OnClick_Clear_Page(object sender, EventArgs e)
        {
            InvoiceItems.DataBind();
            OriginalInvoice.Text = "";
            RefundInvoiceNum.Text = "Refund Invoice #";
            SubTotal.Text = "0.00";
            GST.Text = "0.00";
            RefundTotal.Text = "0.00";
            RefundBtn.CssClass = "btn btn-light btn-lg";
            RefundBtn.Enabled = false;
        }

        protected void OnCheck_Refund(object sender, EventArgs e)
        {
            //GridViewRow row = (sender as CheckBox).NamingContainer as GridViewRow;
            int count = 0;
            decimal subtotal = 0;

            foreach (GridViewRow i in InvoiceItems.Rows)
            {
                if (i.FindCheckBox("Refund").Checked == true)
                {
                    decimal restockCharge = decimal.Parse(i.FindLabel("RestockCharge").Text);
                    restockCharge = restockCharge * decimal.Parse(i.FindLabel("Quantity").Text);
                    subtotal = subtotal + decimal.Parse(i.FindLabel("Amount").Text) - restockCharge;
                    i.FindTextBox("RefundReason").Enabled = true;
                    count++;
                }
                else
                {
                    i.FindTextBox("RefundReason").Enabled = false;
                }
            }

            if (count > 0)
            {
                RefundBtn.CssClass = "btn btn-success btn-lg";
                RefundBtn.Enabled = true;
            }

            else
            {
                RefundBtn.CssClass = "btn btn-light btn-lg";
                RefundBtn.Enabled = false;
            }

            subtotal = Math.Round(subtotal, 2);

            decimal gst = subtotal * (decimal)0.05;
            decimal total = subtotal + gst;

            gst = Math.Round(gst, 2);
            total = Math.Round(total, 2);

            SubTotal.Text = subtotal.ToString();
            GST.Text = gst.ToString();
            RefundTotal.Text = total.ToString();
        }

        protected void InvoiceItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void OnClick_Refund(object sender, EventArgs e)
        {
            bool validationPassed = true;
            string message = "";
            SalesController sysmgr = new SalesController();
            int originalInvoiceID = int.Parse(OriginalInvoice.Text);
            List<ProcessedRefunds> processedRefunds = sysmgr.GetProcessedRefundsList(originalInvoiceID);

            foreach (GridViewRow row in InvoiceItems.Rows)
            {
                if (row.FindCheckBox("Refund").Checked == true && row.FindTextBox("RefundReason").Text == "")
                { 
                    validationPassed = false;
                    message = "ERROR: Refund Reason field MUST be entered for each item being refunded.";
                }

                foreach (ProcessedRefunds item in processedRefunds)
                {
                    if (row.FindCheckBox("Refund").Checked == true && row.FindHiddenField("ProductId").Value.ToInt() == item.ProductID)
                    {
                        validationPassed = false;
                        message = "ERROR: The product " + item.ProductName + " has already been refunded on this Invoice #" + 
                            originalInvoiceID + " (Refund Invoice #" + item.RefundInvID + ")";
                    }
                }

            }

            if (validationPassed == false)
            {
                MessageUserControl.ShowInfo(message);
            }

            else
            {
                int employeeID = 49;
                DateTime currentDate = DateTime.Today;

                decimal subtotal = 0;

                List<RefundDetails> refundItems = new List<RefundDetails>();

                foreach (GridViewRow i in InvoiceItems.Rows)
                {
                    if (i.FindCheckBox("Refund").Checked == true)
                    {
                        decimal restockCharge = decimal.Parse(i.FindLabel("RestockCharge").Text);
                        restockCharge = restockCharge * decimal.Parse(i.FindLabel("Quantity").Text);
                        subtotal = subtotal - decimal.Parse(i.FindLabel("Amount").Text) + restockCharge;

                        var item = new RefundDetails
                        {
                            OriginalInvoiceID = int.Parse(OriginalInvoice.Text),
                            ProductID = i.FindHiddenField("ProductId").Value.ToInt(),
                            Reason = i.FindTextBox("RefundReason").Text,
                            Quantity = i.FindLabel("Quantity").Text.ToInt()
                        };
                        refundItems.Add(item);
                    }
                }

                subtotal = Math.Round(subtotal, 2);

                decimal gst = subtotal * (decimal)0.05;
                decimal total = subtotal + gst;

                gst = Math.Round(gst, 2);
                total = Math.Round(total, 2);

                MessageUserControl.TryRun(() =>
                {
                    Refund refund = new Refund();
                    refund.RefundDate = currentDate;
                    refund.EmployeeID = employeeID;
                    refund.SubTotal = subtotal;
                    refund.GST = gst;
                    refund.Total = total;
                    refund.Details = refundItems;


                    int newInvoiceId = sysmgr.ProcessRefund(refund);

                    RefundInvoiceNum.Text = newInvoiceId.ToString();
                    RefundBtn.CssClass = "btn btn-light btn-lg";
                    RefundBtn.Enabled = false;
                    InvoiceItems.Enabled = false;

                }, "", "SUCCESS: Refund has been processed.");
            }
        }
    }
}