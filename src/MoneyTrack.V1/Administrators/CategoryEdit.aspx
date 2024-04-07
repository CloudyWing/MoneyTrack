<%@ Page Language="C#" MasterPageFile="~/MasterPages/Form.Master" AutoEventWireup="true" CodeBehind="CategoryEdit.aspx.cs" Inherits="CloudyWing.MoneyTrack.Administrators.CategoryEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Container" runat="server">
    <div class="row mb-3">
        <asp:Label ID="lblName" AssociatedControlID="txtName" runat="server" CssClass="col-sm-3 ccol-md-2 col-form-label" Text="分類名稱"></asp:Label>
        <div class="col-sm-9 ccol-md-10">
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Buttons" runat="server">
    <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="儲存" OnCommand="CommandHandler" CommandName="Save" />
    <asp:Button ID="btnReturn" runat="server" CssClass="btn btn-info" Text="返回" OnCommand="CommandHandler" CommandName="Return" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
