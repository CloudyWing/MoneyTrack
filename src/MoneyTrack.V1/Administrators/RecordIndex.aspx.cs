using System;
using System.Linq;
using System.Web;
using CloudyWing.FormValidators;
using CloudyWing.MoneyTrack.Infrastructure.Application;
using CloudyWing.MoneyTrack.Infrastructure.Pages;
using CloudyWing.MoneyTrack.Infrastructure.UserControls;
using CloudyWing.MoneyTrack.Models;
using CloudyWing.MoneyTrack.Models.DataBinders;
using CloudyWing.MoneyTrack.Models.Enumerations;
using CloudyWing.MoneyTrack.Models.Transactions;

namespace CloudyWing.MoneyTrack.Administrators {
    public partial class RecordIndex : AdministratorPageBase {
        protected override bool AutoLoadData => true;

        protected override void LoadPageCacheVariables(VariableDictionary variables) {
            ddlCategoryId.SelectedValue = variables.GetValue<string>("CategoryId");
            rblIncome.SelectedValue = variables.GetValue<string>("Income");
            txtStartDate.Text = variables.GetValue<string>("StartDate");
            txtEndDate.Text = variables.GetValue<string>("EndDate");
            txtDescription.Text = variables.GetValue<string>("Description");
            variables.Clear();
        }

        protected override void BindControlDataOnce() {
            DateTime today = DateTime.Today;
            DateTime startDate = today.AddDays(0 - today.Day + 1);

            txtStartDate.Text = startDate.ToDateString();
            txtEndDate.Text = startDate.AddMonths(1).AddDays(-1).ToDateString();

            CategoryBinder binder = new CategoryBinder(ddlCategoryId, true) {
                EmptyItemText = "全選"
            };
            binder.BindData();
            rblIncome.SelectedValue = "";
        }

        protected override void BindControlData() {
            PagedListPager.DataBinder = BindData;
        }

        protected override void BindData() {
            BindData(1);
        }

        private void BindData(int pageNumber) {
            BulkValidator validators = new BulkValidator(cfg => {
                cfg.Add(lblCategoryId.Text, ddlCategoryId.SelectedValue, opt => opt.Integer());
                cfg.Add(lblIncome.Text, rblIncome.SelectedValue, opt => opt.Bool());
                cfg.Add(lblStartDate.Text, txtStartDate.Text, opt => opt.DateTime());
                cfg.Add(lblEndDate.Text, txtEndDate.Text, opt => opt.DateTime());
                cfg.Add(lblMinAmount.Text, txtMinAmount.Text, opt => opt.Integer());
                cfg.Add(lblMaxAmount.Text, txtMaxAmount.Text, opt => opt.Integer());
            });

            if (!validators.Validate()) {
                Script.Alert(validators.ErrorMessageWithLF);
                return;
            }

            RecordModel recordModel = RecordModel.Instance;

            RecordQuerier querier = new RecordQuerier {
                PageNumber = pageNumber,
                PageSize = 20
            };
            if (!string.IsNullOrEmpty(ddlCategoryId.SelectedValue)) {
                querier.AddCondition(recordModel.CategoryId, ddlCategoryId.SelectedValue);
            }

            if (!string.IsNullOrEmpty(rblIncome.SelectedValue)) {
                querier.AddCondition(recordModel.IsIncome, rblIncome.SelectedValue);
            }

            if (!string.IsNullOrEmpty(txtStartDate.Text)) {
                querier.AddCondition(recordModel.RecordDate, CompareMode.GreaterOrEqual, txtStartDate.Text);
            }

            if (!string.IsNullOrEmpty(txtEndDate.Text)) {
                querier.AddCondition(recordModel.RecordDate, CompareMode.LessOrEqual, txtEndDate.Text);
            }

            if (!string.IsNullOrEmpty(txtMinAmount.Text)) {
                querier.AddCondition(recordModel.Amount, CompareMode.GreaterOrEqual, txtMinAmount.Text);
            }

            if (!string.IsNullOrEmpty(txtMaxAmount.Text)) {
                querier.AddCondition(recordModel.Amount, CompareMode.LessOrEqual, txtMaxAmount.Text);
            }

            if (!string.IsNullOrEmpty(txtDescription.Text)) {
                querier.AddCondition(recordModel.Description, CompareMode.Like, txtDescription.Text);
            }
            gvList.DataSource = querier.GetDataTable();
            gvList.DataBind();

            PagedListPager.BindMetadata(querier.GetPagingMetadata());

            VariableDictionary vars = UserContext.PageCache.Variables;
            vars.Add("CategoryId", ddlCategoryId.SelectedValue);
            vars.Add("Income", rblIncome.SelectedValue);
            vars.Add("StartDate", txtStartDate.Text);
            vars.Add("EndDate", txtEndDate.Text);
            vars.Add("Description", txtDescription.Text);
        }

        private void QueryHandler(CommandArgs args) { }

        private void EditHandler(CommandArgs args) {
            bool hasArgs = args.Arguments.Any();
            VariableDictionary vars = UserContext.PageTransfer.GetVariables(typeof(RecordEdit));
            vars.Add("IsUpdate", hasArgs);
            vars.Add("Id", hasArgs ? (int?)int.Parse(args.Arguments[0]) : null);

            Response.RedirectToNext("./RecordEdit.aspx");
        }

        private void Delete2Handler(CommandArgs args) {
            int id = int.Parse(args.Arguments.First());
            RecordTransaction tran = new RecordTransaction();
            tran.CreateDeleteAction(id);

            if (!tran.Execute()) {
                Script.Alert(tran.ErrorMessage);
                return;
            }

            UserContext.PageTransfer.SetMessage(typeof(RecordIndex), "刪除成功。");
            Response.RedirectToSelf();
        }
    }
}
