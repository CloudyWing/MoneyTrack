using System.Collections.Generic;

namespace System.Web.UI.WebControls {
    /// <summary>
    /// Provides extension methods for the <see cref="CheckBoxList"/> class.
    /// </summary>
    public static class CheckBoxListExtension {
        /// <summary>
        /// Gets a list of selected values in the CheckBoxList control.
        /// </summary>
        /// <param name="checkBoxList">The CheckBoxList control to get the selected values from.</param>
        /// <returns>A List of strings representing the selected values in the CheckBoxList control.</returns>
        public static List<string> GetSelectedValueList(this CheckBoxList checkBoxList) {
            List<string> list = new List<string>();
            foreach (ListItem li in checkBoxList.Items) {
                if (li.Selected) {
                    list.Add(li.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// Gets a string of selected values in the CheckBoxList control separated by the specified separator.
        /// </summary>
        /// <param name="checkBoxList">The CheckBoxList control to get the selected values from.</param>
        /// <returns>An ArrayList containing the selected values in the CheckBoxList control.</returns>
        public static string GetSelectedValueString(this CheckBoxList checkBoxList) {
            return string.Join(",", checkBoxList.GetSelectedValueList().ToArray());
        }

        /// <summary>
        /// Gets the selected values from the CheckBoxList as a comma-separated string.
        /// </summary>
        /// <param name="checkBoxList">The CheckBoxList control.</param>
        /// <param name="separator">The separator to use between the values.</param>
        /// <returns>A comma-separated string of selected values.</returns>
        public static string GetSelectedValueString(this CheckBoxList checkBoxList, string separator) {
            return string.Join(separator, checkBoxList.GetSelectedValueList().ToArray());
        }
    }
}
