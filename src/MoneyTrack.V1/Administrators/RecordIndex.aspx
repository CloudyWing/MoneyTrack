<%@ Page Language="C#" MasterPageFile="~/MasterPages/Query.Master" AutoEventWireup="true" CodeBehind="RecordIndex.aspx.cs" Inherits="CloudyWing.MoneyTrack.Administrators.RecordIndex" %>

<%@ Register Src="~/Infrastructure/UserControls/PagedListPager.ascx" TagPrefix="uc" TagName="PagedListPager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="QueryFields" runat="server">
    <div class="row">
        <div class="col-12 col-md-6">
            <div class="row mb-3">
                <asp:Label runat="server" ID="lblCategoryId" AssociatedControlID="ddlCategoryId" CssClass="col-3 col-md-4 col-form-label" Text="分類"></asp:Label>
                <div class="col-9 col-md-8">
                    <asp:DropDownList runat="server" ID="ddlCategoryId" CssClass="form-select"></asp:DropDownList>
                </div>
            </div>
        </div>
        <div class="col-12 col-md-6">
            <div class="row mb-3">
                <asp:Label runat="server" ID="lblIncome" AssociatedControlID="rblIncome" CssClass="col-3 col-md-4 col-form-label" Text="收入/支出"></asp:Label>
                <div class="col-9 col-md-8">
                    <asp:RadioButtonList runat="server" ID="rblIncome" RepeatLayout="flow" RepeatDirection="Horizontal">
                        <asp:ListItem class="form-check-item form-check-item-inline" Text="全部" Value=""></asp:ListItem>
                        <asp:ListItem class="form-check-item form-check-item-inline" Text="收入" Value="true"></asp:ListItem>
                        <asp:ListItem class="form-check-item form-check-item-inline" Text="支出" Value="false"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-12 col-md-6">
            <div class="row mb-3">
                <asp:Label runat="server" ID="lblStartDate" AssociatedControlID="txtStartDate" CssClass="col-3 col-md-4 col-form-label" Text="開始日期"></asp:Label>
                <div class="col-9 col-md-8">
                    <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="col-12 col-md-6">
            <div class="row mb-3">
                <asp:Label runat="server" ID="lblEndDate" AssociatedControlID="txtEndDate" CssClass="col-3 col-md-4 col-form-label" Text="結束日期"></asp:Label>
                <div class="col-9 col-md-8">
                    <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-12 col-md-6">
            <div class="row mb-3">
                <asp:Label runat="server" ID="lblMinAmount" AssociatedControlID="txtMinAmount" CssClass="col-3 col-md-4 col-form-label" Text="最小金額"></asp:Label>
                <div class="col-9 col-md-8">
                    <asp:TextBox ID="txtMinAmount" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="col-12 col-md-6">
            <div class="row mb-3">
                <asp:Label runat="server" ID="lblMaxAmount" AssociatedControlID="txtMaxAmount" CssClass="col-3 col-md-4 col-form-label" Text="最大金額"></asp:Label>
                <div class="col-9 col-md-8">
                    <asp:TextBox ID="txtMaxAmount" runat="server" TextMode="Number" step="1" min="0" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
    <div class="row mb-3">
        <asp:Label runat="server" ID="lblDescription" AssociatedControlID="txtDescription" CssClass="col-3 col-md-2 col-form-label" Text="備註"></asp:Label>
        <div class="col-9 col-md-10">
            <asp:TextBox runat="server" ID="txtDescription" TextMode="Search" CssClass="form-control"></asp:TextBox>
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
    <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-hover">
        <Columns>
            <asp:BoundField DataField="CategoryName" HeaderText="分類" />
            <asp:TemplateField HeaderText="交易日期">
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# ((DateTime)Eval("RecordDate")).ToDateString() %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="收入/支出">
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# (bool)Eval("IsIncome") ? "收入" : "支出" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Amount" HeaderText="金額" />
            <asp:BoundField DataField="Description" HeaderText="備註" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="btnEdit" runat="server" CssClass="btn btn-info" Text="編輯" OnCommand="CommandHandler" CommandName="Edit" CommandArgument='<%# Eval("Id") %>' />
                    <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-danger delete" Text="刪除" OnCommand="CommandHandler" CommandName="Delete2" CommandArgument='<%# Eval("Id") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <uc:PagedListPager runat="server" ID="PagedListPager" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
