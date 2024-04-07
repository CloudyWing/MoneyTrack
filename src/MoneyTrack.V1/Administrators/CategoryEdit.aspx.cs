using System.Data;
using System.Data.SqlClient;
using System.Web;
using CloudyWing.FormValidators;
using CloudyWing.MoneyTrack.Infrastructure.Application;
using CloudyWing.MoneyTrack.Infrastructure.Pages;
using CloudyWing.MoneyTrack.Models;
using CloudyWing.MoneyTrack.Models.Queriers;
using CloudyWing.MoneyTrack.Models.Transactions;
using CloudyWing.MoneyTrack.Models.Transactions.Categories;

namespace CloudyWing.MoneyTrack.Administrators {
    public partial class CategoryEdit : AdministratorPageBase {
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

        public override string RootNavUrl => "~/Manage/CategoryIndex.aspx";

        protected override void LoadTransferVariables(VariableDictionary variables) {
            base.LoadTransferVariables(variables);
            Id = variables.GetValue<int?>("Id");
            IsUpdate = variables.GetValue<bool>("IsUpdate");
        }

        protected override void BindControlData() {
            if (IsUpdate) {
                CategoryModel model = CategoryModel.Instance;
                SingleTableQuerier querier = new SingleTableQuerier(model);
                querier.AddCondition(model.Id, Id);

                using (SqlDataReader dr = querier.GetDataReader(CommandBehavior.SingleRow)) {
                    if (dr.Read()) {
                        txtName.Text = dr["Name"].ToString();
                    }
                }
            }
        }

        private void SaveHandler(CommandArgs args) {
            BulkValidator validators = new BulkValidator(cfg => {
                cfg.Add(lblName.Text, txtName.Text, opt => opt.Required());
            });

            if (!validators.Validate()) {
                Script.Alert(validators.ErrorMessageWithLF);
                return;
            }

            (bool IsOk, string Message) = IsUpdate ? Update() : Create();

            if (IsOk) {
                UserContext.PageTransfer.SetMessage(typeof(CategoryIndex), Message);
                UserContext.PageTransfer.Variables.Clear();
                Response.RedirectToPrevious();
            } else {
                Script.Alert(Message);
            }
        }

        private (bool IsOk, string Message) Create() {
            CategoryTransaction tran = new CategoryTransaction();
            tran.CreateInsertAction(txtName.Text);

            bool result = tran.Execute();
            string message = result ? "新增成功。" : tran.ErrorMessage;

            return (result, message);
        }

        private (bool IsOk, string Message) Update() {
            CategoryTransaction tran = new CategoryTransaction();
            CategoryUpdateAction action = tran.CreateUpdateAction(Id.Value);
            action.Name = txtName.Text;

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
