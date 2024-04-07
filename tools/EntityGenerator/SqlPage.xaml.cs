using System.Windows.Controls;
using CloudyWing.MoneyTrack.Tools.EntityGenerator.ViewModels;

namespace CloudyWing.MoneyTrack.Tools.EntityGenerator {
    /// <summary>
    /// SqlPage.xaml 的互動邏輯
    /// </summary>
    public partial class SqlPage : Page {
        public SqlPage(SqlPageViewModel viewModel) {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
