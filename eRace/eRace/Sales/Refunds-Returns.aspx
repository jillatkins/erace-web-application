<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Refunds-Returns.aspx.cs" Inherits="eRace.Sales.Refunds_Returns" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jimbotron">
        <h1>Returns & Refunds</h1>
        <asp:Label ID="Message" runat="server" />
        <uc1:MessageUserControl runat="server" ID="MessageUserControl" />
        <br /> <br />
    </div>
    <!-- ****** LOOKUP TEXT BOX & BUTTONS PANEL -->
    <div class="row">
        <asp:Panel ID="LookupInvoices" runat="server" Visible="true" >
            
            <asp:Label ID="Label1" runat="server" AssociatedControlID="OriginalInvoice">Original Invoice #</asp:Label>
            <div class="form-inline">        
                <asp:TextBox ID="OriginalInvoice" runat="server" CssClass="form-control" TextMode="Number" min="0"/>
                &nbsp;&nbsp;
                <asp:LinkButton ID="LookupInvoice" runat="server" CssClass="btn btn-info" OnCommand="OnClick_Lookup_Invoice"
                        CommandName="LookupInvoice">Lookup Invoice</asp:LinkButton>
                &nbsp;&nbsp;
                <asp:LinkButton ID="ClearPage" runat="server" CssClass="btn btn-secondary" OnCommand="OnClick_Clear_Page"
                        CommandName="ClearPage">Clear</asp:LinkButton>
            </div>
        </asp:Panel>
    </div>
    <br /> <br />
    <div class="row">
        <!-- ****** INVOICE PRODUCTS GRIDVIEW -->
        <asp:GridView ID="InvoiceItems" runat="server"
                CssClass="table table-hover table-sm" 
                ItemType="eRaceSystem.VIEWMODELS.Sales.InvoiceProducts" 
                DataKeyNames="ProductId" 
                AutoGenerateColumns="false"
                OnRowCommand="InvoiceItems_RowCommand">
                <EmptyDataTemplate><i>No Products Found</i></EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="Product">
                            <ItemTemplate>
                                <asp:HiddenField ID="CategoryId" runat="server" Value="<%# Item.CategoryID %>" />
                                <asp:HiddenField ID="ProductId" runat="server" Value="<%# Item.ProductID %>" />
                                <asp:Label ID="Description" runat="server" Text="<%# Item.ItemName %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    <asp:TemplateField HeaderText="Qty">
                            <ItemTemplate>
                                <asp:Label ID="Quantity" runat="server" Text="<%# Item.Quantity %>" />
                            </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Price">
                            <ItemTemplate>
                                <asp:Label ID="Price" runat="server" Text="<%# Item.Price %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    <asp:TemplateField HeaderText="Amount">
                            <ItemTemplate>
                                <asp:Label ID="Amount" runat="server" Text="<%# Item.Amount %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    <asp:TemplateField HeaderText="Restock Charge">
                            <ItemTemplate>
                                <asp:CheckBox ID="RestockChgExists" runat="server" Checked="<%# Item.RestockChgExists %>" Enabled="false" />
                                <asp:Label ID="RestockCharge" runat="server" Text="<%# Item.RestockCharge %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    <asp:TemplateField HeaderText="Refund Reason">
                            <ItemTemplate>
                                <asp:CheckBox ID="Refund" runat="server" Enabled="true" OnCheckedChanged="OnCheck_Refund" AutoPostBack="true" />
                                <asp:TextBox ID="RefundReason" runat="server" Text="<%# Item.Reason %>" Enabled="false"
                                    CssClass="form-control input-sm" />
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
    </div>
    <br /> <br />
    <div class="row">
        <asp:Panel ID="ProcessRefundPanel" runat="server" Visible="true" >
            <div class="form-inline">
                <div class="form-group row">
                    <div class="col-md-5">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SubTotal</span>
                            </div>
                            <asp:TextBox ID="SubTotal" runat="server" CssClass="form-control" ReadOnly="true" Text="0.00" style="text-align: right" />  
                        </div>

                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Tax</span>
                            </div>
                            <asp:TextBox ID="GST" runat="server" CssClass="form-control" ReadOnly="true" Text="0.00" style="text-align: right" />  
                        </div>

                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">Refund Total</span>
                            </div>
                            <asp:TextBox ID="RefundTotal" runat="server" CssClass="form-control" ReadOnly="true" Text="0.00" style="text-align: right" />  
                        </div>  
                    </div>

                    <div class="col-md-6">
                        <asp:LinkButton ID="RefundBtn" runat="server" 
                        CssClass="btn btn-light btn-lg" OnCommand="OnClick_Refund" Enabled="false"
                                CommandName="RefundItems"><b>REFUND</b><br /> Cash/Debit</asp:LinkButton>
                        <asp:TextBox ID="RefundInvoiceNum" runat="server" CssClass="form-control" ReadOnly ="true" Text="Refund Invoice #"/>
                    </div>
                </div>
             </div>
        </asp:Panel>
    </div>


</asp:Content>
