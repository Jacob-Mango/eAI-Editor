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
using System.Xml;
using System.Xml.Serialization;

namespace eAIEditor
{
    public class FSMTransition : ViewModelBase
    {
        public FSM Root;

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

        private string _Event;
        public string Event
        {
            get { return _Event; }
            set { _Event = value; OnPropertyChanged(); }
        }

        private string _Guard;
        public string Guard
        {
            get { return _Guard; }
            set { _Guard = value; OnPropertyChanged(); }
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

        public FSMTransition(FSM root)
        {
            Root = root;
            View = new FSMTransitionView(this);
        }

        public void Read(XmlElement node)
        {
            string src = node["from_state"] != null ? node["from_state"].GetAttribute("name") : "";
            string dst = node["to_state"] != null ? node["to_state"].GetAttribute("name") : "";
            Event = node["event"] != null ? node["event"].GetAttribute("name") : "";

            foreach (FSMState state in Root.States)
            {
                if (state.Name == src) Source = state;
                if (state.Name == dst) Destination = state;
            }

            var editor_data = node["editor_data"];
            if (editor_data != null)
            {
                _Src_RelX = double.Parse(editor_data["position_source"].GetAttribute("x"));
                _Src_RelY = double.Parse(editor_data["position_source"].GetAttribute("y"));
                _Dst_RelX = double.Parse(editor_data["position_destination"].GetAttribute("x"));
                _Dst_RelY = double.Parse(editor_data["position_destination"].GetAttribute("y"));
            }

            Guard = node["guard"] != null ? node["guard"].InnerText : "";

            StateChanged();
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("transition");

            writer.WriteStartElement("editor_data");

            writer.WriteStartElement("position_source");
            writer.WriteAttributeString("x", _Src_RelX.ToString());
            writer.WriteAttributeString("y", _Src_RelY.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("position_destination");
            writer.WriteAttributeString("x", _Dst_RelX.ToString());
            writer.WriteAttributeString("y", _Dst_RelY.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.WriteStartElement("guard");
            if (!string.IsNullOrWhiteSpace(Guard))
            {
                writer.WriteString(Guard);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("event");
            if (!string.IsNullOrWhiteSpace(Event))
            {
                writer.WriteAttributeString("name", Event);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("from_state");
            if (Source != null)
            {
                writer.WriteAttributeString("name", Source.Name);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("to_state");
            if (Destination != null)
            {
                writer.WriteAttributeString("name", Destination.Name);
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }

    public partial class FSMTransitionView : UserControl
    {
        protected FSMTransition m_Transition;
        protected FSMState m_HoveringState;

        protected Point m_DragPoint;

        public FSMTransitionView(FSMTransition node)
        {
            InitializeComponent();
            DataContext = m_Transition = node;
        }

        private void Source_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_Transition.Root.Root.Dragging)
            {
                return;
            }

            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            
            m_DragPoint = e.GetPosition(sender as FrameworkElement);
            m_DragPoint.X -= 6;
            m_DragPoint.Y -= 6;

            m_Transition.DraggingSource = true;
            m_Transition.DraggingDestination = false;

            m_Transition.Root.Root.Selected = m_Transition;
            m_Transition.Root.Root.Dragging = true;
        }

        private void Source_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            m_Transition.DraggingSource = false;
            m_Transition.StateChanged();

            m_Transition.Root.Root.Dragging = false;
        }

        private void Source_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_Transition.DraggingSource)
            {
                return;
            }

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
            }
            else if (p3.X > 3.0 * m_HoveringState.Width / 4.0)
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

        private void Destination_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_Transition.Root.Root.Dragging)
            {
                return;
            }

            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            m_DragPoint = e.GetPosition(sender as FrameworkElement);
            m_DragPoint.X -= 6;
            m_DragPoint.Y -= 6;

            m_Transition.DraggingDestination = true;
            m_Transition.DraggingSource = false;

            m_Transition.Root.Root.Selected = m_Transition;
            m_Transition.Root.Root.Dragging = true;
        }

        private void Destination_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            m_Transition.DraggingDestination = false;
            m_Transition.StateChanged();

            m_Transition.Root.Root.Dragging = false;
        }

        private void Destination_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_Transition.DraggingDestination)
            {
                return;
            }

            Point position = e.GetPosition(Parent as FrameworkElement);

            var state = m_Transition.Destination;

            if (state != null)
            {
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

        public void MouseMove(object sender, MouseEventArgs e)
        {
            Source_MouseMove(sender, e);
            Destination_MouseMove(sender, e);
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
