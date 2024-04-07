<%@ Page Language="C#" MasterPageFile="~/MasterPages/Query.Master" AutoEventWireup="true" CodeBehind="CategoryIndex.aspx.cs" Inherits="CloudyWing.MoneyTrack.Administrators.CategoryIndex" %>

<%@ Register Src="~/Infrastructure/UserControls/PagedListPager.ascx" TagPrefix="uc" TagName="PagedListPager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="QueryFields" runat="server">
    <div class="row mb-3">
        <asp:Label runat="server" ID="lblName" AssociatedControlID="txtName" CssClass="col-sm-2 col-form-label" Text="分類名稱"></asp:Label>
        <div class="col-sm-10">
            <asp:TextBox runat="server" ID="txtName" TextMode="Search" CssClass="form-control"></asp:TextBox>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="QueryButtons" runat="server">
    <asp:Button ID="btnQuery" runat="server" CssClass="btn btn-primary" Text="查詢" OnCommand="CommandHandler" CommandName="Query" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Buttons" runat="server">
    <asp:Button ID="btnCreate" runat="server" CssClass="btn btn-info" Text="新增" OnCommand="CommandHandler" CommandName="Edit" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="QueryResults" runat="server">
    <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-hover" OnRowDataBound="gvList_RowDataBound">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="分類名稱" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="btnEdit" runat="server" CssClass="btn btn-info" Text="編輯" OnCommand="CommandHandler" CommandName="Edit" CommandArgument='<%# Eval("Id") %>' />
                    <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-danger delete" Text="刪除" OnCommand="CommandHandler" CommandName="Delete2" CommandArgument='<%# Eval("Id") %>' />
                    <asp:Button ID="btnMoveUp" runat="server" CssClass="btn btn-secondary" Text="上移" OnCommand="CommandHandler" CommandName="MoveUp" CommandArgument='<%# Eval("Id")%>' />
                    <asp:Button ID="btnMoveDown" runat="server" CssClass="btn btn-secondary" Text="下移" OnCommand="CommandHandler" CommandName="MoveDown" CommandArgument='<%# Eval("Id")%>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
