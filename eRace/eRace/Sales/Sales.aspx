<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Sales.aspx.cs" Inherits="eRace.Sales.Sales" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jimbotron">
        <h1>In-Store Purchases</h1>
        <asp:Label ID="Message" runat="server" />
        <uc1:MessageUserControl runat="server" ID="MessageUserControl" />
        <br /> <br />
    </div>

    <div class="row">
        <div class="col-md-4">
            <!-- *** CATEGORY DDL *** -->
            <asp:DropDownList ID="CategoryDropDownList" runat="server" CssClass="form-control"
                AppendDataBoundItems="true" AutoPostBack="true"
                OnSelectedIndexChanged="CategoryDropDown_SelectedIndexChanged" 
                DataSourceID="CategoryODS" DataTextField="Description" DataValueField="CategoryID">
                <asp:ListItem Value="0">Select a Category...</asp:ListItem>
            </asp:DropDownList>
        </div>

        <div class="col-md-4">
            <!-- *** PRODUCT DDL *** -->
            <asp:DropDownList ID="ProductDropDownList" runat="server" CssClass="form-control"
                AppendDataBoundItems="false" AutoPostBack="true"
                OnSelectedIndexChanged="ProductDropDown_SelectedIndexChanged"
                DataSourceID="ProductODS" DataTextField="ItemName" DataValueField="ProductID">
            </asp:DropDownList>
        </div>
        <!-- *** ADD QUANTITY FIELD *** -->
        <div class="col-md-2">
            <asp:TextBox ID="ProductQuantity" runat="server" CssClass="form-control input-sm" TextMode="Number" Width="100" min="1"/>
        </div>
        <!-- *** ADD BUTTON *** -->
        <div class="col-md-2">
            <asp:LinkButton ID="AddProductBtn" runat="server" 
                CssClass="btn btn-light btn-xs" Enabled="false" OnCommand="OnClick_Add_Product"
                    CommandName="AddProductToGridView">+Add</asp:LinkButton>
        </div>
    </div>
    <br /> <br />
    <div class="row">
        <div class="col-md-12">
            <!--*** PURCHASE GRIDVIEW *** -->
            <asp:GridView ID="ProductsPurchase" runat="server"
                CssClass="table table-hover table-sm" 
                ItemType="eRaceSystem.VIEWMODELS.Sales.ProductSummary" 
                DataKeyNames="ProductId" 
                AutoGenerateColumns="false"
                OnRowCommand="ProductsPurchase_RowCommand">
                <EmptyDataTemplate><i>No Products Added</i></EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="Product">
                            <ItemTemplate>
                                <asp:HiddenField ID="ProductId" runat="server" Value="<%# Item.ProductID %>" />
                                <asp:Label ID="Description" runat="server" Text="<%# Item.ItemName %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    <asp:TemplateField HeaderText="Quantity">
                            <ItemTemplate>
                                <asp:TextBox ID="Quantity" runat="server" Text=<%# Item.Quantity %>
                                        CssClass="input-sm" TextMode="Number" min="1" OnTextChanged="On_Qty_Update" AutoPostBack="true" />
                            </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Price">
                            <ItemTemplate>
                                <asp:Label ID="Price" runat="server" Text="<%# Item.ItemPrice %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    <asp:TemplateField HeaderText="Amount">
                            <ItemTemplate>
                                <asp:Label ID="Amount" runat="server" Text="<%# Item.ExtendedPrice %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    <asp:ButtonField Text="X" CommandName="Remove" ControlStyle-CssClass="btn btn-danger btn-sm" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <br /> <br />

    <div class="row">
        <asp:Panel ID="ProcessPurchasePanel" runat="server" Visible="true" >
            <div class="form-inline">
                <div class="form-group row">
                    <div class="col-md-4">
                        <asp:LinkButton ID="PaymentBtn" runat="server" 
                        CssClass="btn btn-light btn-lg" OnCommand="OnClick_Purchase"
                                CommandName="SavePurchase"><b>PAYMENT</b><br /> Cash/Debit</asp:LinkButton>
                    </div>
                    <div class="col-md-3">
                        <asp:LinkButton ID="ClearCart" runat="server" 
                            CssClass="btn btn-secondary btn-md" OnCommand="OnClick_ClearCart"
                                CommandName="ClearCart">Clear Cart</asp:LinkButton>
                    </div>
                    <div class="col-md-5">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">SubTotal</span>
                            </div>
                            <asp:TextBox ID="SubTotal" runat="server" CssClass="form-control" ReadOnly ="true" Text="0.00" style="text-align: right"/>  
                        </div>

                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Tax</span>
                            </div>
                            <asp:TextBox ID="GST" runat="server" CssClass="form-control" ReadOnly ="true" Text="0.00" style="text-align: right"/>  
                        </div>

                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Total</span>
                            </div>
                            <asp:TextBox ID="Total" runat="server" CssClass="form-control" ReadOnly ="true" Text="0.00" style="text-align: right"/>  
                        </div>  
                    </div>
                </div>
             </div>
        </asp:Panel>
    </div>

    <!-- *** DATA SOURCES *** -->
    <asp:ObjectDataSource ID="CategoryODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="Categories_List" TypeName="eRaceSystem.BLL.Sales.SalesController"></asp:ObjectDataSource>

    <asp:ObjectDataSource ID="ProductODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="Products_By_Category" TypeName="eRaceSystem.BLL.Sales.SalesController">
        <SelectParameters>
            <asp:ControlParameter ControlID="CategoryDropDownList"
                PropertyName="SelectedValue" DefaultValue="none"
                Name="CategoryID" Type="String"></asp:ControlParameter>
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>

