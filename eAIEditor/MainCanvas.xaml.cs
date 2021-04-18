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
    public class MainCanvasContext: INotifyPropertyChanged
    {
        public ObservableCollection<FSMNode> Nodes = new ObservableCollection<FSMNode>();

        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            DataContext = m_MainCanvasContext = new MainCanvasContext();
        }

        void InsertNode(FSMNode node)
        {
            m_MainCanvasContext.Nodes.Add(node);
            CanvasView.Children.Add(node);
        }

        void RemoveNode(FSMNode node)
        {
            m_MainCanvasContext.Nodes.Remove(node);
            CanvasView.Children.Remove(node);
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = FindResource("MainCtxMenu") as ContextMenu;
            cm.IsOpen = true;
        }

        private void AddNode_Click(object sender, RoutedEventArgs e)
        {

            FSMNodeData node_data = new FSMNodeData {
                Name = "Test"
            };

            FSMNode node = new FSMNode(node_data);
            InsertNode(node);

            MenuItem item = sender as MenuItem;
            
            node.SetPosition(item.RenderTransformOrigin);
        }
    }
}
