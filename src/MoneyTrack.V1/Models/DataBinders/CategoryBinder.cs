using System.Data;
using System.Web.UI.WebControls;
using CloudyWing.MoneyTrack.Models.Queriers;

namespace CloudyWing.MoneyTrack.Models.DataBinders {
    public class CategoryBinder : ListDataBinder {
        private DataTable source;
        private static readonly CategoryModel model = CategoryModel.Instance;

        public CategoryBinder(ListControl control, bool hasEmptyItem) : base(control, hasEmptyItem) { }

        public override object Source {
            get {
                if (source is null) {
                    CategoryModel model = CategoryModel.Instance;
                    SingleTableQuerier querier = new SingleTableQuerier(model) {
                        OrderBy = model.DisplayOrder.NameWithBrackets
                    };

                    source = querier.GetDataTable();
                }

                return source;
            }
        }

        public override string DropDownListEmptyItemText => "請選擇分類";

        public override string TextField => model.Name.Name;

        public override string ValueField => model.Id.Name;
    }
}
