using System.Windows.Controls;
using CloudyWing.MoneyTrack.Tools.EntityGenerator.ViewModels;

namespace CloudyWing.MoneyTrack.Tools.EntityGenerator {
    /// <summary>
    /// SchemaPage.xaml 的互動邏輯
    /// </summary>
    public partial class SchemaPage : Page {
        public SchemaPage(SchemaPageViewModel viewModel) {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
