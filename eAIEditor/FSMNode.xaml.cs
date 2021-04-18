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
    // Serialized node
    [Serializable]
    public class FSMState : INotifyPropertyChanged
    {
        public Point Position;

        [NonSerialized]
        private string _name;
        public string Name {
            set {
                _name = value;
                OnPropertyChanged();
            }
            get => _name;
        }

        public FSMStateView View { get; protected set; }

        public FSMState()
        {
            View = new FSMStateView(this);
        }

        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public partial class FSMStateView : UserControl
    {
        public static readonly Brush BRUSH_DEFAULT = Brushes.Black;
        public static readonly Brush BRUSH_HIGHLIGHT = Brushes.CadetBlue;
        public static readonly Brush BRUSH_SELECTED = Brushes.White;

        protected FSMState m_State;

        protected Point m_DragPoint;

        public FSMStateView(FSMState node)
        {
            Debug.WriteLine("Creating Node...");
            InitializeComponent();
            DataContext = m_State = node;

            GiveFeedback += FSMNodeView_GiveFeedback;
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
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                m_DragPoint = e.GetPosition(this);
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(Parent as FrameworkElement);

                m_State.Position.X = position.X - m_DragPoint.X;
                m_State.Position.Y = position.Y - m_DragPoint.Y;

                Canvas.SetLeft(this, m_State.Position.X);
                Canvas.SetTop(this, m_State.Position.Y);
            }
        }

        private void NodeRight_Click(object sender, RoutedEventArgs e)
        {

        }        
        
        private void NodeLeft_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FSMNodeView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Debug.WriteLine(e.Effects);
        }
    }
}
