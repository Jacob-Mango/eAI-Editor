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
    public partial class FSMNodeData : INotifyPropertyChanged
    {
        public Point Poisiton;

        private string _name;
        public string Name {
            set {
                _name = value;
                OnPropertyChanged();
            }
            get => _name;
        }

        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class FSMState : FSMNodeData
    {

    }

    public partial class FSMTransition
    {

    }

    public partial class FSMNode : UserControl, INotifyPropertyChanged
    {
        public static readonly Brush BRUSH_DEFAULT = Brushes.Black;
        public static readonly Brush BRUSH_HIGHLIGHT = Brushes.CadetBlue;
        public static readonly Brush BRUSH_SELECTED = Brushes.White;

        private FSMNodeData _Data;
        public FSMNodeData Data {
            get => _Data;
            set {
                _Data = value;
                OnPropertyChanged();
            }
        }

        public FSMNode(FSMNodeData data)
        {
            Debug.WriteLine("Creating Node...");
            InitializeComponent();
            DataContext = Data = data;
        }

        public void SetPosition(Point position)
        {
            Data.Poisiton = position;
            Canvas.SetLeft(this, (double)position.X);
            Canvas.SetTop(this, (double)position.Y);
        }

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
                SetPosition(e.GetPosition());
            }
        }
    }
}
