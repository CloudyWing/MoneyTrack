<%@ Page Title="登入" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CloudyWing.MoneyTrack._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%: Page.Title %></h1>
    <div class="row">
        <div class="col-md-4">
            <section>
                <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
                <div class="form-floating mb-3">
                    <asp:TextBox ID="txtUserId" runat="server" CssClass="form-control" autocomplete="UserId" aria-required="true" placeholder="UserName" />
                    <asp:Label AssociatedControlID="txtUserId" runat="server" CssClass="form-label" Text="帳號"></asp:Label>
                </div>
                <div class="form-floating mb-3">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" autocomplete="password" aria-required="true" placeholder="Password" />
                    <asp:Label AssociatedControlID="txtPassword" runat="server" CssClass="form-label" Text="密碼"></asp:Label>
                </div>
                <div>
                    <asp:Button ID="btnLogin" runat="server" CssClass="w-100 btn btn-lg btn-primary" Text="登入" OnCommand="CommandHandler" CommandName="Login" />
                </div>
            </section>
        </div>
    </div>
</asp:Content>
