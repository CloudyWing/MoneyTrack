using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudyWing.MoneyTrack.Infrastructure.Application;
using CloudyWing.MoneyTrack.Infrastructure.Pages;
using CloudyWing.MoneyTrack.Models;
using CloudyWing.MoneyTrack.Models.Enumerations;
using CloudyWing.MoneyTrack.Models.Queriers;
using CloudyWing.MoneyTrack.Models.Transactions;

namespace CloudyWing.MoneyTrack.Administrators {
    public partial class CategoryIndex : AdministratorPageBase {
        protected override bool AutoLoadData => true;

        protected override void LoadPageCacheVariables(VariableDictionary variables) {
            txtName.Text = variables.GetValue<string>("Name");
            variables.Clear();
        }

        protected override void BindData() {
            CategoryModel model = CategoryModel.Instance;
            SingleTableQuerier querier = new SingleTableQuerier(model);

            if (!string.IsNullOrEmpty(txtName.Text)) {
                querier.AddCondition(model.Name, CompareMode.Like, txtName.Text);
            }
            querier.OrderBy = model.DisplayOrder.Name;
            gvList.DataSource = querier.GetDataTable();
            gvList.DataBind();

            VariableDictionary vars = UserContext.PageCache.Variables;
            vars.Add("Name", txtName.Text);
        }

        protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e) {
            if (e.Row.RowType == DataControlRowType.DataRow) {
                GridView grid = (GridView)sender;
                DataTable source = grid.DataSource as DataTable;

                if (e.Row.RowIndex == 0) {
                    Control btnMoveUp = e.Row.FindControl("btnMoveUp");
                    btnMoveUp.Visible = false;
                }

                if (e.Row.RowIndex == source.Rows.Count - 1) {
                    Control btnMoveDown = e.Row.FindControl("btnMoveDown");
                    btnMoveDown.Visible = false;
                }
            }
        }

        private void QueryHandler(CommandArgs args) { }

        private void EditHandler(CommandArgs args) {
            bool hasArgs = args.Arguments.Any();
            VariableDictionary vars = UserContext.PageTransfer.GetVariables(typeof(CategoryEdit));
            vars.Add("IsUpdate", hasArgs);
            vars.Add("Id", hasArgs ? (int?)int.Parse(args.Arguments[0]) : null);

            Response.RedirectToNext("./CategoryEdit.aspx");
        }

        private void Delete2Handler(CommandArgs args) {
            int id = int.Parse(args.Arguments.First());
            CategoryTransaction tran = new CategoryTransaction();
            tran.CreateDeleteAction(id);

            if (!tran.Execute()) {
                Script.Alert(tran.ErrorMessage);
                return;
            }

            UserContext.PageTransfer.SetMessage(typeof(CategoryIndex), "刪除成功。");
            Response.RedirectToSelf();
        }

        private void MoveUpHandler(CommandArgs args) {
            int id = int.Parse(args.Arguments.First());
            CategoryTransaction tran = new CategoryTransaction();
            tran.CreateMoveUpAction(id);

            if (!tran.Execute()) {
                Script.Alert(tran.ErrorMessage);
            }

            Response.RedirectToSelf();
        }

        private void MoveDownHandler(CommandArgs args) {
            int id = int.Parse(args.Arguments.First());
            CategoryTransaction tran = new CategoryTransaction();
            tran.CreateMoveDownAction(id);

            if (!tran.Execute()) {
                Script.Alert(tran.ErrorMessage);
            }

            Response.RedirectToSelf();
        }
    }
}
