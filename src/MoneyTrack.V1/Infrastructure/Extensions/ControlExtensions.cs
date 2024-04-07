using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudyWing.MoneyTrack.Infrastructure.Extensions {
    /// <summary>
    /// Provides extension methods for the <see cref="Control"/> class.
    /// </summary>
    public static class ControlExtensions {
        /// <summary>
        /// Clears the values of the specified controls and their child controls.
        /// </summary>
        /// <param name="controls">The controls to clear.</param>
        public static void ClearValues(this IEnumerable<Control> controls) {
            foreach (dynamic control in controls) {
                control.ClearValue();
            }
        }

        /// <summary>
        /// Clears the value of the specified control and its child controls.
        /// </summary>
        /// <param name="control">The control to clear.</param>
        public static void ClearValue(this Control control) {
            foreach (dynamic c in control.Controls) {
                c.ClearValue();
            }
        }

        /// <summary>
        /// Clears the value of the specified TextBox control.
        /// </summary>
        /// <param name="textBox">The TextBox control to clear.</param>
        public static void ClearValue(this TextBox textBox) {
            textBox.Text = string.Empty;
        }

        /// <summary>
        /// Clears the selected value(s) of the specified ListControl control.
        /// </summary>
        /// <param name="listControl">The ListControl control to clear.</param>
        public static void ClearValue(this ListControl listControl) {
            foreach (ListItem li in listControl.Items) {
                li.Selected = false;
            }
        }

        /// <summary>
        /// Clears the values of the specified Table control and its child controls.
        /// </summary>
        /// <param name="table">The Table control to clear.</param>
        public static void ClearValue(this Table table) {
            foreach (TableRow tr in table.Rows) {
                foreach (TableCell tc in tr.Cells) {
                    foreach (dynamic control in tc.Controls) {
                        control.ClearValue();
                    }
                }
            }
        }

        /// <summary>
        /// Clears the value of the specified CheckBox control.
        /// </summary>
        /// <param name="checkBox">The CheckBox control to clear.</param>
        public static void ClearValue(this CheckBox checkBox) {
            checkBox.Checked = false;
        }

    }
}
