using System.Web.UI.WebControls;

namespace CloudyWing.MoneyTrack.Models.DataBinders {
    public abstract class ListDataBinder {
        private string emptyItemText;
        private readonly bool hasEmptyItem;

        public ListDataBinder(ListControl control, bool hasEmptyItem) {
            ListControl = control;
            this.hasEmptyItem = hasEmptyItem;
        }

        public ListControl ListControl { get; private set; }

        public abstract object Source { get; }

        public abstract string DropDownListEmptyItemText { get; }

        public abstract string TextField { get; }

        public abstract string ValueField { get; }

        public string EmptyItemText {
            get {
                if (emptyItemText is null) {
                    return ListControl is DropDownList ? DropDownListEmptyItemText : "無";
                } else {
                    return emptyItemText;
                }
            }
            set => emptyItemText = value;
        }

        public void BindData() {
            ListControl.DataSource = Source;
            ListControl.DataTextField = TextField;
            ListControl.DataValueField = ValueField;
            ListControl.DataBind();

            if (hasEmptyItem) {
                ListControl.Items.Insert(0, new ListItem(EmptyItemText, ""));
            }
        }
    }
}
