using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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

            Width = 150;
            Height = 40;
        }

        public void CopyToClipboard()
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                CheckCharacters = true,
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    writer.WriteStartDocument();

                    Write(writer);

                    writer.WriteEndDocument();
                }

                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream))
                {
                    Clipboard.SetText(reader.ReadToEnd());
                }
            }
        }

        public void PasteFromClipboard()
        {
            XmlDocument doc = new XmlDocument();
            string text = Clipboard.GetText();

            using (var stream = new MemoryStream((System.Text.Encoding.UTF8.GetBytes(text))))
            {
                doc.Load(stream);

                Read(doc["state"]);
            }
        }

        public override void UpdateSelected()
        {
            View.UpdateSelected();
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

            EventEntry = Root.ReadCodeElement(node["event_entry"]);
            EventExit = Root.ReadCodeElement(node["event_exit"]);
            EventUpdate = Root.ReadCodeElement(node["event_update"]);
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("state");
            if (!String.IsNullOrWhiteSpace(Name))
            {
                Name.Replace(" ", "_");

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

            Root.WriteCodeElement(writer, "event_entry", EventEntry);
            Root.WriteCodeElement(writer, "event_exit", EventExit);
            Root.WriteCodeElement(writer, "event_update", EventUpdate);

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

        protected Point? m_MousePosition;

        public FSMStateView(FSMState node)
        {
            InitializeComponent();
            DataContext = m_State = node;

            UpdateSelected();
        }

        private void DeleteState(object target, ExecutedRoutedEventArgs e)
        {
            m_State.Root.RemoveState(m_State);
        }

        private void CanDeleteState(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CopyState(object target, ExecutedRoutedEventArgs e)
        {
            m_State.CopyToClipboard();
        }

        private void CanCopyState(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MouseEnter(object sender, MouseEventArgs e)
        {
            HighlightBorder.BorderBrush = BRUSH_HIGHLIGHT;
        }

        private void MouseLeave(object sender, MouseEventArgs e)
        {
            UpdateSelected();
        }

        public void UpdateSelected()
        {
            if (m_State.Root.Root.Selected == m_State)
            {
                HighlightBorder.BorderBrush = BRUSH_SELECTED;
            }
            else
            {
                HighlightBorder.BorderBrush = BRUSH_DEFAULT;
            }
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

            var canvas = m_State.Root.View.canvas;
            m_MousePosition = e.GetPosition(canvas);

            m_State.Root.Root.Selected = m_State;
            m_State.Root.Root.Dragging = true;
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            m_MousePosition = null;

            m_State.Root.Root.Dragging = false;
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (m_MousePosition == null)
            {
                return;
            }

            var canvas = m_State.Root.View.canvas;
            Point position = e.GetPosition(canvas);

            Point delta = new Point();
            delta.X = position.X - m_MousePosition.Value.X;
            delta.Y = position.Y - m_MousePosition.Value.Y;
            m_MousePosition = position;

            m_State.X += delta.X;
            m_State.Y += delta.Y;

            foreach (FSMTransition transition in m_State.Transitions)
            {
                if (transition.Source == m_State || transition.Source == null)
                {
                    transition.SrcX += delta.X;
                    transition.SrcY += delta.Y;
                }

                if (transition.Destination == m_State || transition.Destination == null)
                {
                    transition.DstX += delta.X;
                    transition.DstY += delta.Y;
                }

                transition.StateChanged();
            }
        }
    }
}
