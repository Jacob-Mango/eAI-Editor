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
    [Serializable]
    public class FSMTransition : INotifyPropertyChanged
    {
        [NonSerialized]
        public MainCanvas MainCanvas; //! temp hack fix, pls no kill

        private FSMState _source;
        public FSMState Source
        {
            get { return _source; }
            set
            {
                if (_source != null) _source.Transitions.Remove(this);
                _source = value;
                if (_source != null) _source.Transitions.Add(this);

                OnPropertyChanged();
            }
        }

        private FSMState _destination;
        public FSMState Destination
        {
            get { return _destination; }
            set
            {
                if (_destination != null) _destination.Transitions.Remove(this);
                _destination = value;
                if (_destination != null) _destination.Transitions.Add(this);

                OnPropertyChanged();
            }
        }

        private bool _DraggingSource;
        public bool DraggingSource
        {
            get { return _DraggingSource; }
            set { _DraggingSource = value; OnPropertyChanged(); }
        }

        private bool _DraggingDestination;
        public bool DraggingDestination
        {
            get { return _DraggingDestination; }
            set { _DraggingDestination = value; OnPropertyChanged(); }
        }

        private double _Src_RelX;
        public double RelSrcX
        {
            get { return _Src_RelX; }
            set { _Src_RelX = value < 0 ? 0 : value > 1 ? 0 : value; OnPropertyChanged(); }
        }

        private double _Src_AbsX;
        public double SrcX
        {
            get { return _Src_AbsX; }
            set { _Src_AbsX = value; OnPropertyChanged(); }
        }

        private double _Src_RelY;
        public double RelSrcY
        {
            get { return _Src_RelY; }
            set { _Src_RelY = value < 0 ? 0 : value > 1 ? 0 : value; OnPropertyChanged(); }
        }

        private double _Src_AbsY;

        public double SrcY
        {
            get { return _Src_AbsY; }
            set { _Src_AbsY = value; OnPropertyChanged(); }
        }

        private double _Dst_RelX;
        public double RelDstX
        {
            get { return _Dst_RelX; }
            set { _Dst_RelX = value < 0 ? 0 : value > 1 ? 0 : value; OnPropertyChanged(); }
        }

        private double _Dst_AbsX;
        public double DstX
        {
            get { return _Dst_AbsX; }
            set { _Dst_AbsX = value; OnPropertyChanged(); }
        }

        private double _Dst_RelY;
        public double RelDstY
        {
            get { return _Dst_RelY; }
            set { _Dst_RelY = value < 0 ? 0 : value > 1 ? 0 : value; OnPropertyChanged(); }
        }

        private double _Dst_AbsY;

        public double DstY
        {
            get { return _Dst_AbsY; }
            set { _Dst_AbsY = value; OnPropertyChanged(); }
        }

        public void UpdateAbsoluteSource()
        {
            if (Source == null) return;

            SrcX = Source.X + (Source.Width * _Src_RelX);
            SrcY = Source.Y + (Source.Height * _Src_RelY);
        }

        public void UpdateAbsoluteDestination()
        {
            if (Destination == null) return;

            DstX = Destination.X + (Destination.Width * _Dst_RelX);
            DstY = Destination.Y + (Destination.Height * _Dst_RelY);
        }

        public void StateChanged()
        {
            if (!DraggingSource && !DraggingDestination && Source == null && Destination == null) return; // big error

            if (!DraggingSource)
            {
                if (Source != null)
                {
                    UpdateAbsoluteSource();
                }
                else
                {
                    if (_Src_RelX == 0)
                    {
                        SrcX = Destination.X + Destination.Width + (20.0);
                        SrcY = Destination.Y + (Destination.Height * _Dst_RelY);
                    }
                    else if (_Src_RelY == 0)
                    {
                        SrcX = Destination.X + (Destination.Width * _Dst_RelX);
                        SrcY = Destination.Y + Destination.Height + (20.0);
                    }
                    else if (_Src_RelX == 1)
                    {
                        SrcX = Destination.X + (-20.0);
                        SrcY = Destination.Y + (Destination.Height * _Dst_RelY);
                    }
                    else if (_Src_RelY == 1)
                    {
                        SrcX = Destination.X + (Destination.Width * _Dst_RelX);
                        SrcY = Destination.Y + (-20.0);
                    }
                }
            }

            if (!DraggingDestination)
            {
                if (Destination != null)
                {
                    UpdateAbsoluteDestination();
                }
                else
                {
                    if (_Dst_RelX == 0)
                    {
                        DstX = Source.X + Source.Width + (20.0);
                        DstY = Source.Y + (Source.Height * _Src_RelY);
                    }
                    else if (_Dst_RelY == 0)
                    {
                        DstX = Source.X + (Source.Width * _Src_RelX);
                        DstY = Source.Y + Source.Height + (20.0);
                    }
                    else if (_Dst_RelX == 1)
                    {
                        DstX = Source.X + (-20.0);
                        DstY = Source.Y + (Source.Height * _Src_RelY);
                    }
                    else if (_Dst_RelY == 1)
                    {
                        DstX = Source.X + (Source.Width * _Src_RelX);
                        DstY = Source.Y + (-20.0);
                    }
                }
            }
        }

        public FSMTransitionView View { get; protected set; }

        public FSMTransition()
        {
            View = new FSMTransitionView(this);
        }

        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public partial class FSMTransitionView : UserControl
    {
        protected FSMTransition m_Transition;
        protected FSMState m_HoveringState;

        protected Point m_DragPoint;

        protected bool m_HandlingSrc;
        protected bool m_HandlingDst;

        public FSMTransitionView(FSMTransition node)
        {
            InitializeComponent();
            DataContext = m_Transition = node;
        }
        
        // INotifyPropertyChanged implement
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void TransitionSource_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !m_Transition.MainCanvas.m_HandlingSomething)
            {
                m_DragPoint = e.GetPosition(sender as FrameworkElement);
                m_DragPoint.X -= 6;
                m_DragPoint.Y -= 6;

                m_Transition.DraggingSource = true;
                m_Transition.DraggingDestination = false;

                m_HandlingSrc = m_Transition.MainCanvas.m_HandlingSomething = true;
            }
        }

        private void TransitionSource_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && m_HandlingSrc)
            {
                m_Transition.DraggingSource = false;
                m_Transition.StateChanged();

                m_HandlingSrc = m_Transition.MainCanvas.m_HandlingSomething = false;
            }
        }

        private void TransitionSource_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && m_HandlingSrc)
            {
                Point position = e.GetPosition(Parent as FrameworkElement);

                var state = m_Transition.Source;
                if (state != null)
                {
                    if (state == m_Transition.Destination) m_Transition.Source = null;
                    else if (position.X + 50 < state.X) m_Transition.Source = null;
                    else if (position.Y + 50 < state.Y) m_Transition.Source = null;
                    else if (position.X + state.Width - 50 > state.X) m_Transition.Source = null;
                    else if (position.Y + state.Height - 50 > state.Y) m_Transition.Source = null;
                }

                VisualTreeHelper.HitTest(Parent as FrameworkElement, null, new HitTestResultCallback(StateResult), new PointHitTestParameters(position));

                m_Transition.Source = m_HoveringState;

                if (m_Transition.Source == null)
                {
                    m_Transition.SrcX = position.X - m_DragPoint.X;
                    m_Transition.SrcY = position.Y - m_DragPoint.Y;
                    return;
                }

                Point p2 = e.GetPosition(sender as FrameworkElement);
                Point p3 = e.GetPosition(m_HoveringState.View);

                if (p3.X < 1.0 * m_HoveringState.Width / 4.0)
                {
                    m_Transition.RelSrcX = 0;
                    m_Transition.RelSrcY = p3.Y / m_HoveringState.Width;
                } else if (p3.X > 3.0 * m_HoveringState.Width / 4.0)
                {
                    m_Transition.RelSrcX = 1;
                    m_Transition.RelSrcY = p3.Y / m_HoveringState.Width;
                }

                if (p3.X < 1.0 * m_HoveringState.Height / 4.0)
                {
                    m_Transition.RelSrcX = p3.X / m_HoveringState.Height;
                    m_Transition.RelSrcY = 0;
                }
                else if (p3.X > 3.0 * m_HoveringState.Height / 4.0)
                {
                    m_Transition.RelSrcX = p3.X / m_HoveringState.Height;
                    m_Transition.RelSrcY = 1;
                }

                m_Transition.UpdateAbsoluteSource();
            }
        }

        private void TransitionDestination_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !m_Transition.MainCanvas.m_HandlingSomething)
            {
                m_DragPoint = e.GetPosition(sender as FrameworkElement);
                m_DragPoint.X -= 6;
                m_DragPoint.Y -= 6;

                m_Transition.DraggingDestination = true;
                m_Transition.DraggingSource = false;

                m_HandlingDst = m_Transition.MainCanvas.m_HandlingSomething = true;
            }
        }

        private void TransitionDestination_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && m_HandlingDst)
            {
                m_Transition.DraggingDestination = false;
                m_Transition.StateChanged();

                m_HandlingDst = m_Transition.MainCanvas.m_HandlingSomething = false;
            }
        }

        private void TransitionDestination_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && m_HandlingDst)
            {
                Point position = e.GetPosition(Parent as FrameworkElement);

                var state = m_Transition.Destination;

                if (state != null) {
                    if (state == m_Transition.Source) m_Transition.Destination = null;
                    else if (position.X + 50 < state.X) m_Transition.Destination = null;
                    else if (position.Y + 50 < state.Y) m_Transition.Destination = null;
                    else if (position.X + state.Width - 50 > state.X) m_Transition.Destination = null;
                    else if (position.Y + state.Height - 50 > state.Y) m_Transition.Destination = null;
                }

                VisualTreeHelper.HitTest(Parent as FrameworkElement, null, new HitTestResultCallback(StateResult), new PointHitTestParameters(position));

                m_Transition.Destination = m_HoveringState;

                if (m_Transition.Destination == null)
                {
                    m_Transition.DstX = position.X - m_DragPoint.X;
                    m_Transition.DstY = position.Y - m_DragPoint.Y;
                    return;
                }

                Point p2 = e.GetPosition(sender as FrameworkElement);
                Point p3 = e.GetPosition(m_HoveringState.View);

                if (p3.X < 1.0 * m_HoveringState.Width / 4.0)
                {
                    m_Transition.RelDstX = 0;
                    m_Transition.RelDstY = p3.Y / m_HoveringState.Width;
                }
                else if (p3.X > 3.0 * m_HoveringState.Width / 4.0)
                {
                    m_Transition.RelDstX = 1;
                    m_Transition.RelDstY = p3.Y / m_HoveringState.Width;
                }

                if (p3.X < 1.0 * m_HoveringState.Height / 4.0)
                {
                    m_Transition.RelDstX = p3.X / m_HoveringState.Height;
                    m_Transition.RelDstY = 0;
                }
                else if (p3.X > 3.0 * m_HoveringState.Height / 4.0)
                {
                    m_Transition.RelDstX = p3.X / m_HoveringState.Height;
                    m_Transition.RelDstY = 1;
                }

                m_Transition.UpdateAbsoluteDestination();
            }
        }

        public HitTestResultBehavior StateResult(HitTestResult result)
        {
            var element = (FrameworkElement)result.VisualHit;
            if (element.DataContext.GetType() == typeof(FSMState))
                m_HoveringState = (FSMState)element.DataContext;

            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }
    }
}
