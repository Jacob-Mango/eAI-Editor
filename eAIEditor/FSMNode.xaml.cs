using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eAIEditor
{

    public class FSMNode : INotifyPropertyChanged
    {
        public Point Position {
            set {
                View.SetPosition(value);
                OnPropertyChanged();
            }
            get => View.GetPosition();
        }

        private string _name;
        public string Name {
            set {
                _name = value;
                OnPropertyChanged();
            }
            get => _name;
        }

        public FSMNodeView View { get; protected set; }

        public FSMNode()
        {
            View = new FSMNodeView(this);
        }

        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public partial class FSMNodeView : UserControl
    {
        public static readonly Brush BRUSH_DEFAULT = Brushes.Black;
        public static readonly Brush BRUSH_HIGHLIGHT = Brushes.CadetBlue;
        public static readonly Brush BRUSH_SELECTED = Brushes.White;

        protected FSMNode m_Node;

        public FSMNodeView(FSMNode node)
        {
            Debug.WriteLine("Creating Node...");
            InitializeComponent();
            DataContext = m_Node = node;
        }

        public void SetPosition(Point position)
        {
            Canvas.SetLeft(this, (double)position.X);
            Canvas.SetTop(this, (double)position.Y);
        }

        public Point GetPosition() => new Point { X = Canvas.GetLeft(this), Y = Canvas.GetTop(this) };

        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            HighlightBorder.BorderBrush = BRUSH_HIGHLIGHT;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            HighlightBorder.BorderBrush = BRUSH_DEFAULT;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) {
                //DragMove();
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed) {
                //SetPosition(e.GetPosition());
            }
        }
    }
}
