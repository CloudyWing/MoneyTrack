using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using CloudyWing.FormValidators;
using CloudyWing.MoneyTrack.Infrastructure.Application;
using CloudyWing.MoneyTrack.Infrastructure.Pages;
using CloudyWing.MoneyTrack.Models;
using CloudyWing.MoneyTrack.Models.DataBinders;
using CloudyWing.MoneyTrack.Models.Queriers;
using CloudyWing.MoneyTrack.Models.Transactions;
using CloudyWing.MoneyTrack.Models.Transactions.Records;

namespace CloudyWing.MoneyTrack.Administrators {
    public partial class RecordEdit : AdministratorPageBase {
        protected int? Id {
            get => (int?)ViewState["Id"];
            set => ViewState["Id"] = value;
        }

        protected bool IsUpdate {
            get {
                object o = ViewState["IsUpdate"];
                return o != null && (bool)o;
            }
            set => ViewState["IsUpdate"] = value;
        }

        public override string RootNavUrl => "~/Administrators/RecordIndex.aspx";

        protected override void LoadTransferVariables(VariableDictionary variables) {
            base.LoadTransferVariables(variables);
            Id = variables.GetValue<int?>("Id");
            IsUpdate = variables.GetValue<bool>("IsUpdate");
        }

        protected override void BindControlDataOnce() {
            CategoryBinder binder = new CategoryBinder(ddlCategoryId, true);
            binder.BindData();

            if (IsUpdate) {
                RecordModel model = RecordModel.Instance;
                SingleTableQuerier querier = new SingleTableQuerier(model);
                querier.AddCondition(model.Id, Id);

                using (SqlDataReader dr = querier.GetDataReader(CommandBehavior.SingleRow)) {
                    if (dr.Read()) {
                        ddlCategoryId.SelectedValue = dr[model.CategoryId.Name].ToString();
                        txtRecordDate.Text = ((DateTime)dr[model.RecordDate.Name]).ToDateString();
                        rblIncome.SelectedValue = dr[model.IsIncome.Name].ToString().ToLower();
                        txtAmount.Text = dr[model.Amount.Name].ToString();
                        txtDescription.Text = dr[model.Description.Name].ToString();
                    }
                }
            } else {
                txtRecordDate.Text = DateTime.Today.ToDateString();
            }
        }

        private void SaveHandler(CommandArgs args) {
            BulkValidator validators = new BulkValidator(cfg => {
                cfg.Add(lblCategoryId.Text, ddlCategoryId.SelectedValue, opt => opt.Required());
                cfg.Add(lblRecordDate.Text, txtRecordDate.Text, opt => opt.Required(), opt => opt.DateTime());
                cfg.Add(lblIncome.Text, rblIncome.SelectedValue, opt => opt.Required(), opt => opt.Bool()

                );
                cfg.Add(lblAmount.Text, txtAmount.Text, opt => opt.Required(), opt => opt.Integer(), opt => opt.MinInt(0));
            });

            if (!validators.Validate()) {
                Script.Alert(validators.ErrorMessageWithLF);
                return;
            }

            (bool IsOk, string Message) = IsUpdate ? Update() : Create();

            if (IsOk) {
                UserContext.PageTransfer.SetMessage(typeof(RecordIndex), Message);
                UserContext.PageTransfer.Variables.Clear();
                Response.RedirectToPrevious();
            } else {
                Script.Alert(Message);
            }
        }

        private (bool IsOk, string Message) Create() {
            RecordTransaction tran = new RecordTransaction();
            tran.CreateInsertAction(
                long.Parse(ddlCategoryId.SelectedValue),
                DateTime.Parse(txtRecordDate.Text),
                bool.Parse(rblIncome.SelectedValue),
                long.Parse(txtAmount.Text),
                txtDescription.Text
            );

            bool result = tran.Execute();
            string message = result ? "新增成功。" : tran.ErrorMessage;

            return (result, message);
        }

        private (bool IsOk, string Message) Update() {
            RecordTransaction tran = new RecordTransaction();
            RecordUpdateAction action = tran.CreateUpdateAction(Id.Value);
            action.CategoryId = long.Parse(ddlCategoryId.SelectedValue);
            action.RecordDate = DateTime.Parse(txtRecordDate.Text);
            action.IsIncome = bool.Parse(rblIncome.SelectedValue);
            action.Amount = long.Parse(txtAmount.Text);
            action.Description = txtDescription.Text;

            tran.Execute();

            bool result = tran.Execute();
            string message = result ? "修改成功。" : tran.ErrorMessage;
            Script.Alert(message);

            return (result, message);
        }

        private void ReturnHandler(CommandArgs args) {
            UserContext.PageTransfer.Variables.Clear();
            Response.RedirectToPrevious();
        }
    }
}
