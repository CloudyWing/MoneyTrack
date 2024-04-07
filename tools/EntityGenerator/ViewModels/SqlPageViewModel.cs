using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using CloudyWing.MoneyTrack.Tools.EntityGenerator.Options;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace CloudyWing.MoneyTrack.Tools.EntityGenerator.ViewModels {
    public partial class SqlPageViewModel : ObservableObject {
        private static readonly Dictionary<Type, string> TypeAliases = new() {
            [typeof(int)] = "int",
            [typeof(short)] = "short",
            [typeof(byte)] = "byte",
            [typeof(byte[])] = "byte[]",
            [typeof(long)] = "long",
            [typeof(double)] = "double",
            [typeof(decimal)] = "decimal",
            [typeof(float)] = "float",
            [typeof(bool)] = "bool",
            [typeof(string)] = "string"
        };

        private static readonly HashSet<Type> NullableTypes = new() {
            typeof(int),
            typeof(short),
            typeof(long),
            typeof(double),
            typeof(decimal),
            typeof(float),
            typeof(bool),
            typeof(DateTime)
        };

        private readonly IOptionsMonitor<AppOptions> appOptions;
        private readonly string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Codes", "FromSQL");

        [ObservableProperty]
        private string? _namespace;

        [ObservableProperty]
        private string? className;

        [ObservableProperty]
        private string? sql;

        public SqlPageViewModel(IOptionsMonitor<AppOptions> appOptions) {
            this.appOptions = appOptions;
            Namespace = appOptions.CurrentValue.DefaultNamespaceForSql;

            appOptions.OnChange(x => {
                Namespace = appOptions.CurrentValue.DefaultNamespaceForSql;
            });

            Directory.CreateDirectory(outputPath);
        }

        [RelayCommand]
        private async Task SubmitAsync() {
            try {
                using SqlConnection conn = new(appOptions.CurrentValue.ConnectionString);
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = Sql;
                SqlDataReader reader = cmd.ExecuteReader();

                do {
                    if (reader.FieldCount <= 1) {
                        continue;
                    }

                    StringBuilder builder = new();
                    builder.AppendFormat("namespace {0}{1} {", Namespace, Environment.NewLine);
                    builder.AppendFormat("    public class {0}{1} {", ClassName, Environment.NewLine);
                    DataTable schema = reader.GetSchemaTable();

                    int i = 0;
                    foreach (DataRow row in schema.Rows) {
                        Type type = (Type)row["DataType"];
                        string name = TypeAliases.ContainsKey(type) ? TypeAliases[type] : type.Name;
                        bool isNullable = (bool)row["AllowDBNull"] && NullableTypes.Contains(type);
                        string columnName = FormatWord((string)row["ColumnName"]);

                        builder.AppendLine(string.Format("        public {0}{1} {2} {{ get; set; }}", name, isNullable ? "?" : string.Empty, columnName));

                        if (++i < schema.Rows.Count) {
                            builder.AppendLine();
                        }
                    }

                    builder.AppendLine("    }");
                    builder.AppendLine("}");

                    await File.WriteAllTextAsync(Path.Combine(outputPath, ClassName + ".cs"), builder.ToString());
                } while (reader.NextResult());

                Namespace = appOptions.CurrentValue.DefaultNamespaceForSql;

                MessageBox.Show("檔案產生成功。");
            } catch (Exception ex) {
                MessageBox.Show($"檔案產生失敗，{ex.Message}。");
            }
        }

        private static string FormatWord(string str) {
            if (Regex.IsMatch(str, @"[A-Z]") && Regex.IsMatch(str, @"[a-z]")) {
                return str;
            }

            IEnumerable<string> temps = str.Split('_').Select(x => ToCamelCase(x));

            return string.Join("_", temps);
        }

        private static string ToCamelCase(string str) {
            return str[..1].ToUpper() + str[1..].ToLower();
        }
    }
}
