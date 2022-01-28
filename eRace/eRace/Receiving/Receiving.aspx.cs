using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eRaceSystem.DAL;
using eRaceSystem.ENTITIES;
using eRaceSystem.VIEWMODELS.Receiving;
using eRaceSystem.BLL.Receiving;

namespace eRace.Receiving
{
    public partial class Receiving : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void OnClick_GetVendorAndPurchaseOrderDetails(object sender, EventArgs e)
        {
            if (VendorDropDown.SelectedIndex > 0)
            {
                var controller = new ReceivingController();
                var vendorInfo = controller.GetVendorDetails(int.Parse(VendorDropDown.SelectedValue));
                SetVendor(vendorInfo.VendorName, vendorInfo.Address, vendorInfo.ContactName, vendorInfo.Phone);
                List<PurchaseOrderDetails> orderDetails = GetPurchaseOrderDetails();
                PurchaseOrderGridView.DataSource = orderDetails;
                PurchaseOrderGridView.DataBind();
            }


        }
        private void SetVendor(string name, string address, string contact, string phone)
        {
            VendorName.Text = name;
            VendorAddressPlusCity.Text = address;
            VendorContact.Text = contact;
            VendorPhone.Text = phone;
        }
        protected PurchaseOrderDetails GetOrderDetailsFromRow(GridViewRow row)
        {
            PurchaseOrderDetails item = new PurchaseOrderDetails()
            {
                VendorID = row.FindHiddenField("VendorID").Value.ToInt(),
                ItemName = row.FindLabel("Item").Text,
                QtyOrdered = row.FindLabel("OrderQuantity").Text.ToInt(),
                OrderUnitSize = row.FindLabel("OrderedUnits").Text.ToInt(),
                ReceivedItemQuantity = row.FindLabel("ReceivedUnits").Text.ToInt(),
                RejectedUnits = row.FindTextBox("RejectedUnits").Text.ToInt(),
                ReturnReason = row.FindTextBox("Reason").Text,
                SalvagedItems = row.FindTextBox("SalvagedItems").Text.ToInt()

            };
            return item;
        }
        List<PurchaseOrderDetails> GetPurchaseOrderDetails()
        {
            var list = new List<PurchaseOrderDetails>();
            foreach (GridViewRow row in PurchaseOrderGridView.Rows)
            {
                PurchaseOrderDetails item = GetOrderDetailsFromRow(row);
                list.Add(item);
            }
            return list;
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
    }
    #endregion
}
