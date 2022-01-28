<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Purchasing.aspx.cs" Inherits="eRace.Purchasing.Purchasing" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div style="margin-left:-4rem; margin-right:-4rem;">

    
    <h1>Purchasing</h1>
    <div class="row">
        <uc1:MessageUserControl runat="server" ID="MessageUserControl" />
    </div>
    
    <div class="row">
        <div class="col-6">
            <h3>Purchase Order</h3>
            <div class="row">
                <!-- Vender DDL and Page Controls -->
                <div class="row">
                    <asp:Label ID="VendorLabel" runat="server" Text="Vendor:"></asp:Label>&nbsp;
                    <asp:DropDownList ID="VendorDDL" runat="server" DataSourceID="VendorODS" DataTextField="Description" DataValueField="ID" AppendDataBoundItems="true">
                        <asp:ListItem Value="0">Select...</asp:ListItem>
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="VendorODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="Vendor_List" TypeName="eRaceSystem.BLL.Purchasing.PurchasingController"></asp:ObjectDataSource>
                </div>
                
                <div class="row" style="padding-top: 1rem;">
                    <asp:Button ID="SelectVendor" runat="server" Text="Select" CssClass="btn btn-secondary" OnClick="SelectVendor_Click"/>&nbsp;
                    <asp:Button ID="PlaceOrder" runat="server" Text="Place Order" CssClass="btn btn-primary" Enabled="false" OnClick="PlaceOrder_Click" />&nbsp;
                    <asp:Button ID="SaveOrder" runat="server" Text="Save" CssClass="btn btn-success" Enabled="false" OnClick="SaveOrder_Click" />&nbsp;
                    <asp:Button ID="DeleteOrder" runat="server" Text="Delete" CssClass="btn btn-danger" Enabled="false" OnClick="DeleteOrder_Click" />&nbsp;
                    <asp:Button ID="CancelOrder" runat="server" Text="Cancel" CssClass="btn btn-secondary" Enabled="false" OnClick="CancelOrder_Click" />
                </div>
                
            </div>
            <br />
            <div class="row">
                <!-- Vendor information and Order Costs -->
                <div class="row">
                    <asp:HiddenField ID="OrderID" runat="server" Value="0" />
                    <asp:Label ID="VendorNameLabel" runat="server" Text="Vendor Name"></asp:Label>&nbsp;-&nbsp;
                    <asp:Label ID="VendorContactLabel" runat="server" Text="Contact"></asp:Label>&nbsp;-&nbsp;
                    <asp:Label ID="VendorPhoneLabel" runat="server" Text="Phone"></asp:Label>
                </div>
                <div class="row">
                    <asp:TextBox ID="CommentsTextBox" runat="server" Text="Comments" TextMode="MultiLine" Width="50%" ></asp:TextBox>
                </div>
                <div class="row" style="padding-top: 1rem;">
                    <asp:Label ID="SubtotalLabel" runat="server" Text="Subtotal"></asp:Label>&nbsp;
                    <asp:TextBox ID="SubtotalTextBox" runat="server" Width="12%" Enabled="false"></asp:TextBox>&nbsp;&nbsp;&nbsp;
                    <asp:Label ID="TaxLabel" runat="server" Text="Tax"></asp:Label>&nbsp;
                    <asp:TextBox ID="TaxTextBox" runat="server" Width="12%" Enabled="false"></asp:TextBox>&nbsp;&nbsp;&nbsp;
                    <asp:Label ID="TotalLabel" runat="server" Text="Total"></asp:Label>&nbsp;
                    <asp:TextBox ID="TotalTextBox" runat="server" Width="12%" Enabled="false"></asp:TextBox>&nbsp;&nbsp;&nbsp;
                </div>
            </div>
            <br />
            <div class="row">
                <!-- Order Details GridView -->
                <asp:GridView ID="OrderDetailsGridView" runat="server" ItemType="eRaceSystem.VIEWMODELS.Purchasing.VendorProductDetail" DataKeyNames="ProductID"
                    AutoGenerateColumns="false" OnRowCommand="OrderDetailsGridView_RowCommand" GridLines="Horizontal" CssClass="table table-hover table-striped table-sm">
                    <EmptyDataTemplate><i>No Order Items Added...</i></EmptyDataTemplate>
                    <Columns>
                        <asp:ButtonField CommandName="Remove" Text="X" ></asp:ButtonField>
                        <asp:TemplateField HeaderText="Product" >
                            <ItemTemplate>
                                <asp:Label ID="ItemName" runat="server" Text="<%# Item.ItemName %>" ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Order Qty" HeaderStyle-Width="14%">
                            <ItemTemplate>
                                <asp:TextBox ID="POQuantity" runat="server" Text="<%# Item.POQuantity %>"
                                CssClass="form-control form-control-sm" TextMode="Number" Width="70%" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Unit Size">
                            <ItemTemplate>
                                <asp:Label ID="OrderUnitSize" runat="server" Text="<%# Item.OrderUnitSize %>" ></asp:Label>&nbsp;
                                <asp:Label ID="OrderUnitType" runat="server" Text="<%# Item.OrderUnitType %>" ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Unit Cost ($)" HeaderStyle-Width="16%" >
                            <ItemTemplate>
                                <asp:TextBox ID="POCost" runat="server" Text="<%# Item.POCost %>"
                                CssClass="form-control form-control-sm" TextMode="Number" Width="90%" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField CommandName="Refresh" Text="R"></asp:ButtonField>
                        <asp:TemplateField HeaderText="Per-Item Cost ($)">
                            <ItemTemplate>
                                <asp:Label ID="PerItemCostWarn" runat="server" Text="" ></asp:Label>
                                <asp:Label ID="PerItemCost" runat="server" Text="<%# Item.POCost / Item.OrderUnitSize %>" ></asp:Label>
                                <asp:HiddenField ID="OrderUnitCost" runat="server" Value="<%# Item.OrderUnitCost %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Extended Cost ($)">
                            <ItemTemplate>
                                <asp:Label ID="ExtendedCost" runat="server" Text="<%# Item.POCost * Item.POQuantity %>" ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="ProductID" runat="server" Value="<%# Item.ProductID %>" />
                                <asp:HiddenField ID="ItemSalePrice" runat="server" Value="<%# Item.ItemSalePrice %>" />
                                <asp:HiddenField ID="QuantityOnHand" runat="server" Value="<%# Item.QuantityOnHand %>" />
                                <asp:HiddenField ID="QuantityOnOrder" runat="server" Value="<%# Item.QuantityOnOrder %>" />
                                <asp:HiddenField ID="ReOrderLevel" runat="server" Value="<%# Item.ReOrderLevel %>" />
                                <asp:HiddenField ID="OrderDetailID" runat="server" Value="<%# Item.OrderDetailID %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div class="col-6">
            <h3>Inventory</h3>
            <!-- Vendor Category Repeater and associated Product Gridviews -->
            <asp:Repeater ID="VendorCategoryRepeater" runat="server" ItemType="eRaceSystem.VIEWMODELS.Purchasing.VendorCategory">
                <ItemTemplate>
                    <h5><%# Item.CategoryDescription %></h5>

                    <asp:GridView ID="VendorCategoryProductGridView" runat="server" DataSource="<%# Item.Products %>" DataKeyNames="ProductID"
                        ItemType="eRaceSystem.VIEWMODELS.Purchasing.VendorProductDetail" OnRowCommand="VendorCategoryProductGridView_RowCommand"
                        CssClass="table table-hover table-striped table-sm" BorderStyle="None" GridLines="Horizontal" AutoGenerateColumns="false">
                        <Columns>
                            <asp:ButtonField CommandName="Add" Text="+"></asp:ButtonField>
                            <asp:TemplateField HeaderText="Product">
                                <ItemTemplate>
                                    <asp:Label ID="ItemName" runat="server" Text="<%# Item.ItemName %>" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Reorder">
                                <ItemTemplate>
                                    <asp:Label ID="ReOrderLevel" runat="server" Text="<%# Item.ReOrderLevel %>" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="In Stock">
                                <ItemTemplate>
                                    <asp:Label ID="QuantityOnHand" runat="server" Text="<%# Item.QuantityOnHand %>" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="On Order">
                                <ItemTemplate>
                                    <asp:Label ID="QuantityOnOrder" runat="server" Text="<%# Item.QuantityOnOrder %>" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Size">
                                <ItemTemplate>
                                    <asp:Label ID="OrderUnitType" runat="server" Text="<%# Item.OrderUnitType %>" ></asp:Label>&nbsp;(
                                    <asp:Label ID="OrderUnitSize" runat="server" Text="<%# Item.OrderUnitSize %>" ></asp:Label>&nbsp;)
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="ProductID" runat="server" Value="<%# Item.ProductID %>" />
                                <asp:HiddenField ID="ItemSalePrice" runat="server" Value="<%# Item.ItemSalePrice %>" />
                                <asp:HiddenField ID="OrderDetailID" runat="server" Value="<%# Item.OrderDetailID %>" />
                                <asp:HiddenField ID="POQuantity" runat="server" Value="<%# Item.POQuantity %>" />
                                <asp:HiddenField ID="POCost" runat="server" Value="<%# Item.POCost %>" />
                                <asp:HiddenField ID="OrderUnitCost" runat="server" Value="<%# Item.OrderUnitCost %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ItemTemplate>
                <SeparatorTemplate>
                    <hr style="height:3px;" />
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>
</asp:Content>
