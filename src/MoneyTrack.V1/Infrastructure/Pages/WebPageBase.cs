using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudyWing.MoneyTrack.Infrastructure.Application;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Infrastructure.Pages {
    public abstract class WebPageBase : Page {
        private ScriptHelper script;
        private UserContext userContext;
        private readonly Dictionary<string, Action<CommandArgs>> commandMaps = new Dictionary<string, Action<CommandArgs>>(StringComparer.OrdinalIgnoreCase);

        protected ScriptHelper Script {
            get {
                if (script is null) {
                    script = new ScriptHelper(Context);
                }
                return script;
            }
        }

        protected UserContext UserContext {
            get {
                if (userContext is null) {
                    userContext = UserContext.GetInstance(Context);
                }
                return userContext;
            }
        }

        protected virtual bool IsLoginRequired => false;

        protected virtual bool AutoLoadData => false;

        protected HostEnvironment HostEnvironment { get; } = new HostEnvironment();

        public virtual string RootNavUrl => AppRelativeVirtualPath;

        protected override void OnPreInit(EventArgs e) {
            base.OnPreInit(e);

            if (IsLoginRequired && !UserContext.IsLogined) {
                UserContext.PageTransfer.SetMessage(typeof(_Default), "此頁面必須要登入才能瀏覽！");
                Response.Redirect("~/Default.aspx");
            }
        }

        protected override void OnInit(EventArgs e) {
            base.OnInit(e);

            MethodInfo[] methods = GetType().BaseType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MethodInfo method in methods) {
                if (method.Name.EndsWith("Handler") && method.GetParameters().Length == 1) {
                    Type paramType = method.GetParameters()[0].ParameterType;

                    if (typeof(CommandArgs).IsAssignableFrom(paramType)) {
                        string commandName = method.Name.Substring(0, method.Name.Length - "Handler".Length);
                        Type actionType = typeof(Action<>).MakeGenericType(paramType);
                        Delegate action = Delegate.CreateDelegate(actionType, this, method);
                        commandMaps[commandName] = (Action<CommandArgs>)action;
                    }
                }
            }
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            IEnumerable<string> messages = UserContext.PageTransfer
                .GetMessages(PageTransferContext.AnyPage, GetType().BaseType.Name);

            foreach (string message in messages) {
                Script.Alert(message);
            }

            UserContext.PageTransfer
                .RemoveMessages(PageTransferContext.AnyPage, GetType().BaseType.Name);

            if (!IsPostBack) {
                if (UserContext.PageTransfer.Variables.Any()) {
                    LoadTransferVariables(UserContext.PageTransfer.Variables);
                }

                BindControlDataOnce();

                if (UserContext.PageCache.Variables.Any()) {
                    LoadPageCacheVariables(UserContext.PageCache.Variables);
                }
            }

            BindControlData();
        }

        /// <summary>
        /// Triggered on the initial page load; won't execute again during PostBacks. Mainly used for binding control data sources and setting initial values.
        /// </summary>
        protected virtual void BindControlDataOnce() { }

        /// <summary>
        /// Triggered on each page load, executed at the end of <c>Page_Load</c>. Used for operations like setting <c>PagedListPager.DataBinder = BindData</c>.
        /// </summary>
        protected virtual void BindControlData() { }

        /// <summary>
        /// Triggered on the initial page load; won't execute again during PostBacks. Executed before <c>BindControlDataOnce</c>. Mainly used to read passed values from other pages, process them, and set data sources or values for page controls. For example, on an edit page, querying existing data using the passed <c>Id</c> from a search page, then setting values to respective controls.
        /// </summary>
        protected virtual void LoadTransferVariables(VariableDictionary variables) { }

        /// <summary>
        /// Triggered on the initial page load; won't execute again during PostBacks. Executed after <c>BindControlDataOnce</c>. Mainly used to read previously cached values, such as storing query conditions on a query page and retrieving them when returning from an edit page to prevent loss of query conditions.
        /// </summary>
        protected virtual void LoadPageCacheVariables(VariableDictionary variables) { }

        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);

            if (IsPostBack || AutoLoadData) {
                BindData();
            }

            SitePageTitle(SiteMap.RootNode);
        }

        protected virtual void BindData() { }

        protected void CommandHandler(object sender, CommandEventArgs e) {
            string commandName = e.CommandName;
            if (commandMaps.TryGetValue(commandName, out Action<CommandArgs> action)) {
                CommandArgs args = new CommandArgs(sender, e.CommandName, e.CommandArgument);
                action(args);
            }
        }

        private void SitePageTitle(SiteMapNode node) {
            if (ResolveUrl(RootNavUrl).Equals(node.Url, StringComparison.OrdinalIgnoreCase)) {
                Title = node.Title;
                return;
            }

            foreach (SiteMapNode childNode in node.ChildNodes) {
                SitePageTitle(childNode);
            }
        }

        protected override void OnError(EventArgs e) {
            UserContext.PageTransfer.SetMessageOfAnyPage("系統運行異常。");

            if (IsPostBack) {
                Response.Redirect(Request.Url.AbsoluteUri);
            } else {
                if (Request.UrlReferrer is null) {
                    if (ResolveUrl(AppRelativeVirtualPath) == ResolveUrl("~/Default.aspx")) {
                        Server.ClearError();
                    } else {
                        Response.Redirect("~/Default.aspx");
                    }
                } else {
                    Response.Redirect(Request.UrlReferrer.AbsoluteUri);
                }
            }

            base.OnError(e);
        }
    }
}
