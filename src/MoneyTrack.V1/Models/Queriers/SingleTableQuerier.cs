using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CloudyWing.MoneyTrack.Models.Enumerations;

namespace CloudyWing.MoneyTrack.Models.Queriers {
    /// <summary>
    /// Provides query functionality for a single data table through a <see cref="DataModel"/>.
    /// </summary>
    public sealed class SingleTableQuerier : MapTableQuerier {
        private DataModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleTableQuerier"/> class with the specified <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The data model that represents the data table.</param>
        public SingleTableQuerier(DataModel model) {
            Model = model;
        }

        /// <summary>
        /// Gets or sets the DataModel used by this SimpleTableProvider instance.
        /// </summary>
        public DataModel Model {
            set {
                model = value;
                ReSet();
            }
            get => model;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the query result should contain distinct rows.
        /// </summary>
        public bool IsDistinct { get; set; }

        /// <inheritdoc/>
        protected override void InitFields() {
            if (Model != null) {
                Fields.Clear();

                IEnumerable<TableField> tfs = Model.TableFields;
                foreach (TableField tf in tfs) {
                    Fields.Add(tf.Name, tf.NameWithBrackets);
                }
            } else {
                throw new ArgumentNullException(nameof(Model));
            }
        }

        /// <inheritdoc/>
        protected override void InitOrderBy() {
            if (Model != null) {
                List<string> orderByKeys = new List<string>();
                IEnumerable<TableField> tfs = Model.TableFields;

                foreach (TableField tf in tfs) {
                    if (tf.IsPrimaryKey) {
                        orderByKeys.Add(tf.Name);
                    }
                }
                if (orderByKeys.Count > 0) {
                    OrderBy = string.Join(", ", orderByKeys.ToArray());
                }
            }
        }

        /// <inheritdoc/>
        protected override SqlCommand GetSqlCommand(string field) {

            SqlWhereHelper wheres = new SqlWhereHelper();
            SqlCommand cmd = new SqlCommand();
            CreateBasicWhere(Model, wheres, cmd, Conditions);

            cmd.CommandText = string.Format(@"
                SELECT {0} {1}
                FROM {2}
                {3}
                ",
                IsDistinct ? "DISTINCT" : "",
                field,
                Model.TableName,
                wheres.WhereAndString
            );

            return cmd;
        }

        /// <inheritdoc/>
        public override void ReSet() {
            base.ReSet();
            IsDistinct = false;
        }

        /// <inheritdoc/>
        public override void AddCondition(string prefix, TableField field, CompareMode mode, IEnumerable<object> values) {
            CheckModel(model, field);

            base.AddCondition(prefix, field, mode, values);
        }
    }
}
