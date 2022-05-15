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

        private Point _Src;

        public Point Src
        {
            get { return _Src; }
            set { _Src = value; OnPropertyChanged(); }
        }

        public double SrcX
        {
            get { return _Src.X; }
            set { _Src.X = value; OnPropertyChanged(); }
        }

        public double SrcY
        {
            get { return _Src.Y; }
            set { _Src.Y = value; OnPropertyChanged(); }
        }

        private Point _Dst;

        public Point Dst
        {
            get { return _Dst; }
            set { _Dst = value; OnPropertyChanged(); }
        }

        public double DstX
        {
            get { return _Dst.X; }
            set { _Dst.X = value; OnPropertyChanged(); }
        }

        public double DstY
        {
            get { return _Dst.Y; }
            set { _Dst.Y = value; OnPropertyChanged(); }
        }

        public const double NoConnectionOffset = 50.0;

        public void UpdateSource()
        {
            if (DraggingSource)
            {
                return;
            }

            if (Source != null)
            {
                Point p = EditorHelper.ClampPoint(Src, Source.Position, Source.Size);
                SrcX = p.X;
                SrcY = p.Y;
            }
        }

        public void UpdateDestination()
        {
            if (DraggingDestination)
            {
                return;
            }

            if (Destination != null)
            {
                Point p = EditorHelper.ClampPoint(Dst, Destination.Position, Destination.Size);
                DstX = p.X;
                DstY = p.Y;
            }
        }

        public void StateChanged()
        {
            UpdateSource();
            UpdateDestination();
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
                SrcX = double.Parse(editor_data["position_source"].GetAttribute("x"));
                SrcY = double.Parse(editor_data["position_source"].GetAttribute("y"));
                DstX = double.Parse(editor_data["position_destination"].GetAttribute("x"));
                DstY = double.Parse(editor_data["position_destination"].GetAttribute("y"));
            }

            Guard = node["guard"] != null ? node["guard"].InnerText : "";

            StateChanged();
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("transition");

            writer.WriteStartElement("editor_data");

            writer.WriteStartElement("position_source");
            writer.WriteAttributeString("x", SrcX.ToString());
            writer.WriteAttributeString("y", SrcY.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("position_destination");
            writer.WriteAttributeString("x", DstX.ToString());
            writer.WriteAttributeString("y", DstY.ToString());
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
        public const double ConnectionOffset = 10.0;

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

            var canvas = m_Transition.Root.View.canvas;

            m_DragPoint = e.GetPosition(canvas);
            m_DragPoint.X -= 6;
            m_DragPoint.Y -= 6;

            m_Transition.DraggingSource = true;
            m_Transition.DraggingDestination = false;

            m_Transition.Root.Root.Selected = m_Transition;
            m_Transition.Root.Root.Dragging = true;
        }

        private void Source_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_Transition.DraggingSource)
            {
                return;
            }

            var canvas = m_Transition.Root.View.canvas;

            Point position = e.GetPosition(canvas);

            m_HoveringState = null;
            VisualTreeHelper.HitTest(Parent as FrameworkElement, null, new HitTestResultCallback(StateResult), new PointHitTestParameters(position));

            m_Transition.Source = m_HoveringState;

            if (m_Transition.Source == m_Transition.Destination)
            {
                m_Transition.Source = null;
            }

            m_Transition.SrcX = position.X;
            m_Transition.SrcY = position.Y;

            m_Transition.StateChanged();
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

            var canvas = m_Transition.Root.View.canvas;

            m_DragPoint = e.GetPosition(canvas);
            m_DragPoint.X -= 6;
            m_DragPoint.Y -= 6;

            m_Transition.DraggingDestination = true;
            m_Transition.DraggingSource = false;

            m_Transition.Root.Root.Selected = m_Transition;
            m_Transition.Root.Root.Dragging = true;
        }

        private void Destination_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_Transition.DraggingDestination)
            {
                return;
            }

            var canvas = m_Transition.Root.View.canvas;

            Point position = e.GetPosition(canvas);

            m_HoveringState = null;
            VisualTreeHelper.HitTest(Parent as FrameworkElement, null, new HitTestResultCallback(StateResult), new PointHitTestParameters(position));

            m_Transition.Destination = m_HoveringState;

            if (m_Transition.Destination == m_Transition.Source)
            {
                m_Transition.Destination = null;
            }

            m_Transition.DstX = position.X;
            m_Transition.DstY = position.Y;

            m_Transition.StateChanged();
        }

        public void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            m_Transition.DraggingSource = false;
            m_Transition.DraggingDestination = false;
            m_Transition.StateChanged();

            m_Transition.Root.Root.Dragging = false;
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
