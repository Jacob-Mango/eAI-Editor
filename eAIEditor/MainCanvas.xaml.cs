using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace eAIEditor
{
    public class MainCanvasContext : INotifyPropertyChanged
    {
        public ObservableCollection<FSMState> States { get; } = new ObservableCollection<FSMState>();
        public ObservableCollection<FSMTransition> Transitions { get; } = new ObservableCollection<FSMTransition>();

        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    /// <summary>
    /// Interaction logic for MainCanvas.xaml
    /// </summary>
    public partial class MainCanvas : Window
    {
        protected ScaleTransform m_ScaleTransform;
        protected MainCanvasContext m_MainCanvasContext;

        public bool m_HandlingSomething;

        public MainCanvas()
        {
            InitializeComponent();
            m_MainCanvasContext = DataContext as MainCanvasContext;

            m_ScaleTransform = new ScaleTransform();
            NodeCanvasView.RenderTransform = m_ScaleTransform;
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            double ScaleRate = 1.1;

            if (e.Delta > 0) {
                m_ScaleTransform.ScaleX *= ScaleRate;
                m_ScaleTransform.ScaleY *= ScaleRate;
            }
            else {
                m_ScaleTransform.ScaleX /= ScaleRate;
                m_ScaleTransform.ScaleY /= ScaleRate;
            }

            NodeCanvasView.Width = NodeCanvasView.ActualWidth / m_ScaleTransform.ScaleX;
            NodeCanvasView.Height = NodeCanvasView.ActualHeight / m_ScaleTransform.ScaleY;
        }

        public void InsertNode(FSMState node)
        {
            node.MainCanvas = this;

            m_MainCanvasContext.States.Add(node);
            NodeCanvasView.Children.Add(node.View);
        }

        public void RemoveNode(FSMState node)
        {
            m_MainCanvasContext.States.Remove(node);
            NodeCanvasView.Children.Remove(node.View);
        }

        public void AddTransition(FSMTransition transition)
        {
            transition.MainCanvas = this;

            m_MainCanvasContext.Transitions.Add(transition);
            NodeCanvasView.Children.Add(transition.View);
        }

        public void RemoveTransition(FSMTransition transition)
        {
            m_MainCanvasContext.Transitions.Remove(transition);
            NodeCanvasView.Children.Remove(transition.View);
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = FindResource("MainCtxMenu") as ContextMenu;
            cm.IsOpen = true;
        }

        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            FSMState node = new FSMState {
                Name = "state",
                Position = item.PointToScreen(new Point())
            };

            Debug.WriteLine(item.PointToScreen(new Point()));
            InsertNode(node);

            // testing
            if (m_MainCanvasContext.States.Count() == 2) {
                Debug.WriteLine("Hi");
                FSMTransition transition = new FSMTransition() {
                    Source = m_MainCanvasContext.States[0],
                    Destination = m_MainCanvasContext.States[1]
                };

                AddTransition(transition);
            }
        }
    }
}
