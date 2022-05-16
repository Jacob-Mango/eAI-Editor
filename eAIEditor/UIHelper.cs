using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace eAIEditor
{
    public static class EditorHelper
    {
        public static bool IsPointWithin(Point p, Point min, Point max)
        {
            if (p.X < min.X) return false;
            if (p.X > max.X) return false;
            if (p.Y < min.Y) return false;
            if (p.Y > max.Y) return false;

            return true;
        }

        public static Point ClampPoint(Point P, Point center, Point bounds)
        {
            Vector _P = new Vector(P.X, P.Y);
            Vector _center = new Vector(center.X, center.Y);
            Vector _bounds = new Vector(bounds.X, bounds.Y);

            _center += _bounds * 0.5;
            _bounds *= 0.5;

            Vector result = ClampVector(_P, _center, _bounds);
            return new Point(result.X, result.Y);
        }

        public static Vector ClampVector(Vector P, Vector center, Vector bounds)
        {
            Vector closestPoint = new Vector();
            bool insideX = center.X - bounds.X < P.X && P.X < center.X + bounds.X;
            bool insideY = center.Y - bounds.Y < P.Y && P.Y < center.Y + bounds.Y;
            bool pointInsideRectangle = insideX && insideY;

            if (!pointInsideRectangle)
            {
                closestPoint.X = Math.Max(center.X - bounds.X, Math.Min(P.X, center.X + bounds.X));
                closestPoint.Y = Math.Max(center.Y - bounds.Y, Math.Min(P.Y, center.Y + bounds.Y));
            }
            else
            {
                Vector distanceToPositiveBounds = center + bounds - P;
                Vector distanceToNegativeBounds = -(center - bounds - P);
                double smallestX = Math.Min(distanceToPositiveBounds.X, distanceToNegativeBounds.X);
                double smallestY = Math.Min(distanceToPositiveBounds.Y, distanceToNegativeBounds.Y);
                double smallestDistance = Math.Min(smallestX, smallestY);

                if (smallestDistance == distanceToPositiveBounds.X)
                {
                    closestPoint.X = center.X + bounds.X;
                    closestPoint.Y = P.Y;
                }
                else if (smallestDistance == distanceToNegativeBounds.X)
                {
                    closestPoint.X = center.X - bounds.X;
                    closestPoint.Y = P.Y;
                }
                else if (smallestDistance == distanceToPositiveBounds.Y)
                {
                    closestPoint.X = P.X;
                    closestPoint.Y = center.Y + bounds.Y;
                }
                else
                {
                    closestPoint.X = P.X;
                    closestPoint.Y = center.Y - bounds.Y;
                }
            }
            return closestPoint;
        }
    }

    public static class EditorCommands
    {
        public static readonly RoutedUICommand SaveAll = new RoutedUICommand("SaveAll", "SaveAll", typeof(EditorCommands));
        public static readonly RoutedUICommand AddState = new RoutedUICommand("AddState", "AddState", typeof(EditorCommands));
        public static readonly RoutedUICommand RemoveState = new RoutedUICommand("RemoveState", "RemoveState", typeof(EditorCommands));

        static EditorCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(EditorCommands), new CommandBinding(SaveAll));
            CommandManager.RegisterClassCommandBinding(typeof(EditorCommands), new CommandBinding(AddState));
            CommandManager.RegisterClassCommandBinding(typeof(EditorCommands), new CommandBinding(RemoveState));
        }
    }

    public class UIHelper
    {
        public static T FindChild<T>(DependencyObject parent, string childName)
      where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null)
            {
                return null;
            }

            //check if the parent matches the type we're looking for
            return parentObject is T parent ? parent : FindParent<T>(parentObject);
        }
    }

    public class BooleanVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((bool)value)) return Visibility.Visible;

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertBooleanVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!((bool)value)) return Visibility.Visible;

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void UpdateSelected()
        {
        }
    }
}
