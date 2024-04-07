<%@ Page Language="C#" MasterPageFile="~/MasterPages/Form.Master" AutoEventWireup="true" CodeBehind="RecordEdit.aspx.cs" Inherits="CloudyWing.MoneyTrack.Administrators.RecordEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Container" runat="server">
    <div class="row mb-3">
        <asp:Label ID="lblCategoryId" AssociatedControlID="ddlCategoryId" runat="server" CssClass="col-sm-3 col-md-2 form-label" Text="分類"></asp:Label>
        <div class="col-sm-9 col-md-10">
            <asp:DropDownList runat="server" ID="ddlCategoryId" CssClass="form-select"></asp:DropDownList>
        </div>
    </div>
    <div class="row mb-3">
        <asp:Label ID="lblRecordDate" AssociatedControlID="txtRecordDate" runat="server" CssClass="col-sm-3 col-md-2 col-form-label" Text="交易日期"></asp:Label>
        <div class="col-sm-9 col-md-10">
            <asp:TextBox ID="txtRecordDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
        </div>
    </div>
    <div class="row mb-3">
        <asp:Label ID="lblIncome" AssociatedControlID="rblIncome" runat="server" CssClass="col-sm-3 col-md-2 col-form-label" Text="收入/支出"></asp:Label>
        <div class="col-sm-9 col-md-10">
            <asp:RadioButtonList runat="server" ID="rblIncome" RepeatLayout="Flow" RepeatDirection="Horizontal">
                <asp:ListItem class="form-check-item form-check-item-inline" Text="收入" Value="true"></asp:ListItem>
                <asp:ListItem class="form-check-item form-check-item-inline" Text="支出" Value="false"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
    </div>
    <div class="row mb-3">
        <asp:Label ID="lblAmount" AssociatedControlID="txtAmount" runat="server" CssClass="col-sm-3 col-md-2 col-form-label" Text="金額"></asp:Label>
        <div class="col-sm-9 col-md-10">
            <asp:TextBox ID="txtAmount" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
        </div>
    </div>
    <div class="row mb-3">
        <asp:Label ID="lblDescription" AssociatedControlID="txtDescription" runat="server" CssClass="col-sm-3 col-md-2 col-form-label" Text="備註"></asp:Label>
        <div class="col-sm-9 col-md-10">
            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Buttons" runat="server">
    <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="儲存" OnCommand="CommandHandler" CommandName="Save" />
    <asp:Button ID="btnReturn" runat="server" CssClass="btn btn-info" Text="返回" OnCommand="CommandHandler" CommandName="Return" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
