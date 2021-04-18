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
    public partial class FSMNode
    {
        [NonSerialized]
        public Point Poisiton;

        public string Name;
    }

    public class FSMState: FSMNode
    {

    }

    public partial class FSMTransition
    { 

    }


    public class MainCanvasContext: INotifyPropertyChanged
    {
        public ObservableCollection<FSMNode> Nodes = new();

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
            m_MainCanvasContext.Nodes.Insert(node);
        }

        void RemoveNode(FSMNode node)
        {
            m_MainCanvasContext.Nodes.Remove(node);
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine(sender);
            ContextMenu cm = FindResource("MainCtxMenu") as ContextMenu;
            Debug.WriteLine(cm);
            cm.IsOpen = true;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("What");
        }
    }
}
