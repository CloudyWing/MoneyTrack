using System.Windows;

namespace CloudyWing.MoneyTrack.Tools.EntityGenerator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly SchemaPage schemaPage;
        private readonly SqlPage sqlPage;

        public MainWindow(SchemaPage schemaPage, SqlPage sqlPage) {
            InitializeComponent();

            this.schemaPage = schemaPage;
            this.sqlPage = sqlPage;
            Main.Content = schemaPage;
        }

        private void SchemaPageClick(object sender, RoutedEventArgs e) {
            Main.Content = schemaPage;
        }

        private void SqlPageClick(object sender, RoutedEventArgs e) {
            Main.Content = sqlPage;
        }
    }
}
