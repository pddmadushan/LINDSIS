using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public static void searchGridByKey(DataGrid dg, string searchKey, string sarchValue)
        {
            int gridRow = dg.Items.Count;
            if (gridRow > 0)
            {
                for (int i = 0; i < gridRow; i++)
                {
                    var selectedRow = (System.Data.DataRowView)dg.Items[i];
                    string cellContent = selectedRow[searchKey].ToString();
                    try
                    {
                        int FirstChr = cellContent.ToUpper().IndexOf(sarchValue.ToUpper());
                        if (FirstChr != -1)
                        {
                            object item = dg.Items[i];
                            dg.SelectedItem = item;
                            dg.ScrollIntoView(item);
                            break;
                        }
                        else
                        {
                            dg.UnselectAll();
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

    }
}
