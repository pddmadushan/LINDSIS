using System.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace CMSXtream
{
    public class Helper
    {
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;

                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);

                    if (foundChild != null) break;
                }
                else
                    if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;

                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                    else
                    {
                        foundChild = FindChild<T>(child, childName);

                        if (foundChild != null)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        private static int _lastFoundIndex = -1;
        private static string _lastSearchText = "";

        public static void searchGridByKey(DataGrid dg, string searchKey, string searchValue)
        {
            if (dg.Items.Count == 0 || string.IsNullOrWhiteSpace(searchValue))
                return;

            // Reset search position if search text changed
            if (!_lastSearchText.Equals(searchValue, StringComparison.OrdinalIgnoreCase))
            {
                _lastFoundIndex = -1;
                _lastSearchText = searchValue;
            }

            int startIndex = _lastFoundIndex + 1;

            // Wrap around
            if (startIndex >= dg.Items.Count)
                startIndex = 0;

            for (int loop = 0; loop < dg.Items.Count; loop++)
            {
                int i = (startIndex + loop) % dg.Items.Count;

                if (dg.Items[i] is DataRowView row)
                {
                    string cellContent = row[searchKey]?.ToString() ?? "";

                    if (cellContent.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        object item = dg.Items[i];

                        dg.SelectedItem = item;
                        dg.ScrollIntoView(item);

                        _lastFoundIndex = i;
                        return;
                    }
                }
            }
        }

        public static void ApplySearchFilter(
        TextBox txtBox,
        DataGrid grid,
        params string[] columns)
        {
            if (txtBox == null || grid == null || columns == null || columns.Length == 0)
                return;

            string search = txtBox.Text.Replace("'", "''");

            DataView dv = grid.ItemsSource as DataView;

            if (dv != null)
            {
                string filter = string.Join(" OR ",
                    columns.Select(col =>
                        $"Convert([{col}], 'System.String') LIKE '%{search}%'"));

                dv.RowFilter = filter;
            }
        }

        public static void ApplyInfoSearchFilter(TextBox txtBox, DataGrid grid)
        {
            if (txtBox == null || grid == null)
                return;

            DataView dv = grid.ItemsSource as DataView;

            if (dv == null)
                return;

            string searchText = txtBox.Text.Trim();

            // Clear filter if empty
            if (string.IsNullOrWhiteSpace(searchText))
            {
                dv.RowFilter = string.Empty;
                return;
            }

            // Split by spaces
            string[] words = searchText
                .Split(' ')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            string[] columns =
            {
            "bandscore",
            "exam_type",
            "when_do_you_want",
            "your_english_knowledge",
            "your_ielts_knowledge"
        };

            // Each word must match at least one column
            var wordFilters = words.Select(word =>
            {
                string safeWord = word.Replace("'", "''");

                return "(" + string.Join(" OR ",
                    columns.Select(col =>
                        $"Convert([{col}], 'System.String') LIKE '%{safeWord}%'"))
                    + ")";
            });

            // Combine all words using AND
            dv.RowFilter = string.Join(" AND ", wordFilters);
        }

    }
}
