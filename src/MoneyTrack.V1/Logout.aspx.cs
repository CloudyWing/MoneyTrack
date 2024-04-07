using System;
using CloudyWing.MoneyTrack.Infrastructure.Pages;

namespace CloudyWing.MoneyTrack {
    public partial class Logout : WebPageBase {
        protected void Page_Load(object sender, EventArgs e) {
            Session.Clear();
            Session.Abandon();

            UserContext.PageTransfer.SetMessage(typeof(_Default), "登出成功。");
            Response.Redirect("~/Default.aspx");
        }
    }
}
