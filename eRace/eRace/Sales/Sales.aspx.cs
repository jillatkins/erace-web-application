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
    public partial class Sales : System.Web.UI.Page
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

        protected void CategoryDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            var controller = new SalesController();

            // Create a empty Invoice/Product List for the GridView...

            if (CategoryDropDownList.SelectedIndex > 0)
            {
                ProductDropDownList.DataBind();
                ProductDropDownList.Items.Insert(0, new ListItem("Select a Product..."));
                AddProductBtn.CssClass = "btn btn-light";
            }

            else
            {
                AddProductBtn.Enabled = false;
                AddProductBtn.CssClass = "btn btn-light";
            }
        }

        protected void ProductDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {

            // Enable the Add Product button if a Product has been selected
            // Auto populate the quantity field to 1
            if(ProductDropDownList.SelectedIndex > 0)
            {
                ProductQuantity.Text = "1";
                AddProductBtn.Enabled = true;
                AddProductBtn.CssClass = "btn btn-info";
            }
            
            else
            {
                AddProductBtn.Enabled = false;
                AddProductBtn.CssClass = "btn btn-light";
            }
                
        }

        protected void OnClick_Add_Product(object sender, EventArgs e)
        {
            int productID = int.Parse(ProductDropDownList.SelectedValue);
            int index = 0;
            int rowIndex = 0;
            ProductSummary dupItem = null;
            var controller = new SalesController();
            List<ProductSummary> productList = GetListOfProductsFromGV();
            ProductSummary product = controller.GetProducts(productID);


            if (ProductQuantity.Text.ToInt() < 1)
            {
                MessageUserControl.ShowInfo("Quantity must be greater than 0.");
            }

            else
            {
                ProductSummary item = new ProductSummary
                {
                    ProductID = productID,
                    ItemName = product.ItemName,
                    ItemPrice = Math.Round(product.ItemPrice, 2),
                    Quantity = ProductQuantity.Text.ToInt(),
                    ExtendedPrice = Math.Round(product.ItemPrice * ProductQuantity.Text.ToInt(), 2)
                };

                foreach (ProductSummary prod in productList)
                {
                    if (prod.ProductID == item.ProductID)
                    {
                        item.Quantity = prod.Quantity + item.Quantity;
                        item.ExtendedPrice = item.Quantity * item.ItemPrice;
                        dupItem = prod;
                        rowIndex = index;
                    }
                    index++;
                }

                productList.Remove(dupItem);
                productList.Insert(rowIndex, item);

                ProductsPurchase.DataSource = productList;
                ProductsPurchase.DataBind();

                UpdateOrderTotals();

                if (productList.Count() > 0)
                {
                    PaymentBtn.CssClass = "btn btn-success btn-lg";
                    PaymentBtn.Enabled = true;
                }
            }   
        }

        List<ProductSummary> GetListOfProductsFromGV()
        {
            var list = new List<ProductSummary>();
            foreach(GridViewRow row in ProductsPurchase.Rows)
            {
                var item = new ProductSummary
                {
                    ProductID = row.FindHiddenField("ProductID").Value.ToInt(),
                    ItemName = row.FindLabel("Description").Text,
                    ItemPrice = decimal.Parse(row.FindLabel("Price").Text),
                    Quantity = row.FindTextBox("Quantity").Text.ToInt(),
                    ExtendedPrice = decimal.Parse(row.FindLabel("Amount").Text)
                };
                list.Add(item);
            }

            return list;
        }

        protected void ProductsPurchase_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            GridViewRow agvrow = ProductsPurchase.Rows[rowIndex];
            int index = 0;
            int rmvindex = 0;
            List<ProductSummary> productList = GetListOfProductsFromGV();

            ProductSummary product = GetProductFromGridView(agvrow);

            if (e.CommandName == "Remove")
            {
                foreach (ProductSummary item in productList)
                {
                    if (item.ProductID == product.ProductID)
                    {
                        rmvindex = index;
                    }

                    index++;
                }
            }

            productList.RemoveAt(rmvindex);
            ProductsPurchase.DataSource = productList;
            ProductsPurchase.DataBind();
            UpdateOrderTotals();

            if (productList.Count() == 0)
            {
                PaymentBtn.CssClass = "btn btn-light btn-lg";
                PaymentBtn.Enabled = false;
            }
        }

        protected void On_Qty_Update(object sender, EventArgs e)
        {
            GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
            ProductSummary product = GetProductFromGridView(row);
            ProductSummary dupe = null;
            int rowIndex = row.RowIndex;
            List<ProductSummary> productList = GetListOfProductsFromGV();

            if (row.FindTextBox("Quantity").Text.ToInt() < 1)
            {
                MessageUserControl.ShowInfo("Quantity must be greater than 0.");
            }
            else
            {
                foreach (ProductSummary prod in productList)
                {
                    if (prod.ProductID == product.ProductID)
                    {
                        dupe = prod;
                    }
                }

                productList.Remove(dupe);

                product.Quantity = row.FindTextBox("Quantity").Text.ToInt();
                product.ExtendedPrice = product.ItemPrice * product.Quantity;

                productList.Insert(rowIndex, product);

                ProductsPurchase.DataSource = productList;
                ProductsPurchase.DataBind();

                UpdateOrderTotals();
            }
        }

        protected void UpdateOrderTotals()
        {
            decimal subtotal = 0;

            foreach (GridViewRow i in ProductsPurchase.Rows)
            {
                subtotal = subtotal + decimal.Parse(i.FindLabel("Amount").Text);
            }
            subtotal = Math.Round(subtotal, 2);

            decimal gst = subtotal * (decimal)0.05;
            decimal total = subtotal + gst;

            gst = Math.Round(gst, 2);
            total = Math.Round(total, 2);

            SubTotal.Text = subtotal.ToString();
            GST.Text = gst.ToString();
            Total.Text = total.ToString();
        }

        protected void OnClick_ClearCart(object sender, EventArgs e)
        {
            ProductsPurchase.DataBind();
            CategoryDropDownList.SelectedIndex = 0;
            ProductQuantity.Text = "";
            SubTotal.Text = "0.00";
            GST.Text = "0.00";
            Total.Text = "0.00";
            PaymentBtn.CssClass = "btn btn-light btn-lg";
            PaymentBtn.Enabled = false;
        }

        ProductSummary GetProductFromGridView(GridViewRow row)
        {
            var item = new ProductSummary
            {
                ProductID = row.FindHiddenField("ProductID").Value.ToInt(),
                ItemName = row.FindLabel("Description").Text,
                ItemPrice = decimal.Parse(row.FindLabel("Price").Text),
                Quantity = row.FindTextBox("Quantity").Text.ToInt(),
                ExtendedPrice = decimal.Parse(row.FindLabel("Amount").Text)
            };

            return item;
        }

        protected void OnClick_Purchase(object sender, EventArgs e)
        {
            bool valid = true;
            string errorMessage = "";
            
            // Hardcode ID for CLERK
            int employeeID = 49;
            DateTime currentDate = DateTime.Today;

            decimal subtotal = 0;

            foreach (GridViewRow i in ProductsPurchase.Rows)
            {
                subtotal = subtotal + decimal.Parse(i.FindLabel("Amount").Text);
            }
            subtotal = Math.Round(subtotal, 2);

            decimal gst = subtotal * (decimal)0.05;
            decimal total = subtotal + gst;

            gst = Math.Round(gst, 2);
            total = Math.Round(total, 2);

            List<ProductSummary> productsPurchase = GetListOfProductsFromGV();
            List<PurchaseDetails> purchaseDetail = new List<PurchaseDetails>();

            if (productsPurchase.Count() == 0)
            {
                valid = false;
                errorMessage = "Must add at least 1 product to purchase.";
            }

            else
            {
                foreach (var item in productsPurchase)
                {
                    if (item.Quantity == 0)
                    {
                        valid = false;
                        errorMessage = "Item quantities must be greater than 0.";
                    }

                    var detail = new PurchaseDetails
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        Price = item.ItemPrice
                    };
                    purchaseDetail.Add(detail);
                }
            }

            if (valid == false)
            {
                MessageUserControl.ShowInfo("ERROR: " + errorMessage);
            }

            else
            {
                MessageUserControl.TryRun(() =>
                {
                    Purchase purchase = new Purchase();
                    purchase.PurchaseDate = currentDate;
                    purchase.EmployeeID = employeeID;
                    purchase.SubTotal = subtotal;
                    purchase.GST = gst;
                    purchase.Total = total;
                    purchase.Details = purchaseDetail;

                    SalesController sysmgr = new SalesController();
                    sysmgr.ProcessPurchase(purchase);

                    // clear the page
                    ProductsPurchase.DataBind();
                    CategoryDropDownList.SelectedIndex = 0;
                    ProductQuantity.Text = "";
                    SubTotal.Text = "0.00";
                    GST.Text = "0.00";
                    Total.Text = "0.00";
                    PaymentBtn.CssClass = "btn btn-light btn-lg";
                    PaymentBtn.Enabled = false;

                }, "", "SUCCESS: Purchase has been processed!");
            }  

        }

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
        public static bool ToBool(this string self) => bool.Parse(self);
    }
    #endregion

}