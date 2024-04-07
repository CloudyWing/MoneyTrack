<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagedListPager.ascx.cs" Inherits="CloudyWing.MoneyTrack.Infrastructure.UserControls.PagedListPager" %>
<asp:Panel ID="pnlPager" runat="server" Visible="false">
    <nav aria-label="Page navigation">
        <ul class="pagination justify-content-center">
            <li class="page-item">
                <asp:Button ID="btnFirstPage" runat="server" CssClass="page-link" OnCommand="btnPage_Command" Text="<<"></asp:Button>
            </li>
            <li class="page-item">
                <asp:Button ID="btnPrevPage" runat="server" CssClass="page-link" OnCommand="btnPage_Command" Text="<"></asp:Button>
            </li>
            <asp:Repeater ID="rptPage" runat="server">
                <ItemTemplate>
                    <li class="page-item">
                        <asp:Button ID="btnPage" runat="server" OnCommand="btnPage_Command" CommandArgument="<%# Container.DataItem %>"
                            CssClass='<%# PageNumber.ToString() == Container.DataItem.ToString() ? "page-link active": "page-link" %>' Text="<%# Container.DataItem %>"></asp:Button>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
            <li class="page-item">
                <asp:Button ID="btnNextPage" runat="server" CssClass="page-link" OnCommand="btnPage_Command" Text=">"></asp:Button>
            </li>
            <li class="page-item">
                <asp:Button ID="btnLastPage" runat="server" CssClass="page-link" OnCommand="btnPage_Command" Text=">>"></asp:Button>
            </li>
            <asp:HiddenField ID="hidPage" runat="server" />
        </ul>
    </nav>
</asp:Panel>
