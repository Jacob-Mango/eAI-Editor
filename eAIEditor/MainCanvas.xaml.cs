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
        public ObservableCollection<FSMNode> Nodes { get; } = new ObservableCollection<FSMNode>();
        public ObservableCollection<FSMTransition> Transitions { get; } = new ObservableCollection<FSMTransition>();

        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class FSMTransition
    {
        public FSMNode Node0;
        public FSMNode Node1;
    }

    /// <summary>
    /// Interaction logic for MainCanvas.xaml
    /// </summary>
    public partial class MainCanvas : Window
    {
        protected MainCanvasContext m_MainCanvasContext;

        public MainCanvas()
        {
            InitializeComponent();
            m_MainCanvasContext = DataContext as MainCanvasContext;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            foreach (FSMTransition transition in m_MainCanvasContext.Transitions) {
                if (transition.Node0 == null || transition.Node1 == null) {
                    continue;
                }

                drawingContext.DrawLine(new Pen(Brushes.Black, 2.0), transition.Node0.Position, transition.Node1.Position);
            }
        }

        public void InsertNode(FSMNode node)
        {
            m_MainCanvasContext.Nodes.Add(node);
            NodeCanvasView.Children.Add(node.View);
        }

        public void RemoveNode(FSMNode node)
        {
            m_MainCanvasContext.Nodes.Remove(node);
            NodeCanvasView.Children.Remove(node.View);
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = FindResource("MainCtxMenu") as ContextMenu;
            cm.IsOpen = true;
        }

        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            FSMNode node = new FSMNode {
                Name = "node0",
                Position = item.PointToScreen(new Point())
            };

            Debug.WriteLine(item.PointToScreen(new Point()));

            InsertNode(node);
        }
    }
}
