using System.Collections.Generic;
using System.Data.SqlClient;
using CloudyWing.MoneyTrack.Models;
using CloudyWing.MoneyTrack.Models.Enumerations;
using CloudyWing.MoneyTrack.Models.Queriers;

public class RecordQuerier : MapTableQuerier {
    private const string RecordAlias = "Record";
    private const string CategoryAlias = "Category";

    protected override void InitFields() {
        Fields.Clear();

        IEnumerable<TableField> fields = RecordModel.TableFields;
        foreach (TableField field in fields) {
            Fields.Add(field.Name, $"{RecordAlias}.{field.NameWithBrackets}");
        }

        fields = CategoryModel.TableFields;
        foreach (TableField field in fields) {
            string fieldName = CategoryAlias + field.Name;
            if (!Fields.Contains(fieldName)) {
                Fields.Add(fieldName, $"{CategoryAlias}.{field.NameWithBrackets}");
            }
        }
    }

    protected override void InitOrderBy() {
        OrderBy = "Id";
    }

    protected override SqlCommand GetSqlCommand(string field) {

        SqlCommand cmd = new SqlCommand();

        SqlWhereHelper wheres = new SqlWhereHelper();

        CreateBasicWhere(RecordModel, wheres, cmd, Conditions, RecordAlias);
        CreateBasicWhere(CategoryModel, wheres, cmd, Conditions, CategoryAlias, CategoryAlias);

        string sql = $@"
            SELECT {field} FROM {RecordModel} {RecordAlias}
            INNER JOIN {CategoryModel} {CategoryAlias}
                ON {RecordAlias}.{RecordModel.CategoryId} = {CategoryAlias}.{CategoryModel.Id}
            {wheres.WhereAndString}";
        cmd.CommandText = sql.ToString();

        return cmd;
    }

    public void AddCategoryCondition(TableField field, CompareMode mode, IEnumerable<object> values) {
        AddCondition(CategoryAlias, field, mode, values);
    }
}
