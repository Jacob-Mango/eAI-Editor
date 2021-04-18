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
        [NonSerialized]
        public MainCanvas MainCanvas; //! temp hack fix, pls no kill

        [NonSerialized]
        public Point Position;

        [NonSerialized]
        private string _name;
        public string Name
        {
            set
            {
                _name = value;
                OnPropertyChanged();
            }
            get => _name;
        }

        public float X
        {
            set
            {
                Position.X = value;
                OnPropertyChanged();
            }
            get => (float)Position.X;
        }

        public float Y
        {
            set
            {
                Position.Y = value;
                OnPropertyChanged();
            }
            get => (float)Position.Y;
        }

        public float Width
        {
            set
            {
            }
            get => (float)150;
        }

        public float Height
        {
            set
            {
                Position.Y = value;
                OnPropertyChanged();
            }
            get => (float)40;
        }

        [NonSerialized]
        private string _eventEntry;
        public string EventEntry
        {
            set { _eventEntry = value; OnPropertyChanged(); }
            get { return _eventEntry; }
        }

        [NonSerialized]
        private string _eventExit;
        public string EventExit
        {
            set { _eventExit = value; OnPropertyChanged(); }
            get { return _eventExit; }
        }

        [NonSerialized]
        private string _eventUpdate;
        public string EventUpdate
        {
            set { _eventUpdate = value; OnPropertyChanged(); }
            get { return _eventUpdate; }
        }

        public List<FSMTransition> Transitions = new List<FSMTransition>();

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
        protected bool m_Handling;

        public FSMStateView(FSMState node)
        {
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

            if (e.ChangedButton == MouseButton.Left && !m_State.MainCanvas.m_HandlingSomething)
            {
                m_DragPoint = e.GetPosition(this);
                m_Handling = m_State.MainCanvas.m_HandlingSomething = true;
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left && m_Handling)
            {
                m_Handling = m_State.MainCanvas.m_HandlingSomething = false;
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed && m_Handling)
            {
                Point position = e.GetPosition(Parent as FrameworkElement);

                m_State.Position.X = position.X - m_DragPoint.X;
                m_State.Position.Y = position.Y - m_DragPoint.Y;

                Canvas.SetLeft(this, m_State.Position.X);
                Canvas.SetTop(this, m_State.Position.Y);

                foreach (FSMTransition transition in m_State.Transitions)
                {
                    transition.UpdateAbsoluteSource();
                    transition.UpdateAbsoluteDestination();
                }
            }
        }

        private void FSMNodeView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Debug.WriteLine(e.Effects);
        }
    }
}
