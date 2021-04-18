using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace eAIEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /*public partial class MainWindow : Window
    {
        Nullable<Point> dragStart = null;

        MainWindowViewModel VM;

        ViewModelBase Selected;

        ScaleTransform st;

        FSMTransition currentTransition;
        FSMState currentState;

        FSMState hoveringState;

        public MainWindow()
        {
            InitializeComponent();

            VM = (MainWindowViewModel)DataContext;

            st = new ScaleTransform();
            canvas.RenderTransform = st;
        }

        void New(object target, ExecutedRoutedEventArgs e)
        {
            MainWindowViewModel model = (MainWindowViewModel)DataContext;
            model.New();
        }

        void CanNew(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void Open(object target, ExecutedRoutedEventArgs e)
        {
            MainWindowViewModel model = (MainWindowViewModel)DataContext;

            OpenFileDialog dialog = new OpenFileDialog {
                InitialDirectory = "P:\\eai\\Scripts\\FSM",
                Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == true) {
                model.Load(dialog.FileName);
            }
        }

        void CanOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void Save(object target, ExecutedRoutedEventArgs e)
        {
            MainWindowViewModel model = (MainWindowViewModel)DataContext;

            if (model.NeedPath()) {
                SaveAs(target, e);
                return;
            }

            model.Save("");
        }

        void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void SaveAs(object target, ExecutedRoutedEventArgs e)
        {
            MainWindowViewModel model = (MainWindowViewModel)DataContext;

            SaveFileDialog dialog = new SaveFileDialog {
                InitialDirectory = "P:\\eai\\Scripts\\FSM",
                Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == true) {
                model.Save(dialog.FileName);
            }
        }

        void CanSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        const double ScaleRate = 1.1;
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) {
                st.ScaleX *= ScaleRate;
                st.ScaleY *= ScaleRate;
            }
            else {
                st.ScaleX /= ScaleRate;
                st.ScaleY /= ScaleRate;
            }

            canvas.Width = graphArea.ActualWidth / st.ScaleX;
            canvas.Height = graphArea.ActualHeight / st.ScaleY;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            dragStart = e.GetPosition(graphArea);
            element.CaptureMouse();

            switch (element.DataContext.GetType()) {
                // case typeof(FSMState):
                //     break;
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //var element = (FrameworkElement)sender;
            //dragStart = null;
            //element.ReleaseMouseCapture();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentState == null && dragStart != null && e.LeftButton == MouseButtonState.Pressed) {
                var p2 = e.GetPosition(graphArea);

                //p2.X -= dragStart.Value.X;
                //p2.Y -= dragStart.Value.Y;

                //p2.X += Canvas.GetLeft(canvas);
                //p2.Y += Canvas.GetTop(canvas);

                //Canvas.SetLeft(canvas, p2.X);
                //Canvas.SetTop(canvas, p2.Y);
            }
        }

        void UpdateSelected(ViewModelBase selected)
        {
            Selected = selected;

            if (Selected != null) {
                if (Selected.GetType() == typeof(FSMState)) {
                    FSMState state = (FSMState)Selected;

                    Control_State.Visibility = Visibility.Visible;
                    Control_Transition.Visibility = Visibility.Hidden;

                    Control_State_EventEntry.Text = state.EventEntry;
                    Control_State_EventExit.Text = state.EventExit;
                    Control_State_EventUpdate.Text = state.EventUpdate;
                    return;
                }

                if (Selected.GetType() == typeof(FSMTransition)) {
                    FSMTransition transition = (FSMTransition)Selected;

                    Control_State.Visibility = Visibility.Hidden;
                    Control_Transition.Visibility = Visibility.Visible;

                    Control_Transition_Event.Text = transition.Event;
                    Control_Transition_From.Text = transition.Source != null ? transition.Source.Name : "";
                    Control_Transition_To.Text = transition.Destination != null ? transition.Destination.Name : "";
                    Control_Transition_Guard.Text = transition.Guard;
                    return;
                }
            }

            Control_State.Visibility = Visibility.Hidden;
            Control_Transition.Visibility = Visibility.Hidden;
        }


        private void State_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            dragStart = e.GetPosition(element);
            element.CaptureMouse();

            currentState = (FSMState)element.DataContext;
            currentState.Selected = true;

            UpdateSelected(currentState);
        }

        private void State_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            dragStart = null;
            currentState = null;
            element.ReleaseMouseCapture();
        }

        private void State_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentState != null && dragStart != null && e.LeftButton == MouseButtonState.Pressed) {
                var p2 = e.GetPosition(canvas);
                if (currentState == null) return;

                currentState.X = p2.X - dragStart.Value.X;
                currentState.Y = p2.Y - dragStart.Value.Y;
            }
        }

        private void TransitionSource_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            Point p = e.GetPosition(element);
            p.X -= 6;
            p.Y -= 6;
            dragStart = p;
            element.CaptureMouse();

            currentTransition = (FSMTransition)element.DataContext;
            currentTransition.Selected = true;
            currentTransition.DraggingSource = true;
            currentTransition.DraggingDestination = false;

            UpdateSelected(currentTransition);
        }

        private void TransitionSource_MouseUp(object sender, MouseButtonEventArgs e)
        {
            currentTransition.DraggingSource = false;
            currentTransition.StateChanged();

            var element = (FrameworkElement)sender;
            dragStart = null;
            hoveringState = null;
            currentTransition = null;
            element.ReleaseMouseCapture();
        }

        private void TransitionSource_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentTransition != null && dragStart != null && e.LeftButton == MouseButtonState.Pressed) {
                var p2 = e.GetPosition(canvas);
                if (currentTransition == null) return;

                var state = currentTransition.Source;

                if (state != null) {
                    if (state == currentTransition.Destination) currentTransition.Source = null;
                    else if (p2.X + 50 < state.X) currentTransition.Source = null;
                    else if (p2.Y + 50 < state.Y) currentTransition.Source = null;
                    else if (p2.X + state.Width - 50 > state.X) currentTransition.Source = null;
                    else if (p2.Y + state.Height - 50 > state.Y) currentTransition.Source = null;
                }

                hoveringState = null;

                VisualTreeHelper.HitTest(canvas, null, new HitTestResultCallback(StateResult), new PointHitTestParameters(p2));

                currentTransition.Source = hoveringState;

                if (currentTransition.Source == null) {
                    currentTransition.SrcX = p2.X - dragStart.Value.X;
                    currentTransition.SrcY = p2.Y - dragStart.Value.Y;
                    return;
                }

                currentTransition.UpdateAbsoluteSource();
            }
        }

        private void TransitionDestination_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            Point p = e.GetPosition(element);
            p.X -= 6;
            p.Y -= 6;
            dragStart = p;
            element.CaptureMouse();

            currentTransition = (FSMTransition)element.DataContext;
            currentTransition.Selected = true;
            currentTransition.DraggingDestination = true;
            currentTransition.DraggingSource = false;

            UpdateSelected(currentTransition);
        }

        private void TransitionDestination_MouseUp(object sender, MouseButtonEventArgs e)
        {
            currentTransition.DraggingDestination = false;
            currentTransition.StateChanged();

            var element = (FrameworkElement)sender;
            dragStart = null;
            hoveringState = null;
            currentTransition = null;
            element.ReleaseMouseCapture();

        }

        private void TransitionDestination_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentTransition != null && dragStart != null && e.LeftButton == MouseButtonState.Pressed) {
                var p2 = e.GetPosition(canvas);
                if (currentTransition == null) return;

                var state = currentTransition.Destination;

                if (state != null) {
                    if (state == currentTransition.Source) currentTransition.Destination = null;
                    else if (p2.X + 50 < state.X) currentTransition.Destination = null;
                    else if (p2.Y + 50 < state.Y) currentTransition.Destination = null;
                    else if (p2.X + state.Width - 50 > state.X) currentTransition.Destination = null;
                    else if (p2.Y + state.Height - 50 > state.Y) currentTransition.Destination = null;
                }

                hoveringState = null;

                VisualTreeHelper.HitTest(canvas, null, new HitTestResultCallback(StateResult), new PointHitTestParameters(p2));

                currentTransition.Destination = hoveringState;

                if (currentTransition.Destination == null) {
                    currentTransition.DstX = p2.X - dragStart.Value.X;
                    currentTransition.DstY = p2.Y - dragStart.Value.Y;
                    return;
                }

                currentTransition.UpdateAbsoluteDestination();
            }
        }

        public HitTestResultBehavior StateResult(HitTestResult result)
        {
            var element = (FrameworkElement)result.VisualHit;
            if (element.DataContext.GetType() == typeof(FSMState))
                hoveringState = (FSMState)element.DataContext;

            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
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
    }*/
}