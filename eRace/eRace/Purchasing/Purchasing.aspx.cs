using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using eRaceSystem.BLL.Purchasing;
using eRaceSystem.VIEWMODELS.Purchasing;

namespace eRace.Purchasing
{
    public partial class Purchasing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated && (User.IsInRole("Director") || User.IsInRole("OfficeManager")))
            {

            }
            else
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }

        protected void UpdateOrderTotals()
        {
            List<VendorProductDetail> orderDetailList = GetOrderDetailsList();
            // Don't see a smart way from the DB to tell items that need tax, so just setting the tax as a flat constant of the subtotal.
            const decimal taxRate = 0.05M;
            decimal subTotal = 0;
            decimal tax = 0;
            decimal total = 0;

            foreach (VendorProductDetail product in orderDetailList)
            {
                subTotal += (decimal)product.POCost * (decimal)product.POQuantity;
            }
            tax = subTotal * taxRate;
            total = subTotal + tax;

            SubtotalTextBox.Text = subTotal.ToString();
            TaxTextBox.Text = tax.ToString();
            TotalTextBox.Text = total.ToString();
        }

        protected void UpdateGridRowCosts(GridViewRow row)
        {
            decimal newPerItemCost = row.FindTextBox("POCost").Text.ToDecimal() / row.FindLabel("OrderUnitSize").Text.ToDecimal();
            row.FindLabel("PerItemCost").Text = newPerItemCost.ToString();

            decimal itemSalePrice = row.FindHiddenField("ItemSalePrice").Value.ToDecimal();

            if (itemSalePrice < newPerItemCost)
                row.FindLabel("PerItemCostWarn").Text = "!!!";
            else
                row.FindLabel("PerItemCostWarn").Text = "";

            decimal newExtendedCost = row.FindTextBox("POCost").Text.ToDecimal() * row.FindTextBox("POQuantity").Text.ToDecimal();
            row.FindLabel("ExtendedCost").Text = newExtendedCost.ToString();

            UpdateOrderTotals();
        }

        protected void ClearPage()
        {
            // Reset the page
            VendorDDL.SelectedIndex = 0;
            VendorDDL.Enabled = true;
            SelectVendor.Enabled = true;

            PlaceOrder.Enabled = false;
            SaveOrder.Enabled = false;
            DeleteOrder.Enabled = false;
            CancelOrder.Enabled = false;

            VendorCategoryRepeater.DataSource = null;
            VendorCategoryRepeater.DataBind();

            OrderID.Value = "";
            VendorNameLabel.Text = "Vendor Name";
            VendorContactLabel.Text = "Contact";
            VendorPhoneLabel.Text = "Phone";
            CommentsTextBox.Text = "Comments";
            SubtotalTextBox.Text = "";
            TaxTextBox.Text = "";
            TotalTextBox.Text = "";

            OrderDetailsGridView.DataSource = null;
            OrderDetailsGridView.DataBind();
        }

        #region Form Buttons
        protected void SelectVendor_Click(object sender, EventArgs e)
        {
            int vendorid = VendorDDL.SelectedValue.ToInt();
            if (vendorid == 0)
                MessageUserControl.ShowInfo("", "Please select a vendor...");
            else
            {
                // Disable VendorDDL and Select so the user can't change vendor until they hit cancel
                VendorDDL.Enabled = false;
                SelectVendor.Enabled = false;
                PlaceOrder.Enabled = true;
                SaveOrder.Enabled = true;
                DeleteOrder.Enabled = true;
                CancelOrder.Enabled = true;

                var controller = new PurchasingController();
                // Fill out Vendor Categories Gridview
                var vendorcategories = controller.VendorCategoryProducts_List(vendorid);
                VendorCategoryRepeater.DataSource = vendorcategories;
                VendorCategoryRepeater.DataBind();

                // If existing PO fill out Order Details and Vendor Info
                var existingPurchaseOrder = controller.PurchaseOrder_Find(vendorid);

                if (existingPurchaseOrder != null)
                {
                    OrderID.Value = existingPurchaseOrder.OrderID.ToString();
                    VendorNameLabel.Text = existingPurchaseOrder.Name;
                    VendorContactLabel.Text = existingPurchaseOrder.Contact;
                    VendorPhoneLabel.Text = existingPurchaseOrder.Phone;
                    CommentsTextBox.Text = existingPurchaseOrder.Comment;
                    SubtotalTextBox.Text = existingPurchaseOrder.Subtotal.ToString();
                    TaxTextBox.Text = existingPurchaseOrder.TaxGST.ToString();
                    TotalTextBox.Text = (existingPurchaseOrder.Subtotal + existingPurchaseOrder.TaxGST).ToString();

                    var orderDetailItems = controller.OrderDetailProducts_List(vendorid, existingPurchaseOrder.OrderID);
                    OrderDetailsGridView.DataSource = orderDetailItems;
                    OrderDetailsGridView.DataBind();

                    // Warn the user if any of the loaded costs are greater than the items sale price
                    foreach (GridViewRow row in OrderDetailsGridView.Rows)
                    {
                        UpdateGridRowCosts(row);
                    }
                }
                else // Else lookup and fill out Vendor Info
                {
                    var vendorinfo = controller.Vendor_Get(vendorid); // Should prob check this doesnt return null
                    OrderID.Value = "0";
                    VendorNameLabel.Text = vendorinfo.Name;
                    VendorContactLabel.Text = vendorinfo.Contact;
                    VendorPhoneLabel.Text = vendorinfo.Phone;
                    CommentsTextBox.Text = "";

                    OrderDetailsGridView.DataSource = null;
                    OrderDetailsGridView.DataBind();
                }
            }
        }

        protected void PlaceOrder_Click(object sender, EventArgs e)
        {
            int vendorid = VendorDDL.SelectedValue.ToInt();
            if (vendorid == 0)
                MessageUserControl.ShowInfo("", "Please select a vendor...");
            else
            {
                if (OrderDetailsGridView.Rows.Count == 0)
                {
                    MessageUserControl.ShowInfo("", "Please add products to your order...");
                }
                else
                {
                    MessageUserControl.TryRun(() =>
                    {
                        int returnedPK = 0;
                        int badQuantityCount = 0;
                        // Update totals so we don't get any old information
                        foreach (GridViewRow row in OrderDetailsGridView.Rows)
                        {
                            UpdateGridRowCosts(row);

                            // Check the POQuantity int is more than 0, and POCost is 0 or greater.
                            if ((row.FindTextBox("POQuantity").Text.ToInt() <= 0) || (row.FindTextBox("POCost").Text.ToDecimal() < 0))
                            {
                                badQuantityCount++;
                            }
                        }

                        if (badQuantityCount > 0)
                        {
                            throw new Exception($"{badQuantityCount} of your items have invalid quantities or costs, quantities must be more than 0, and costs cannot be negative...");
                        }
                        else
                        {
                            PurchaseOrder purchaseOrder = new PurchaseOrder()
                            {
                                VendorID = VendorDDL.SelectedValue.ToInt(),
                                Name = VendorNameLabel.Text,
                                Phone = VendorPhoneLabel.Text,
                                Contact = VendorContactLabel.Text,

                                OrderID = OrderID.Value.ToInt(), // Should be 0 if the order is new
                                OrderNumber = null, // Set these in the BLL
                                OrderDate = null,
                                Subtotal = SubtotalTextBox.Text.ToDecimal(),
                                TaxGST = TaxTextBox.Text.ToDecimal(),
                                Closed = false,
                                Comment = CommentsTextBox.Text
                            };

                            List<VendorProductDetail> orderDetailList = GetOrderDetailsList();

                            string userName = User.Identity.Name;

                            var controller = new PurchasingController();
                            returnedPK = controller.PurchaseOrder_Place(purchaseOrder, orderDetailList, userName);
                            ClearPage();
                        }
                    }, "", "Success: Purchase Order has been Placed.");
                }
            }
        }

        protected void SaveOrder_Click(object sender, EventArgs e)
        {
            int vendorid = VendorDDL.SelectedValue.ToInt();
            if (vendorid == 0)
                MessageUserControl.ShowInfo("", "Please select a vendor...");
            else
            {
                if (OrderDetailsGridView.Rows.Count == 0)
                {
                    MessageUserControl.ShowInfo("", "Please add products to your order...");
                }
                else
                {
                    MessageUserControl.TryRun(() =>
                    {
                        int returnedPK = 0;
                        int badQuantityCount = 0;
                        // Update totals so we don't get any old information
                        foreach (GridViewRow row in OrderDetailsGridView.Rows)
                        {
                            UpdateGridRowCosts(row);

                            // Quantity cant be 0 or negative, cost cant be negative
                            if ((row.FindTextBox("POQuantity").Text.ToInt() <= 0) || (row.FindTextBox("POCost").Text.ToDecimal() < 0))
                            {
                                badQuantityCount++;
                            }
                        }

                        if (badQuantityCount > 0)
                        {
                            throw new Exception($"{badQuantityCount} of your items have invalid quantities or costs, quantities must be more than 0, and costs cannot be negative...");
                        }
                        else
                        {
                            PurchaseOrder purchaseOrder = new PurchaseOrder()
                            {
                                VendorID = VendorDDL.SelectedValue.ToInt(),
                                Name = VendorNameLabel.Text,
                                Phone = VendorPhoneLabel.Text,
                                Contact = VendorContactLabel.Text,

                                OrderID = OrderID.Value.ToInt(), // Should be 0 if the order is new
                                OrderNumber = null,
                                OrderDate = null,
                                Subtotal = SubtotalTextBox.Text.ToDecimal(),
                                TaxGST = TaxTextBox.Text.ToDecimal(),
                                Closed = false,
                                Comment = CommentsTextBox.Text
                            };

                            List<VendorProductDetail> orderDetailList = GetOrderDetailsList();

                            string userName = User.Identity.Name;

                            var controller = new PurchasingController();
                            returnedPK = controller.PurchaseOrder_Save(purchaseOrder, orderDetailList, userName);
                            OrderID.Value = returnedPK.ToString();
                        }
                    }, "", "Success: Purchase Order has been Saved.");
                }
            }
        }

        protected void DeleteOrder_Click(object sender, EventArgs e)
        {
            int vendorid = VendorDDL.SelectedValue.ToInt();
            if (vendorid == 0)
                MessageUserControl.ShowInfo("", "Please select a vendor...");
            else
            {
                if (OrderID.Value == "0")
                    MessageUserControl.ShowInfo("", "Cannot delete an order that hasn't been saved...");
                else
                {
                    MessageUserControl.TryRun(() =>
                    {
                        int orderID = OrderID.Value.ToInt();
                        var controller = new PurchasingController();
                        controller.PurchaseOrder_Delete(orderID);

                        ClearPage();
                    }, "", "Success: Purchase Order has been Deleted.");
                }
            } 
        }

        protected void CancelOrder_Click(object sender, EventArgs e)
        {
            ClearPage();
            MessageUserControl.ShowInfo("", "Purchase Order Window Cleared.");
        }
        #endregion Form Buttons

        #region GridView Buttons
        protected void OrderDetailsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = OrderDetailsGridView.Rows[index];
            if (e.CommandName == "Remove")
            {
                List<VendorProductDetail> orderDetailList = GetOrderDetailsList();
                VendorProductDetail removeitem = GetVendorProductFromOrderDetailRow(row);

                orderDetailList.RemoveAll(x => x.ProductID == removeitem.ProductID);

                OrderDetailsGridView.DataSource = orderDetailList;
                OrderDetailsGridView.DataBind();

                UpdateOrderTotals();
                MessageUserControl.ShowInfo("", "Item removed from order details grid...");
            }
            else if (e.CommandName == "Refresh")
            {
                MessageUserControl.TryRun(() =>
                {
                    UpdateGridRowCosts(row);
                }, "", "Row refreshed successfully.");
            }
            else
                MessageUserControl.ShowInfo("", "An error occurred with your selection...");
        }

        protected void VendorCategoryProductGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Add")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridView gridview = (GridView)sender; // TY stack overflow https://stackoverflow.com/questions/41133971/get-the-selected-row-of-a-gridview-thats-nested-in-a-repeater
                GridViewRow row = gridview.Rows[index];

                VendorProductDetail product = GetVendorProductFromCategoryRow(row);

                List<VendorProductDetail> orderDetailList = GetOrderDetailsList();

                if (orderDetailList.Exists(x => x.ProductID == product.ProductID))
                {
                    MessageUserControl.ShowInfo("", "Product already exists on the purchase order, added 1 to the selected item.");
                    VendorProductDetail tempItem = orderDetailList.Find(x => x.ProductID == product.ProductID);
                    orderDetailList.Remove(tempItem);
                    tempItem.POQuantity += 1;
                    orderDetailList.Add(tempItem);
                }
                else
                {
                    MessageUserControl.ShowInfo("", "Product added to the purchase order.");
                    orderDetailList.Add(product);
                }
                OrderDetailsGridView.DataSource = orderDetailList;
                OrderDetailsGridView.DataBind();

                UpdateOrderTotals();
            }
            else
                MessageUserControl.ShowInfo("", "An error occurred with your selection...");
        }
        #endregion GridView Buttons

        #region Build VendorProductDetail from GridViewRow
        protected VendorProductDetail GetVendorProductFromCategoryRow(GridViewRow row)
        {
            VendorProductDetail item = new VendorProductDetail()
            {
                ProductID = row.FindHiddenField("ProductID").Value.ToInt(),
                ItemName = row.FindLabel("ItemName").Text,
                ItemSalePrice = row.FindHiddenField("ItemSalePrice").Value.ToDecimal(),
                QuantityOnHand = row.FindLabel("QuantityOnHand").Text.ToInt(),
                QuantityOnOrder = row.FindLabel("QuantityOnOrder").Text.ToInt(),
                ReOrderLevel = row.FindLabel("ReOrderLevel").Text.ToInt(),

                OrderDetailID = 0,
                POQuantity = 1,
                POCost = row.FindHiddenField("OrderUnitCost").Value.ToDecimal(),

                OrderUnitType = row.FindLabel("OrderUnitType").Text,
                OrderUnitSize = row.FindLabel("OrderUnitSize").Text.ToInt(),
                OrderUnitCost = row.FindHiddenField("OrderUnitCost").Value.ToDecimal()
            };
            return item;
        }

        protected VendorProductDetail GetVendorProductFromOrderDetailRow(GridViewRow row)
        {
            VendorProductDetail item = new VendorProductDetail()
            { // Cant just use the GetVendorProductFromCategoryRow method since they have values in different control types.
                ProductID = row.FindHiddenField("ProductID").Value.ToInt(),
                ItemName = row.FindLabel("ItemName").Text,
                ItemSalePrice = row.FindHiddenField("ItemSalePrice").Value.ToDecimal(),
                QuantityOnHand = row.FindHiddenField("QuantityOnHand").Value.ToInt(),
                QuantityOnOrder = row.FindHiddenField("QuantityOnOrder").Value.ToInt(),
                ReOrderLevel = row.FindHiddenField("ReOrderLevel").Value.ToInt(),

                OrderDetailID = row.FindHiddenField("OrderDetailID").Value.ToInt(),
                POQuantity = row.FindTextBox("POQuantity").Text.ToInt(),
                POCost = row.FindTextBox("POCost").Text.ToDecimal(),

                OrderUnitType = row.FindLabel("OrderUnitType").Text,
                OrderUnitSize = row.FindLabel("OrderUnitSize").Text.ToInt(),
                OrderUnitCost = row.FindHiddenField("OrderUnitCost").Value.ToDecimal()
            };
            return item;
        }

        protected List<VendorProductDetail> GetOrderDetailsList()
        {
            var list = new List<VendorProductDetail>();
            foreach (GridViewRow row in OrderDetailsGridView.Rows)
            {
                VendorProductDetail item = GetVendorProductFromOrderDetailRow(row);
                list.Add(item);
            }
            return list;
        }
        #endregion Build VendorProductDetail from GridViewRow

    }
    #region Web Extensions
    public static class WebControlExtensions
    {
        public static Label FindLabel(this Control self, string id)
            => self.FindControl(id) as Label;
        public static TextBox FindTextBox(this Control self, string id)
            => self.FindControl(id) as TextBox;
        public static HiddenField FindHiddenField(this Control self, string id)
            => self.FindControl(id) as HiddenField;
        public static CheckBox FindCheckBox(this Control self, string id)
            => self.FindControl(id) as CheckBox;
        public static int ToInt(this string self) => int.Parse(self);
        public static decimal ToDecimal(this string self) => decimal.Parse(self);
    }
    #endregion
}