using System;
using System.Data;

namespace CloudyWing.MoneyTrack.Models {
    public sealed class CategoryModel : DataModel {
        public static readonly CategoryModel Instance = new CategoryModel();

        private CategoryModel() {
            Id = new TableField(this, "Id", SqlDbType.BigInt, 19, 0, true, false, false, true);
            Name = new TableField(this, "Name", SqlDbType.NVarChar, 255, false, false, false);
            DisplayOrder = new TableField(this, "DisplayOrder", SqlDbType.BigInt, 19, 0, false, false, false, false);
        }

        public TableField Id { get; private set; }

        public TableField Name { get; private set; }

        public TableField DisplayOrder { get; private set; }
    
        public override string TableName => "Categories";
    }

    public sealed class RecordModel : DataModel {
        public static readonly RecordModel Instance = new RecordModel();

        private RecordModel() {
            Id = new TableField(this, "Id", SqlDbType.BigInt, 19, 0, true, false, false, true);
            CategoryId = new TableField(this, "CategoryId", SqlDbType.Int, 10, 0, false, false, false, false);
            RecordDate = new TableField(this, "RecordDate", SqlDbType.Date, false, false, false, false);
            IsIncome = new TableField(this, "IsIncome", SqlDbType.Bit, false, false, false, false);
            Amount = new TableField(this, "Amount", SqlDbType.BigInt, 19, 0, false, false, false, false);
            Description = new TableField(this, "Description", SqlDbType.NVarChar, -1, false, false, false);
        }

        public TableField Id { get; private set; }

        public TableField CategoryId { get; private set; }

        public TableField RecordDate { get; private set; }

        public TableField IsIncome { get; private set; }

        public TableField Amount { get; private set; }

        public TableField Description { get; private set; }
    
        public override string TableName => "Records";
    }

}
