using System.Web;
using CloudyWing.FormValidators;
using CloudyWing.MoneyTrack.Administrators;
using CloudyWing.MoneyTrack.Infrastructure.Application;
using CloudyWing.MoneyTrack.Infrastructure.Pages;

namespace CloudyWing.MoneyTrack {
    public partial class _Default : WebPageBase {
        private void LoginHandler(CommandArgs args) {
            BulkValidator validators = new BulkValidator(cfg => {
                cfg.Add("帳號", txtUserId.Text);
                cfg.Add("密碼", txtPassword.Text);
            });

            if (!validators.Validate()) {
                Script.Alert(ErrorPage);
                return;
            }

            if (ApplicationSettingManager.Instance.VerifyUser(txtUserId.Text, txtPassword.Text)) {
                UserContext.Id = txtUserId.Text;
                UserContext.PageTransfer.SetMessage(typeof(RecordIndex), "登入成功。");
                Response.RedirectToNext("~/Administrators/RecordIndex.aspx");
            } else {
                Script.Alert("帳號或密碼錯誤。");
                return;
            }
        }
    }
}
