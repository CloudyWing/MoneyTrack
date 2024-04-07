namespace System.Web.UI.WebControls {
    /// <summary>
    /// Provides extension methods for the <see cref="Table"/> class.
    /// </summary>
    public static class TableExtension {
        /// <summary>
        /// Adds a new TableHeaderRow to the Table and returns it.
        /// </summary>
        /// <param name="table">The Table to which the new TableHeaderRow will be added.</param>
        /// <param name="cssClass">The CSS class to apply to the new TableHeaderRow.</param>
        /// <returns>The new TableHeaderRow.</returns>
        public static TableHeaderRow AddNewHeaderRow(this Table table, string cssClass = null) {
            TableHeaderRow tr = new TableHeaderRow {
                CssClass = cssClass
            };
            table.Rows.Add(tr);

            return tr;
        }

        /// <summary>
        /// Adds a new TableRow to the Table and returns it.
        /// </summary>
        /// <param name="table">The Table to which the new TableRow will be added.</param>
        /// <param name="cssClass">The CSS class to apply to the new TableRow.</param>
        /// <returns>The new TableRow.</returns>
        public static TableRow AddNewRow(this Table table, string cssClass = null) {
            TableRow tr = new TableRow {
                CssClass = cssClass
            };
            table.Rows.Add(tr);

            return tr;
        }

        /// <summary>
        /// Adds a new TableHeaderCell to the Table with a column span of 1 and a row span of 1, and returns it.
        /// </summary>
        /// <param name="table">The Table to which the new TableHeaderCell will be added.</param>
        /// <returns>The new TableHeaderCell.</returns>
        public static TableHeaderCell AddNewHeaderCell(
            this Table table
        ) {
            return table.AddNewHeaderCell(1);
        }

        /// <summary>
        /// Adds a new table header cell to the table with the specified column span and row span.
        /// </summary>
        /// <param name="table">The table to add the cell to.</param>
        /// <param name="colSpan">Optional number of columns the cell should span. Default is 1.</param>
        /// <param name="rowSpan">Optional number of rows the cell should span. Default is 1.</param>
        /// <param name="cssClass">Optional CSS class to apply to the cell.</param>
        /// <returns>The newly added table header cell.</returns>
        public static TableHeaderCell AddNewHeaderCell(
            this Table table, int colSpan = 1, int rowSpan = 1, string cssClass = null
        ) {
            int index = table.Rows.Count - 1;
            TableHeaderRow tr;
            if (index == -1) {
                tr = table.AddNewHeaderRow();
            } else {
                tr = table.Rows[index] as TableHeaderRow;
            }
            TableHeaderCell tc = new TableHeaderCell();
            if (colSpan > 1) {
                tc.ColumnSpan = colSpan;
            }
            if (rowSpan > 1) {
                tc.RowSpan = rowSpan;
            }
            tc.CssClass = cssClass;

            tr.Cells.Add(tc);

            return tc;
        }

        /// <summary>
        /// Adds a new <see cref="TableCell"/> to the last <see cref="TableRow"/> of the <see cref="Table"/>.
        /// </summary>
        /// <param name="table">The table to add the cell to.</param>
        /// <param name="align">The horizontal alignment of the cell.</param>
        /// <param name="valign">The vertical alignment of the cell.</param>
        /// <param name="colSpan">The number of columns the cell spans.</param>
        /// <param name="rowSpan">The number of rows the cell spans.</param>
        /// <param name="cssClass">The CSS class of the cell.</param>
        /// <returns>The newly created <see cref="TableCell"/>.</returns>
        public static TableCell AddNewCell(
            this Table table, HorizontalAlign align = HorizontalAlign.NotSet, VerticalAlign valign = VerticalAlign.NotSet,
            int colSpan = 1, int rowSpan = 1, string cssClass = null
        ) {
            int index = table.Rows.Count - 1;
            TableRow tr;
            if (index == -1) {
                tr = table.AddNewRow();
            } else {
                tr = table.Rows[index];
            }
            TableCell tc = new TableCell();
            if (colSpan > 1) {
                tc.ColumnSpan = colSpan;
            }
            if (rowSpan > 1) {
                tc.RowSpan = rowSpan;
            }
            tc.HorizontalAlign = align;
            tc.VerticalAlign = valign;
            tc.CssClass = cssClass;

            tr.Cells.Add(tc);

            return tc;
        }

        /// <summary>
        /// Adds a <see cref="Control"/> to the last <see cref="TableCell"/> of the last <see cref="TableRow"/> of the <see cref="Table"/>.
        /// </summary>
        /// <param name="table">The table to add the control to.</param>
        /// <param name="child">The control to add.</param>
        public static void AddContent(this Table table, Control child) {
            int rowIndex = table.Controls.Count - 1;
            TableRow tr;
            if (rowIndex == -1) {
                tr = table.AddNewRow();
            } else {
                tr = table.Rows[rowIndex];
            }
            int colIndex = tr.Controls.Count - 1;
            TableCell tc;
            if (colIndex == -1) {
                tc = table.AddNewCell();
            } else {
                tc = tr.Cells[colIndex];
            }
            tc.Controls.Add(child);
        }

        /// <summary>
        /// Adds a string to the last <see cref="TableCell"/> of the last <see cref="TableRow"/> of the <see cref="Table"/> as a <see cref="LiteralControl"/>.
        /// </summary>
        /// <param name="table">The table to add the string to.</param>
        /// <param name="child">The string to add.</param>
        public static void AddContent(this Table table, string child) {
            table.AddContent(new LiteralControl(child));
        }

        /// <summary>
        /// Gets the control with the specified ID, optionally specifying the row and column indices.
        /// </summary>
        /// <param name="table">The <see cref="Table"/> object.</param>
        /// <param name="id">The ID of the control to get.</param>
        /// <param name="rowIndex">The index of the row containing the control, or -1 if not specified.</param>
        /// <param name="colIndex">The index of the column containing the control, or -1 if not specified.</param>
        /// <returns>The control with the specified ID.</returns>
        public static Control GetControl(this Table table, string id, int rowIndex = -1, int colIndex = -1) {
            if (rowIndex == -1 && colIndex == -1) {
                return table.FindControl(id);
            } else if (colIndex == -1) {
                return table.Rows[rowIndex].FindControl(id);
            } else {
                return table.Rows[rowIndex].Cells[colIndex].FindControl(id);
            }
        }
    }
}
