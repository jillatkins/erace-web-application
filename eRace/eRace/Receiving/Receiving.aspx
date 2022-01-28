<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Receiving.aspx.cs" Inherits="eRace.Receiving.Receiving" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <uc1:MessageUserControl runat="server" ID="MessageUserControl" />
        <h1>Receiving</h1>
    </div>
    <div class="row">
        <div class="col-md-11">
            <asp:DropDownList ID="VendorDropDown" runat="server" DataSourceID="PurchaseOrderODS" AppendDataBoundItems="true" DataTextField="DisplayText" DataValueField="DisplayValue">
                <asp:ListItem Value="0">[Select a PO]</asp:ListItem>
            </asp:DropDownList>

            <asp:ObjectDataSource ID="PurchaseOrderODS" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="Vendor_DDL" TypeName="eRaceSystem.BLL.Receiving.ReceivingController"></asp:ObjectDataSource>
            <asp:Button ID="VendorDetailsFetch" runat="server" Text="Open" OnClick="OnClick_GetVendorAndPurchaseOrderDetails" />
            <br />
            <asp:Label ID="VendorName" runat="server" class="font-weight-bold "></asp:Label>
            <asp:Label ID="VendorAddressPlusCity" runat="server"></asp:Label>
            <br />
            <asp:Label ID="VendorContact" runat="server"></asp:Label>
            <asp:Label ID="VendorPhone" runat="server"></asp:Label>
        </div>
    </div>
    <div class="row">
        <div class="offset-1 col-md-11">
            <asp:GridView ID="PurchaseOrderGridView" runat="server" ItemType="eRaceSystem.VIEWMODELS.Receiving.PurchaseOrderDetails" AutoGenerateColumns="false">               
                <Columns>                    
                    <asp:TemplateField HeaderText="Item">
                        <asp:ItemTemplate>
                            <asp:Label runat="server" ID="ItemName" Text="<%# Item.ItemName %>"></asp:Label>
                        </asp:ItemTemplate>                    
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Quantity Ordered">
                        <asp:ItemTemplate>
                            <asp:Label ID="OrderQuantity" runat="server" Text="<%# Item.QtyOrdered %>"></asp:Label>
                        </asp:ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Ordered Units">
                        <asp:ItemTemplate>
                            <asp:Label ID="OrderedUnitsQuantity" runat="server" Text="<%#Item.QtyOrdered%>"></asp:Label>
                            <asp:Label ID="OrderedUnits" runat="server" Text="<%#Item.OrderUnitSize%>"></asp:Label>
                        </asp:ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="QuantityOutstanding">
                        <asp:ItemTemplate>
                            <asp:Label ID="QtyOutstanding" runat="server" Text="<%#Item.QuantityOutstanding %>"></asp:Label>
                        </asp:ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ReceivedUnits">
                        <asp:ItemTemplate>
                            <asp:TextBox ID="ReceivedUnits" runat="server" Text="<%#Item.ReceivedItemQuantity %>"></asp:TextBox>
                            <asp:Label ID="OrderUnits" runat="server" Text="<%#Item.OrderUnitSize%>"></asp:Label>
                        </asp:ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Rejected Units/Reason">
                        <asp:ItemTemplate>
                            <asp:TextBox ID="RejectedUnits" runat="server" Text="<%#Item.RejectedUnits %>"></asp:TextBox>
                            <asp:TextBox ID="Reason" runat="server" Text="<%#Item.ReturnReason %>"></asp:TextBox>
                        </asp:ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Salvaged Items">
                        <asp:ItemTemplate>
                            <asp:TextBox ID="SalvagedItems" runat="server"></asp:TextBox>
                        </asp:ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    no data
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
