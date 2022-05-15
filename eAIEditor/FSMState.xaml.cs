﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Xml;
using System.Xml.Serialization;

namespace eAIEditor
{
    public class FSMState : ViewModelBase
    {
        public FSM Root;

        public Point Position;
        public Point Size;

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

        private FSM _subFSM;
        public FSM SubFSM
        {
            set { _subFSM = value; OnPropertyChanged(); }
            get { return _subFSM; }
        }

        public double X
        {
            set
            {
                Position.X = value;
                OnPropertyChanged();
            }
            get => Position.X;
        }

        public double Y
        {
            set
            {
                Position.Y = value;
                OnPropertyChanged();
            }
            get => Position.Y;
        }

        public double Width
        {
            set
            {
                Size.X = value;
                OnPropertyChanged();
            }
            get => Size.X;
        }

        public double Height
        {
            set
            {
                Size.Y = value;
                OnPropertyChanged();
            }
            get => Size.Y;
        }

        private string _eventEntry;
        public string EventEntry
        {
            set { _eventEntry = value; OnPropertyChanged(); }
            get { return _eventEntry; }
        }

        private string _eventExit;
        public string EventExit
        {
            set { _eventExit = value; OnPropertyChanged(); }
            get { return _eventExit; }
        }

        private string _guardExit;
        public string GuardExit
        {
            set { _guardExit = value; OnPropertyChanged(); }
            get { return _guardExit; }
        }

        private string _eventUpdate;
        public string EventUpdate
        {
            set { _eventUpdate = value; OnPropertyChanged(); }
            get { return _eventUpdate; }
        }

        public ObservableCollection<FSMVariable> Variables { get; } = new ObservableCollection<FSMVariable>();
        public ObservableCollection<FSMTransition> Transitions { get; } = new ObservableCollection<FSMTransition>();

        public FSMStateView View { get; protected set; }

        public FSMState(FSM root)
        {
            Root = root;
            View = new FSMStateView(this);
        }

        public void Read(XmlElement node)
        {
            Name = node.Attributes["name"].Value;

            var subFSMName = node.Attributes["fsm"];
            if (subFSMName != null)
            {
                SubFSM = Root.Root.Get(subFSMName.Value);
            }

            XmlElement variables = node["variables"];
            if (variables != null)
            {
                foreach (var variable in variables.OfType<XmlElement>())
                {
                    Variables.Add(new FSMVariable(variable));
                }
            }

            var editor_data = node["editor_data"];
            if (editor_data != null)
            {
                X = double.Parse(editor_data["position"].GetAttribute("x"));
                Y = double.Parse(editor_data["position"].GetAttribute("y"));
                Width = double.Parse(editor_data["size"].GetAttribute("width"));
                Height = double.Parse(editor_data["size"].GetAttribute("height"));
            }

            GuardExit = node["guard_exit"] != null ? node["guard_exit"].InnerText : "";
            EventEntry = node["event_entry"] != null ? node["event_entry"].InnerText : "";
            EventExit = node["event_exit"] != null ? node["event_exit"].InnerText : "";
            EventUpdate = node["event_update"] != null ? node["event_update"].InnerText : "";
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("state");
            if (!String.IsNullOrWhiteSpace(Name))
            {
                writer.WriteAttributeString("name", Name);
            }

            if (SubFSM != null)
            {
                writer.WriteAttributeString("fsm", SubFSM.Name);
            }
            
            writer.WriteStartElement("variables");
            foreach (var variable in Variables)
            {
                variable.Write(writer);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("editor_data");
            writer.WriteStartElement("position");
            writer.WriteAttributeString("x", X.ToString());
            writer.WriteAttributeString("y", Y.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("size");
            writer.WriteAttributeString("width", Width.ToString());
            writer.WriteAttributeString("height", Height.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("guard_exit");
            if (!String.IsNullOrWhiteSpace(GuardExit))
            {
                writer.WriteString(GuardExit);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("event_entry");
            if (!String.IsNullOrWhiteSpace(EventEntry))
            {
                writer.WriteString(EventEntry);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("event_exit");
            if (!String.IsNullOrWhiteSpace(EventExit))
            {
                writer.WriteString(EventExit);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("event_update");
            if (!String.IsNullOrWhiteSpace(EventUpdate))
            {
                writer.WriteString(EventUpdate);
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }

    public class FSMVariable : ViewModelBase
    {
        public FSMVariable(XmlElement element)
        {
            Name = element.GetAttribute("name");
            Type = element.GetAttribute("type");
            Default = element.GetAttribute("default");
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("variable");
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("type", Type);
            if (Default != "") writer.WriteAttributeString("default", Default);
            writer.WriteEndElement();
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged(); }
        }

        private string _Type;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; OnPropertyChanged(); }
        }

        private string _Default;

        public string Default
        {
            get { return _Default; }
            set { _Default = value; OnPropertyChanged(); }
        }
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
            InitializeComponent();
            DataContext = m_State = node;
        }

        private void RemoveState(object target, ExecutedRoutedEventArgs e)
        {
            m_State.Root.RemoveState(m_State);
        }

        private void CanRemoveState(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MouseEnter(object sender, MouseEventArgs e)
        {
            HighlightBorder.BorderBrush = BRUSH_HIGHLIGHT;
        }

        private void MouseLeave(object sender, MouseEventArgs e)
        {
            HighlightBorder.BorderBrush = BRUSH_DEFAULT;
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_State.Root.Root.Dragging)
            {
                return;
            }

            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            m_DragPoint = e.GetPosition(this);

            m_State.Root.Root.Selected = m_State;
            m_State.Root.Root.Dragging = true;
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            m_State.Root.Root.Dragging = false;
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            Point position = e.GetPosition(Parent as FrameworkElement);

            m_State.X = position.X - m_DragPoint.X;
            m_State.Y = position.Y - m_DragPoint.Y;

            //Canvas.SetLeft(this, m_State.X);
            //Canvas.SetTop(this, m_State.Y);

            foreach (FSMTransition transition in m_State.Transitions)
            {
                transition.StateChanged();
            }
        }
    }
}
